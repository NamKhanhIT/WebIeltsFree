using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Utilities;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;

namespace WebIeltsFree.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = "teacher,admin")]
public class WritingPromptsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public WritingPromptsController(AppDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Teacher/WritingPrompts
    public async Task<IActionResult> Index(string activeTab = "all")
    {
        var query = _context.WritingPrompts.Where(p => !p.IsDeleted);

        if (activeTab == "draft") query = query.Where(p => p.Status == "draft");
        else if (activeTab == "pending_review") query = query.Where(p => p.Status == "pending_review");
        else if (activeTab == "published") query = query.Where(p => p.Status == "published");
        
        var prompts = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        
        // Let's get creator names (for a small dataset, this is fine; or we can join if Users isn't navigated)
        var userIds = prompts.Where(p => p.CreatedBy.HasValue).Select(p => p.CreatedBy.Value).Distinct().ToList();
        var users = await _context.UserProfiles.Where(up => userIds.Contains(up.UserId)).ToDictionaryAsync(up => up.UserId, up => up.FullName ?? "Unknown");
        ViewBag.Creators = users;

        ViewBag.ActiveTab = activeTab;
        
        ViewBag.AllCount = await _context.WritingPrompts.CountAsync(p => !p.IsDeleted);
        ViewBag.DraftCount = await _context.WritingPrompts.CountAsync(p => p.Status == "draft");
        ViewBag.PendingCount = await _context.WritingPrompts.CountAsync(p => p.Status == "pending_review");
        ViewBag.PublishedCount = await _context.WritingPrompts.CountAsync(p => p.Status == "published");

        return View(prompts);
    }

    // GET: Teacher/WritingPrompts/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        var creatorProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == prompt.CreatedBy);
        ViewBag.CreatorName = creatorProfile?.FullName ?? "Unknown";

        return View(prompt);
    }

    // GET: Teacher/WritingPrompts/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Teacher/WritingPrompts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WritingPromptViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PromptText))
        {
            ModelState.AddModelError("PromptText", "Prompt text is required.");
            return View(model);
        }

        string? promptImageUrl = null;

        if (model.ImageFile != null)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Only .jpg, .png, .jpeg files are allowed.");
                return View(model);
            }

            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "writing");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(fileStream);
            }

            promptImageUrl = "/uploads/writing/" + uniqueFileName;
        }

        var prompt = new WritingPrompt
        {
            TaskType = model.TaskType,
            PromptText = InputSanitizer.StripHtmlTags(model.PromptText),
            SampleAnswer = string.IsNullOrWhiteSpace(model.SampleAnswer) ? null : InputSanitizer.Sanitize(model.SampleAnswer),
            TargetBand = model.TargetBand,
            PromptImageUrl = promptImageUrl,
            Status = "draft",
            CreatedBy = GetCurrentUserId(),
            CreatedAt = DateTime.UtcNow
        };

        _context.WritingPrompts.Add(prompt);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "create_writing_prompt", 
            "WritingPrompt", 
            prompt.PromptId, 
            null, 
            JsonSerializer.Serialize(prompt),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Writing prompt created and saved as draft.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Teacher/WritingPrompts/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        return View(prompt);
    }

    // POST: Teacher/WritingPrompts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WritingPromptViewModel model)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        if (string.IsNullOrWhiteSpace(model.PromptText))
        {
            ModelState.AddModelError("PromptText", "Prompt text is required.");
            return View(prompt);
        }

        if (model.ImageFile != null)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Only .jpg, .png, .jpeg files are allowed.");
                return View(prompt);
            }

            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "writing");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Delete old file if exists
            if (!string.IsNullOrEmpty(prompt.PromptImageUrl))
            {
                var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, prompt.PromptImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(fileStream);
            }

            prompt.PromptImageUrl = "/uploads/writing/" + uniqueFileName;
        }

        prompt.TaskType = model.TaskType;
        prompt.PromptText = InputSanitizer.Sanitize(model.PromptText);
        prompt.SampleAnswer = string.IsNullOrWhiteSpace(model.SampleAnswer) ? null : InputSanitizer.Sanitize(model.SampleAnswer);
        prompt.TargetBand = model.TargetBand;
        
        // Editing resets status to draft if it was published or rejected
        if (prompt.Status == "published" || prompt.Status == "rejected")
        {
            prompt.Status = "draft";
        }

        if (model.Action == "submit_review")
        {
            prompt.Status = "pending_review";
        }

        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "edit_writing_prompt", 
            "WritingPrompt", 
            prompt.PromptId, 
            null, 
            JsonSerializer.Serialize(prompt),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = model.Action == "submit_review" ? "Prompt submitted for peer review." : "Prompt updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Teacher/WritingPrompts/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        if (prompt.CreatedBy == GetCurrentUserId() && !User.IsInRole("admin"))
        {
            TempData["Error"] = "You cannot approve your own content.";
            return RedirectToAction(nameof(Details), new { id = prompt.PromptId });
        }

        prompt.Status = "published";
        prompt.ReviewedBy = GetCurrentUserId();
        prompt.ReviewedAt = DateTime.UtcNow;

        if (prompt.CreatedBy.HasValue)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = prompt.CreatedBy.Value,
                Type = "content_approved",
                Title = "Content Approved",
                Message = "Your writing prompt has been approved and published.",
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
            "WritingPrompt", 
            prompt.PromptId, 
            null, 
            JsonSerializer.Serialize(prompt),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Writing prompt approved successfully.";
        return RedirectToAction(nameof(Details), new { id = prompt.PromptId });
    }

    // POST: Teacher/WritingPrompts/Reject/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        if (prompt.CreatedBy == GetCurrentUserId() && !User.IsInRole("admin"))
        {
            TempData["Error"] = "You cannot reject your own content.";
            return RedirectToAction(nameof(Details), new { id = prompt.PromptId });
        }

        prompt.Status = "rejected";
        prompt.ReviewedBy = GetCurrentUserId();
        prompt.ReviewedAt = DateTime.UtcNow;

        if (prompt.CreatedBy.HasValue)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = prompt.CreatedBy.Value,
                Type = "content_rejected",
                Title = "Content Rejected",
                Message = "Your writing prompt has been rejected during peer review.",
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
            "WritingPrompt", 
            prompt.PromptId, 
            null, 
            JsonSerializer.Serialize(prompt),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Writing prompt rejected.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Teacher/WritingPrompts/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        return View(prompt);
    }

    // POST: Teacher/WritingPrompts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null) return NotFound();

        // Delete file if exists to save disk space
        if (!string.IsNullOrEmpty(prompt.PromptImageUrl))
        {
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, prompt.PromptImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // Soft delete
        prompt.IsDeleted = true;
        prompt.Status = "rejected"; // Keep for backward compatibility
        
        await _context.SaveChangesAsync();
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "soft_delete_writing_prompt", 
            "WritingPrompt", 
            prompt.PromptId, 
            null, 
            JsonSerializer.Serialize(prompt),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Writing prompt archived successfully.";
        return RedirectToAction(nameof(Index));
    }
}
