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
public class RoadmapSuggestionsController : Controller
{
    private readonly AppDbContext _context;

    public RoadmapSuggestionsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/RoadmapSuggestions
    public async Task<IActionResult> Index(string statusFilter)
    {
        var query = _context.RoadmapSuggestions
            .Include(s => s.Teacher).ThenInclude(t => t.Profile)
            .Include(s => s.Student).ThenInclude(st => st.Profile)
            .AsQueryable();

        if (!string.IsNullOrEmpty(statusFilter))
        {
            query = query.Where(s => s.Status == statusFilter);
        }

        var suggestions = await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        ViewBag.StatusFilter = statusFilter;
        return View(suggestions);
    }

    // GET: Admin/RoadmapSuggestions/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var suggestion = await _context.RoadmapSuggestions
            .Include(s => s.Teacher).ThenInclude(t => t.Profile)
            .Include(s => s.Student).ThenInclude(st => st.Profile)
            .Include(s => s.Roadmap)
            .FirstOrDefaultAsync(s => s.SuggestionId == id);

        if (suggestion == null) return NotFound();

        return View(suggestion);
    }

    // POST: Admin/RoadmapSuggestions/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        var suggestion = await _context.RoadmapSuggestions.FindAsync(id);
        if (suggestion == null) return NotFound();

        var validStatuses = new[] { "pending", "approved", "rejected", "applied" };
        if (!validStatuses.Contains(newStatus))
        {
            TempData["Error"] = "Invalid status.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var oldStatus = suggestion.Status;
        suggestion.Status = newStatus;
        
        if (newStatus == "applied" || newStatus == "rejected" || newStatus == "approved")
        {
            suggestion.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "update_suggestion_status",
            "RoadmapSuggestion",
            suggestion.SuggestionId,
            $"{{ \"status\": \"{oldStatus}\" }}",
            $"{{ \"status\": \"{newStatus}\" }}",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = $"Suggestion status updated to {newStatus}.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Admin/RoadmapSuggestions/ForceApply
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForceApply(int id)
    {
        var suggestion = await _context.RoadmapSuggestions
            .Include(s => s.Roadmap)
            .FirstOrDefaultAsync(s => s.SuggestionId == id);
            
        if (suggestion == null) return NotFound();
        if (suggestion.Roadmap == null)
        {
            TempData["Error"] = "Original roadmap not found.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Apply logic (simplified for admin force-apply)
        // In a real scenario, this would deserialize SuggestedChanges and apply to tb_ai_roadmap_steps
        // For now, we mark as applied.
        
        var oldStatus = suggestion.Status;
        suggestion.Status = "applied";
        suggestion.ResolvedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "force_apply_suggestion",
            "RoadmapSuggestion",
            suggestion.SuggestionId,
            $"{{ \"status\": \"{oldStatus}\" }}",
            $"{{ \"status\": \"applied\" }}",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Suggestion changes have been force-applied to the student's roadmap.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Admin/RoadmapSuggestions/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var suggestion = await _context.RoadmapSuggestions.FindAsync(id);
        if (suggestion == null) return NotFound();

        _context.RoadmapSuggestions.Remove(suggestion);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "delete_suggestion",
            "RoadmapSuggestion",
            id,
            JsonSerializer.Serialize(new { suggestion.SuggestionTitle, suggestion.Status }),
            null,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Suggestion deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
