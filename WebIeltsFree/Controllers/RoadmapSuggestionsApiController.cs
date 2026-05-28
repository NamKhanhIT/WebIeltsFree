using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Student-facing API for checking and resolving teacher roadmap suggestions.
/// Used by Dashboard.cshtml JS to implement the blocking-banner flow.
/// </summary>
[ApiController]
[Route("api/roadmap-suggestions")]
[Authorize]
public class RoadmapSuggestionsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<RoadmapSuggestionsApiController> _logger;

    public RoadmapSuggestionsApiController(AppDbContext context, ILogger<RoadmapSuggestionsApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ──────────────────────────────────────────────────────────────
    //  GET /api/roadmap-suggestions/pending
    //  Returns all pending suggestions for the current student.
    //  If non-empty → dashboard is in BLOCKED state.
    // ──────────────────────────────────────────────────────────────
    [HttpGet("pending")]
    public async Task<ActionResult<ApiResponse<List<PendingSuggestionDto>>>> GetPendingSuggestions()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<List<PendingSuggestionDto>>.Fail("Not authenticated"));

        var suggestions = await _context.RoadmapSuggestions
            .Include(s => s.Teacher)
                .ThenInclude(t => t!.Profile)
            .Where(s => s.StudentId == userId && s.Status == "pending")
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new PendingSuggestionDto
            {
                SuggestionId = s.SuggestionId,
                TeacherName = s.Teacher != null && s.Teacher.Profile != null
                    ? s.Teacher.Profile.FullName ?? s.Teacher.Username ?? "Teacher"
                    : "Teacher",
                SuggestionTitle = s.SuggestionTitle,
                Message = s.Message,
                SuggestedChanges = s.SuggestedChanges,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<PendingSuggestionDto>>.Ok(suggestions));
    }

    // ──────────────────────────────────────────────────────────────
    //  PATCH /api/roadmap-suggestions/{id}/accept
    //  Accepts the suggestion and applies changes to roadmap steps.
    // ──────────────────────────────────────────────────────────────
    [HttpPatch("{id}/accept")]
    public async Task<ActionResult<ApiResponse<bool>>> AcceptSuggestion(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<bool>.Fail("Not authenticated"));

        var suggestion = await _context.RoadmapSuggestions
            .FirstOrDefaultAsync(s => s.SuggestionId == id && s.StudentId == userId);

        if (suggestion == null)
            return NotFound(ApiResponse<bool>.Fail("Suggestion not found"));

        if (suggestion.Status != "pending")
            return BadRequest(ApiResponse<bool>.Fail("This suggestion has already been resolved."));

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Mark suggestion as accepted
            suggestion.Status = "accepted";
            suggestion.ResolvedAt = DateTime.UtcNow;

            // 2. Apply suggested changes to roadmap steps
            await ApplySuggestedChanges(suggestion.RoadmapId, suggestion.SuggestedChanges);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Student {UserId} accepted roadmap suggestion {SuggestionId}",
                userId, id);

            return Ok(ApiResponse<bool>.Ok(true, "Suggestion accepted and roadmap updated."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex,
                "Failed to accept suggestion {SuggestionId} for student {UserId}",
                id, userId);
            return StatusCode(500, ApiResponse<bool>.Fail("Failed to apply changes. Please try again."));
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  PATCH /api/roadmap-suggestions/{id}/reject
    //  Rejects the suggestion — roadmap stays unchanged.
    // ──────────────────────────────────────────────────────────────
    [HttpPatch("{id}/reject")]
    public async Task<ActionResult<ApiResponse<bool>>> RejectSuggestion(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<bool>.Fail("Not authenticated"));

        var suggestion = await _context.RoadmapSuggestions
            .FirstOrDefaultAsync(s => s.SuggestionId == id && s.StudentId == userId);

        if (suggestion == null)
            return NotFound(ApiResponse<bool>.Fail("Suggestion not found"));

        if (suggestion.Status != "pending")
            return BadRequest(ApiResponse<bool>.Fail("This suggestion has already been resolved."));

        suggestion.Status = "rejected";
        suggestion.ResolvedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Student {UserId} rejected roadmap suggestion {SuggestionId}",
            userId, id);

        return Ok(ApiResponse<bool>.Ok(true, "Suggestion rejected. Your roadmap is unchanged."));
    }

    // ──────────────────────────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────────────────────────

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
    }

    /// <summary>
    /// Parses the suggested_changes JSON and applies add/remove/modify actions
    /// to the AIRoadmapStep records for the given roadmap.
    /// JSON format: [{ "action": "add|remove|modify", "week_number": N, "description": "..." }]
    /// </summary>
    private async Task ApplySuggestedChanges(int roadmapId, string suggestedChangesJson)
    {
        if (string.IsNullOrWhiteSpace(suggestedChangesJson) || suggestedChangesJson == "[]")
            return;

        List<SuggestedChangeItem>? changes;
        try
        {
            changes = JsonSerializer.Deserialize<List<SuggestedChangeItem>>(suggestedChangesJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse suggested_changes JSON for roadmap {RoadmapId}", roadmapId);
            return;
        }

        if (changes == null || !changes.Any()) return;

        foreach (var change in changes)
        {
            switch (change.Action?.ToLowerInvariant())
            {
                case "add":
                    _context.AIRoadmapSteps.Add(new AIRoadmapStep
                    {
                        RoadmapId = roadmapId,
                        WeekNumber = change.WeekNumber,
                        Description = change.Description,
                        StepType = "teacher_suggested",
                        Priority = 0,
                        IsCompleted = false
                    });
                    break;

                case "remove":
                    var toRemove = await _context.AIRoadmapSteps
                        .Where(s => s.RoadmapId == roadmapId && s.WeekNumber == change.WeekNumber)
                        .ToListAsync();
                    _context.AIRoadmapSteps.RemoveRange(toRemove);
                    break;

                case "modify":
                    var toModify = await _context.AIRoadmapSteps
                        .Where(s => s.RoadmapId == roadmapId && s.WeekNumber == change.WeekNumber)
                        .FirstOrDefaultAsync();
                    if (toModify != null)
                    {
                        toModify.Description = change.Description;
                    }
                    break;
            }
        }
    }
}

// ──────────────────────────────────────────────────────────────
//  DTOs (kept close to the controller that uses them)
// ──────────────────────────────────────────────────────────────

public class PendingSuggestionDto
{
    public int SuggestionId { get; set; }
    public string TeacherName { get; set; } = "";
    public string SuggestionTitle { get; set; } = "";
    public string Message { get; set; } = "";
    public string SuggestedChanges { get; set; } = "[]";
    public DateTime CreatedAt { get; set; }
}

public class SuggestedChangeItem
{
    public string? Action { get; set; }
    public int? WeekNumber { get; set; }
    public string? Description { get; set; }
}
