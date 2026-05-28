using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Notifications API - Manage user notifications and reminders
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(AppDbContext context, ILogger<NotificationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications for current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            IQueryable<Notification> query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var total = await query.CountAsync();
            var notifications = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = notifications.Select(MapToDto).ToList();
            
            return Ok(ApiResponse<List<NotificationDto>>.OkWithPaging(dtos, total, pageNumber, pageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching notifications: {ex.Message}");
            return StatusCode(500, ApiResponse<List<NotificationDto>>.Error("Failed to fetch notifications"));
        }
    }

    /// <summary>
    /// Get single notification by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetNotification(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notification == null)
                return NotFound(ApiResponse<NotificationDto>.Error("Notification not found"));

            return Ok(ApiResponse<NotificationDto>.Ok(MapToDto(notification)));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching notification: {ex.Message}");
            return StatusCode(500, ApiResponse<NotificationDto>.Error("Failed to fetch notification"));
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("{id}/read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notification == null)
                return NotFound(ApiResponse<bool>.Error("Notification not found"));

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Notification {id} marked as read by user {userId}");
            return Ok(ApiResponse<bool>.Ok(true, "Notification marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error marking notification as read: {ex.Message}");
            return StatusCode(500, ApiResponse<bool>.Error("Failed to mark notification as read"));
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("mark-all-read")]
    public async Task<ActionResult<ApiResponse<int>>> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notif in unreadNotifications)
            {
                notif.IsRead = true;
                notif.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Marked {unreadNotifications.Count} notifications as read for user {userId}");
            return Ok(ApiResponse<int>.Ok(unreadNotifications.Count, $"Marked {unreadNotifications.Count} notifications as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error marking all notifications as read: {ex.Message}");
            return StatusCode(500, ApiResponse<int>.Error("Failed to mark notifications as read"));
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteNotification(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notification == null)
                return NotFound(ApiResponse<bool>.Error("Notification not found"));

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Notification {id} deleted by user {userId}");
            return Ok(ApiResponse<bool>.Ok(true, "Notification deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting notification: {ex.Message}");
            return StatusCode(500, ApiResponse<bool>.Error("Failed to delete notification"));
        }
    }

    /// <summary>
    /// Get unread count
    /// </summary>
    [HttpGet("count/unread")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(ApiResponse<int>.Ok(count));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error counting unread notifications: {ex.Message}");
            return StatusCode(500, ApiResponse<int>.Error("Failed to count unread notifications"));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            NotificationId = notification.NotificationId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt
        };
    }

    #endregion
}
