using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Utilities;
using System.Text.Json;

namespace WebIeltsFree.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class AnnouncementsController : Controller
{
    private readonly AppDbContext _context;

    public AnnouncementsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/Announcements
    public async Task<IActionResult> Index()
    {
        var announcements = await _context.Announcements
            .Include(a => a.Teacher)
            .ThenInclude(t => t!.Profile)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return View(announcements);
    }

    // GET: Admin/Announcements/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Announcements/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string content, DateTime? expiresAt)
    {
        var announcement = new Announcement
        {
            TeacherId = GetCurrentUserId(),
            Title = InputSanitizer.Sanitize(title),
            Content = InputSanitizer.Sanitize(content),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "create_announcement",
            "Announcement",
            announcement.AnnouncementId,
            null,
            JsonSerializer.Serialize(new
            {
                announcement.AnnouncementId,
                announcement.TeacherId,
                announcement.Title,
                announcement.Content,
                announcement.IsActive,
                announcement.CreatedAt,
                announcement.ExpiresAt
            }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Announcement posted successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Announcements/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        return View(announcement);
    }

    // POST: Admin/Announcements/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string title, string content, DateTime? expiresAt, bool isActive)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        var oldValuesJson = JsonSerializer.Serialize(new
        {
            announcement.AnnouncementId,
            announcement.TeacherId,
            announcement.Title,
            announcement.Content,
            announcement.IsActive,
            announcement.CreatedAt,
            announcement.ExpiresAt
        });

        announcement.Title = InputSanitizer.Sanitize(title);
        announcement.Content = InputSanitizer.Sanitize(content);
        announcement.ExpiresAt = expiresAt;
        announcement.IsActive = isActive;

        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "edit_announcement",
            "Announcement",
            announcement.AnnouncementId,
            oldValuesJson,
            JsonSerializer.Serialize(new
            {
                announcement.AnnouncementId,
                announcement.TeacherId,
                announcement.Title,
                announcement.Content,
                announcement.IsActive,
                announcement.CreatedAt,
                announcement.ExpiresAt
            }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Announcement updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Announcements/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        var oldValuesJson = JsonSerializer.Serialize(new
        {
            announcement.AnnouncementId,
            announcement.IsActive
        });

        announcement.IsActive = false;
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "deactivate_announcement",
            "Announcement",
            announcement.AnnouncementId,
            oldValuesJson,
            JsonSerializer.Serialize(new
            {
                announcement.AnnouncementId,
                announcement.IsActive
            }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Announcement deactivated.";
        return RedirectToAction(nameof(Index));
    }
}
