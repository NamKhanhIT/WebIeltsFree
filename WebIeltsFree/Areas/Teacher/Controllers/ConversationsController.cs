using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Utilities;
using System.Text.Json;

namespace WebIeltsFree.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = "teacher,admin")]
public class ConversationsController : Controller
{
    private readonly AppDbContext _context;

    public ConversationsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Teacher/Conversations
    public async Task<IActionResult> Index()
    {
        var currentUserId = GetCurrentUserId();

        // Load Open conversations
        var openConversations = await _context.Conversations
            .Include(c => c.Student)
            .ThenInclude(u => u.Profile)
            .Where(c => c.Status == "open")
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        // Load Active conversations for the current teacher
        var myActiveConversations = await _context.Conversations
            .Include(c => c.Student)
            .ThenInclude(u => u.Profile)
            .Where(c => c.Status == "active" && c.TeacherId == currentUserId)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToListAsync();

        ViewBag.OpenConversations = openConversations;
        ViewBag.MyActiveConversations = myActiveConversations;

        return View();
    }

    // GET: Teacher/Conversations/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Student)
            .ThenInclude(u => u.Profile)
            .Include(c => c.Teacher)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(c => c.ConversationId == id);

        if (conversation == null) return NotFound();

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .ThenInclude(u => u.Profile)
            .Where(m => m.ConversationId == id)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        // Mark unread messages as read
        var unreadMessages = messages.Where(m => !m.IsRead && m.SenderId != GetCurrentUserId()).ToList();
        if (unreadMessages.Any())
        {
            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        ViewBag.Messages = messages;

        return View(conversation);
    }

    // POST: Teacher/Conversations/Reply/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation == null) return NotFound();

        if (conversation.Status == "closed")
        {
            TempData["Error"] = "Cannot reply to a closed conversation.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var currentUserId = GetCurrentUserId();

        var message = new Message
        {
            ConversationId = id,
            SenderId = currentUserId,
            Content = InputSanitizer.StripHtmlTags(content),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        conversation.LastMessageAt = DateTime.UtcNow;

        if (conversation.TeacherId == null && conversation.Status == "open")
        {
            conversation.TeacherId = currentUserId;
            conversation.Status = "active";

            _context.Notifications.Add(new Notification
            {
                UserId = conversation.StudentId,
                Type = "new_message",
                Title = "A teacher has replied",
                Message = "A teacher has joined your conversation and sent a reply.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });
        }
        else
        {
            _context.Notifications.Add(new Notification
            {
                UserId = conversation.StudentId,
                Type = "new_message",
                Title = "New message from your teacher",
                Message = "You have received a new reply in your support conversation.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });
        }

        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "reply_conversation", 
            "Conversation", 
            conversation.ConversationId, 
            null, 
            JsonSerializer.Serialize(new { message.MessageId, message.Content }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Teacher/Conversations/Close/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation == null) return NotFound();

        if (conversation.TeacherId != GetCurrentUserId() && !User.IsInRole("admin"))
        {
            TempData["Error"] = "You can only close your own conversations.";
            return RedirectToAction(nameof(Details), new { id });
        }

        conversation.Status = "closed";
        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "close_conversation", 
            "Conversation", 
            conversation.ConversationId, 
            null, 
            JsonSerializer.Serialize(conversation),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Conversation closed.";
        return RedirectToAction(nameof(Index));
    }
}
