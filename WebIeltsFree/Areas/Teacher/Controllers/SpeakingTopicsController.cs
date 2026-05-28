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
public class SpeakingTopicsController : Controller
{
    private readonly AppDbContext _context;

    public SpeakingTopicsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Teacher/SpeakingTopics
    public async Task<IActionResult> Index(string activeTab = "all")
    {
        var query = _context.SpeakingTopics.Where(p => !p.IsDeleted);

        if (activeTab == "draft") query = query.Where(p => p.Status == "draft");
        else if (activeTab == "pending_review") query = query.Where(p => p.Status == "pending_review");
        else if (activeTab == "published") query = query.Where(p => p.Status == "published");
        
        var topics = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        
        var userIds = topics.Where(p => p.CreatedBy.HasValue).Select(p => p.CreatedBy.Value).Distinct().ToList();
        var users = await _context.UserProfiles.Where(up => userIds.Contains(up.UserId)).ToDictionaryAsync(up => up.UserId, up => up.FullName ?? "Unknown");
        ViewBag.Creators = users;

        ViewBag.ActiveTab = activeTab;
        
        ViewBag.AllCount = await _context.SpeakingTopics.CountAsync(p => !p.IsDeleted);
        ViewBag.DraftCount = await _context.SpeakingTopics.CountAsync(p => p.Status == "draft");
        ViewBag.PendingCount = await _context.SpeakingTopics.CountAsync(p => p.Status == "pending_review");
        ViewBag.PublishedCount = await _context.SpeakingTopics.CountAsync(p => p.Status == "published");

        return View(topics);
    }

    // GET: Teacher/SpeakingTopics/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        var creatorProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == topic.CreatedBy);
        ViewBag.CreatorName = creatorProfile?.FullName ?? "Unknown";

        return View(topic);
    }

    // GET: Teacher/SpeakingTopics/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Teacher/SpeakingTopics/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string topicName, int partNumber, decimal targetBand)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            ModelState.AddModelError("TopicName", "Topic Name is required.");
            return View();
        }

        var topic = new SpeakingTopic
        {
            TopicName = InputSanitizer.Sanitize(topicName),
            PartNumber = partNumber,
            TargetBand = targetBand,
            Status = "draft",
            CreatedBy = GetCurrentUserId(),
            CreatedAt = DateTime.UtcNow
        };

        _context.SpeakingTopics.Add(topic);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "create_speaking_topic", 
            "SpeakingTopic", 
            topic.TopicId, 
            null, 
            JsonSerializer.Serialize(topic),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Speaking topic created and saved as draft.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Teacher/SpeakingTopics/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        return View(topic);
    }

    // POST: Teacher/SpeakingTopics/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string topicName, int partNumber, decimal targetBand, string action)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        if (string.IsNullOrWhiteSpace(topicName))
        {
            ModelState.AddModelError("TopicName", "Topic Name is required.");
            return View(topic);
        }

        topic.TopicName = InputSanitizer.Sanitize(topicName);
        topic.PartNumber = partNumber;
        topic.TargetBand = targetBand;
        
        if (topic.Status == "published" || topic.Status == "rejected")
        {
            topic.Status = "draft";
        }

        if (action == "submit_review")
        {
            topic.Status = "pending_review";
        }

        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "edit_speaking_topic", 
            "SpeakingTopic", 
            topic.TopicId, 
            null, 
            JsonSerializer.Serialize(topic),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = action == "submit_review" ? "Topic submitted for peer review." : "Topic updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Teacher/SpeakingTopics/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        if (topic.CreatedBy == GetCurrentUserId() && !User.IsInRole("admin"))
        {
            TempData["Error"] = "You cannot approve your own content.";
            return RedirectToAction(nameof(Details), new { id = topic.TopicId });
        }

        topic.Status = "published";
        topic.ReviewedBy = GetCurrentUserId();
        topic.ReviewedAt = DateTime.UtcNow;

        if (topic.CreatedBy.HasValue)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = topic.CreatedBy.Value,
                Type = "content_approved",
                Title = "Content Approved",
                Message = "Your speaking topic has been approved and published.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });
        }

        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "approve_content", 
            "SpeakingTopic", 
            topic.TopicId, 
            null, 
            JsonSerializer.Serialize(topic),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Speaking topic approved successfully.";
        return RedirectToAction(nameof(Details), new { id = topic.TopicId });
    }

    // POST: Teacher/SpeakingTopics/Reject/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        if (topic.CreatedBy == GetCurrentUserId() && !User.IsInRole("admin"))
        {
            TempData["Error"] = "You cannot reject your own content.";
            return RedirectToAction(nameof(Details), new { id = topic.TopicId });
        }

        topic.Status = "rejected";
        topic.ReviewedBy = GetCurrentUserId();
        topic.ReviewedAt = DateTime.UtcNow;

        if (topic.CreatedBy.HasValue)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = topic.CreatedBy.Value,
                Type = "content_rejected",
                Title = "Content Rejected",
                Message = "Your speaking topic has been rejected during peer review.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });
        }

        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "reject_content", 
            "SpeakingTopic", 
            topic.TopicId, 
            null, 
            JsonSerializer.Serialize(topic),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Speaking topic rejected.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Teacher/SpeakingTopics/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        return View(topic);
    }

    // POST: Teacher/SpeakingTopics/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null) return NotFound();

        // Soft delete
        topic.IsDeleted = true;
        topic.Status = "rejected"; // Keep for backward compatibility
        
        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "soft_delete_speaking_topic", 
            "SpeakingTopic", 
            topic.TopicId, 
            null, 
            JsonSerializer.Serialize(topic),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Speaking topic archived successfully.";
        return RedirectToAction(nameof(Index));
    }
}
