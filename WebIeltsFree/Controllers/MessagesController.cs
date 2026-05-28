using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;
using WebIeltsFree.Middleware;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Messaging API — async direct messaging between students and teachers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(AppDbContext context, ILogger<MessagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Student starts a new conversation
    /// </summary>
    [HttpPost("conversations/start")]
    [Authorize(Roles = "student")]
    public async Task<ActionResult<ApiResponse<ConversationDto>>> StartConversation([FromBody] StartConversationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ConversationDto>.Fail("Invalid request"));

        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized(ApiResponse<ConversationDto>.Fail("Please log in"));

        var conversation = new Conversation
        {
            StudentId = userId,
            Subject = request.Subject != null ? InputSanitizer.StripHtmlTags(request.Subject) : null,
            Status = "open",
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        // Add the initial message
        var message = new Message
        {
            ConversationId = conversation.ConversationId,
            SenderId = userId,
            Content = InputSanitizer.StripHtmlTags(request.Message),
            CreatedAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Notify all teachers about the new conversation
        var teachers = await _context.Users
            .Where(u => u.Role == "teacher" && !u.IsDeleted)
            .Select(u => u.UserId)
            .ToListAsync();

        foreach (var teacherId in teachers)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = teacherId,
                Type = "new_conversation",
                Title = "New Student Conversation",
                Message = $"A student has started a new conversation: {conversation.Subject ?? "No subject"}",
                CreatedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<ConversationDto>.Ok(await MapConversationToDto(conversation, userId), "Conversation started"));
    }

    /// <summary>
    /// List conversations — students see own; teachers see all open/active
    /// </summary>
    [HttpGet("conversations")]
    public async Task<ActionResult<ApiResponse<List<ConversationDto>>>> GetConversations(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        IQueryable<Conversation> query = _context.Conversations
            .Include(c => c.Student).ThenInclude(s => s!.Profile)
            .Include(c => c.Teacher).ThenInclude(t => t!.Profile);

        if (role == "student")
            query = query.Where(c => c.StudentId == userId);
        else
            query = query.Where(c => c.Status == "open" || c.Status == "active");

        var total = await query.CountAsync();
        var conversations = await query
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<ConversationDto>();
        foreach (var c in conversations)
            dtos.Add(await MapConversationToDto(c, userId));

        return Ok(ApiResponse<List<ConversationDto>>.OkWithPaging(dtos, total, pageNumber, pageSize));
    }

    /// <summary>
    /// Get messages in a conversation thread
    /// </summary>
    [HttpGet("conversations/{id}/messages")]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessages(int id,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation == null)
            return NotFound(ApiResponse<List<MessageDto>>.Fail("Conversation not found"));

        // Enforce ownership for students
        if (role == "student" && conversation.StudentId != userId)
            return StatusCode(403, ApiResponse<List<MessageDto>>.Fail("Access denied"));

        var query = _context.Messages
            .Where(m => m.ConversationId == id)
            .Include(m => m.Sender).ThenInclude(s => s!.Profile)
            .OrderByDescending(m => m.CreatedAt);

        var total = await query.CountAsync();
        var messages = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                SenderName = m.Sender != null && m.Sender.Profile != null ? m.Sender.Profile.FullName : m.Sender!.Email,
                SenderRole = m.Sender != null ? m.Sender.Role : null,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<MessageDto>>.OkWithPaging(messages, total, pageNumber, pageSize));
    }

    /// <summary>
    /// Send a message in a conversation
    /// </summary>
    [HttpPost("conversations/{id}/send")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage(int id, [FromBody] SendMessageRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<MessageDto>.Fail("Invalid request"));

        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation == null)
            return NotFound(ApiResponse<MessageDto>.Fail("Conversation not found"));

        // Students can only send to their own conversations
        if (role == "student" && conversation.StudentId != userId)
            return StatusCode(403, ApiResponse<MessageDto>.Fail("Access denied"));

        if (conversation.Status == "closed")
            return BadRequest(ApiResponse<MessageDto>.Fail("This conversation is closed"));

        // If teacher is replying and not yet assigned, assign them
        if ((role == "teacher" || role == "admin") && conversation.TeacherId == null)
        {
            conversation.TeacherId = userId;
            conversation.Status = "active";
        }

        var message = new Message
        {
            ConversationId = id,
            SenderId = userId,
            Content = InputSanitizer.StripHtmlTags(request.Content),
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        conversation.LastMessageAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Notify the other party
        int notifyUserId;
        if (role == "student")
        {
            // If teacher is assigned, notify them
            if (conversation.TeacherId.HasValue)
                notifyUserId = conversation.TeacherId.Value;
            else
                notifyUserId = 0; // No teacher yet — already notified via new_conversation
        }
        else
        {
            notifyUserId = conversation.StudentId;
        }

        if (notifyUserId > 0)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = notifyUserId,
                Type = "new_message",
                Title = "New Message",
                Message = $"You have a new message in conversation: {conversation.Subject ?? "No subject"}",
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        var sender = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == userId);

        return Ok(ApiResponse<MessageDto>.Ok(new MessageDto
        {
            MessageId = message.MessageId,
            SenderId = userId,
            SenderName = sender?.Profile?.FullName ?? sender?.Email,
            SenderRole = role,
            Content = message.Content,
            IsRead = false,
            CreatedAt = message.CreatedAt
        }, "Message sent"));
    }

    /// <summary>
    /// Mark messages in a conversation as read
    /// </summary>
    [HttpPatch("conversations/{id}/mark-read")]
    public async Task<ActionResult<ApiResponse<int>>> MarkRead(int id)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation == null)
            return NotFound(ApiResponse<int>.Fail("Conversation not found"));

        if (role == "student" && conversation.StudentId != userId)
            return StatusCode(403, ApiResponse<int>.Fail("Access denied"));

        // Mark messages from the OTHER party as read
        var unread = await _context.Messages
            .Where(m => m.ConversationId == id && m.SenderId != userId && !m.IsRead)
            .ToListAsync();

        foreach (var msg in unread)
            msg.IsRead = true;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<int>.Ok(unread.Count, $"Marked {unread.Count} messages as read"));
    }

    /// <summary>
    /// Teacher/Admin: Close a conversation
    /// </summary>
    [HttpPatch("conversations/{id}/close")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> CloseConversation(int id)
    {
        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation == null)
            return NotFound(ApiResponse<bool>.Fail("Conversation not found"));

        conversation.Status = "closed";
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Ok(true, "Conversation closed"));
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "student";
    }

    private async Task<ConversationDto> MapConversationToDto(Conversation c, int currentUserId)
    {
        var unreadCount = await _context.Messages
            .CountAsync(m => m.ConversationId == c.ConversationId && m.SenderId != currentUserId && !m.IsRead);

        return new ConversationDto
        {
            ConversationId = c.ConversationId,
            StudentId = c.StudentId,
            StudentName = c.Student?.Profile?.FullName ?? c.Student?.Email,
            TeacherId = c.TeacherId,
            TeacherName = c.Teacher?.Profile?.FullName ?? c.Teacher?.Email,
            Subject = c.Subject,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            LastMessageAt = c.LastMessageAt,
            UnreadCount = unreadCount
        };
    }

    #endregion
}
