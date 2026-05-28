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

    // GET: Teacher/RoadmapSuggestions
    public async Task<IActionResult> Index()
    {
        var teacherId = GetCurrentUserId();
        var suggestions = await _context.RoadmapSuggestions
            .Include(s => s.Student)
            .ThenInclude(u => u!.Profile)
            .Where(s => s.TeacherId == teacherId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return View(suggestions);
    }

    // GET: Teacher/RoadmapSuggestions/Suggest
    public async Task<IActionResult> Suggest()
    {
        // Get all students
        var students = await _context.Users
            .Include(u => u.Profile)
            .Where(u => u.Role == "student" && u.Status == "active")
            .ToListAsync();

        ViewBag.Students = students;

        return View();
    }

    // POST: Teacher/RoadmapSuggestions/Suggest
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Suggest(int studentId, string suggestionTitle, string message, int weekNumber, string actionType, string changeDescription)
    {
        var roadmap = await _context.AIRoadmaps
            .Where(r => r.UserId == studentId)
            .OrderByDescending(r => r.GeneratedAt)
            .Select(r => new { r.RoadmapId })
            .FirstOrDefaultAsync();

        if (roadmap == null)
        {
            TempData["Error"] = "This student does not have an active roadmap.";
            return RedirectToAction(nameof(Suggest));
        }

        var changesList = new List<object>
        {
            new { action = actionType, week_number = weekNumber, description = InputSanitizer.Sanitize(changeDescription) }
        };

        var suggestion = new RoadmapSuggestion
        {
            TeacherId = GetCurrentUserId(),
            StudentId = studentId,
            RoadmapId = roadmap.RoadmapId,
            SuggestionTitle = InputSanitizer.Sanitize(suggestionTitle),
            Message = InputSanitizer.Sanitize(message),
            SuggestedChanges = JsonSerializer.Serialize(changesList),
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.RoadmapSuggestions.Add(suggestion);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "suggest_roadmap_change",
            "RoadmapSuggestion",
            suggestion.SuggestionId,
            null,
            JsonSerializer.Serialize(suggestion),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Roadmap suggestion sent successfully.";
        return RedirectToAction(nameof(Index));
    }
}
