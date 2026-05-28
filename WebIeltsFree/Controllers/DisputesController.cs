using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;
using WebIeltsFree.Middleware;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Grade Dispute API — students submit, teachers resolve
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisputesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DisputesController> _logger;

    public DisputesController(AppDbContext context, ILogger<DisputesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Student submits a grade dispute
    /// </summary>
    [HttpPost("submit")]
    [Authorize(Roles = "student,teacher,admin")]
    public async Task<ActionResult<ApiResponse<DisputeDto>>> SubmitDispute([FromBody] SubmitDisputeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<DisputeDto>.Fail("Invalid request"));

        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized(ApiResponse<DisputeDto>.Fail("Please log in"));

        var submissionType = InputSanitizer.StripHtmlTags(request.SubmissionType).ToLower();
        if (submissionType != "writing" && submissionType != "speaking")
            return BadRequest(ApiResponse<DisputeDto>.Fail("SubmissionType must be 'writing' or 'speaking'"));

        decimal originalScore = 0;
        if (submissionType == "writing")
        {
            var submission = await _context.WritingSubmissions
                .FirstOrDefaultAsync(w => w.SubmissionId == request.SubmissionId && w.UserId == userId);
            if (submission == null)
                return NotFound(ApiResponse<DisputeDto>.Fail("Writing submission not found"));
            originalScore = submission.BandScore ?? 0;
        }
        else
        {
            var session = await _context.SpeakingSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SubmissionId && s.UserId == userId);
            if (session == null)
                return NotFound(ApiResponse<DisputeDto>.Fail("Speaking session not found"));
            originalScore = (decimal)(session.OverallBand ?? 0);
        }

        // Check for existing open dispute on this submission
        var existingDispute = await _context.GradeDisputes
            .AnyAsync(d => d.UserId == userId
                && d.SubmissionType == submissionType
                && d.SubmissionId == request.SubmissionId
                && (d.Status == "pending" || d.Status == "under_review"));
        if (existingDispute)
            return BadRequest(ApiResponse<DisputeDto>.Fail("You already have an open dispute for this submission"));

        var dispute = new GradeDispute
        {
            UserId = userId,
            SubmissionType = submissionType,
            SubmissionId = request.SubmissionId,
            Reason = InputSanitizer.StripHtmlTags(request.Reason),
            OriginalScore = originalScore,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.GradeDisputes.Add(dispute);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Dispute {DisputeId} submitted by user {UserId}", dispute.DisputeId, userId);

        return Ok(ApiResponse<DisputeDto>.Ok(MapToDto(dispute), "Dispute submitted successfully"));
    }

    /// <summary>
    /// Teacher/Admin: List disputes (pending + under_review by default)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<List<DisputeDto>>>> GetDisputes(
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.GradeDisputes
            .Include(d => d.User).ThenInclude(s => s!.Profile)
            .Include(d => d.Reviewer).ThenInclude(t => t!.Profile)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(d => d.Status == status.ToLower());
        else
            query = query.Where(d => d.Status == "pending" || d.Status == "under_review");

        var total = await query.CountAsync();
        var disputes = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = disputes.Select(MapToDto).ToList();
        return Ok(ApiResponse<List<DisputeDto>>.OkWithPaging(dtos, total, pageNumber, pageSize));
    }

    /// <summary>
    /// Teacher/Admin: Get dispute detail
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<DisputeDto>>> GetDispute(int id)
    {
        var dispute = await _context.GradeDisputes
            .Include(d => d.User).ThenInclude(s => s!.Profile)
            .Include(d => d.Reviewer).ThenInclude(t => t!.Profile)
            .FirstOrDefaultAsync(d => d.DisputeId == id);

        if (dispute == null)
            return NotFound(ApiResponse<DisputeDto>.Fail("Dispute not found"));

        return Ok(ApiResponse<DisputeDto>.Ok(MapToDto(dispute)));
    }

    /// <summary>
    /// Teacher/Admin: Claim a pending dispute for review
    /// </summary>
    [HttpPatch("{id}/claim")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<DisputeDto>>> ClaimDispute(int id)
    {
        var teacherId = GetCurrentUserId();
        var dispute = await _context.GradeDisputes.FindAsync(id);

        if (dispute == null)
            return NotFound(ApiResponse<DisputeDto>.Fail("Dispute not found"));

        if (dispute.Status != "pending")
            return BadRequest(ApiResponse<DisputeDto>.Fail("Dispute is not in pending status"));

        dispute.ReviewedBy = teacherId;
        dispute.Status = "under_review";
        await _context.SaveChangesAsync();

        // Notify student
        _context.Notifications.Add(new Notification
        {
            UserId = dispute.UserId,
            Type = "dispute_claimed",
            Title = "Dispute Under Review",
            Message = "Your grade dispute is being reviewed by a teacher.",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        _logger.LogInformation("Dispute {DisputeId} claimed by teacher {TeacherId}", id, teacherId);
        return Ok(ApiResponse<DisputeDto>.Ok(MapToDto(dispute), "Dispute claimed"));
    }

    /// <summary>
    /// Teacher/Admin: Resolve a dispute (uphold or override score)
    /// </summary>
    [HttpPatch("{id}/resolve")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<DisputeDto>>> ResolveDispute(int id, [FromBody] ResolveDisputeRequest request)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.TeacherNotes))
            return BadRequest(ApiResponse<DisputeDto>.Fail("TeacherNotes is required"));

        var teacherId = GetCurrentUserId();
        var dispute = await _context.GradeDisputes.FindAsync(id);

        if (dispute == null)
            return NotFound(ApiResponse<DisputeDto>.Fail("Dispute not found"));

        if (dispute.Status != "under_review")
            return BadRequest(ApiResponse<DisputeDto>.Fail("Dispute must be under_review to resolve"));

        dispute.Status = "resolved";
        dispute.TeacherNotes = InputSanitizer.StripHtmlTags(request.TeacherNotes);
        dispute.RevisedScore = (decimal?)request.RevisedScore;
        dispute.ResolvedAt = DateTime.UtcNow;
        if (dispute.ReviewedBy == null) dispute.ReviewedBy = teacherId;

        // Override the original submission score if revised
        if (request.RevisedScore.HasValue)
        {
            if (dispute.SubmissionType == "writing")
            {
                var submission = await _context.WritingSubmissions.FindAsync(dispute.SubmissionId);
                if (submission != null) submission.BandScore = (decimal)request.RevisedScore.Value;
            }
            else if (dispute.SubmissionType == "speaking")
            {
                var session = await _context.SpeakingSessions.FindAsync(dispute.SubmissionId);
                if (session != null)
                {
                    // Override all sub-scores proportionally
                    session.FluencyScore = request.RevisedScore.Value;
                    session.PronunciationScore = request.RevisedScore.Value;
                    session.GrammarScore = request.RevisedScore.Value;
                }
            }
        }

        await _context.SaveChangesAsync();

        // Notify student
        var message = request.RevisedScore.HasValue
            ? $"Your grade dispute has been resolved. Score updated to {request.RevisedScore.Value}. Teacher feedback: {dispute.TeacherNotes}"
            : $"Your grade dispute has been resolved. Original score upheld. Teacher feedback: {dispute.TeacherNotes}";

        _context.Notifications.Add(new Notification
        {
            UserId = dispute.UserId,
            Type = "dispute_resolved",
            Title = "Dispute Resolved",
            Message = message,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        _logger.LogInformation("Dispute {DisputeId} resolved by teacher {TeacherId}", id, teacherId);
        return Ok(ApiResponse<DisputeDto>.Ok(MapToDto(dispute), "Dispute resolved"));
    }

    /// <summary>
    /// Teacher/Admin: Reject a dispute with feedback
    /// </summary>
    [HttpPatch("{id}/reject")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<DisputeDto>>> RejectDispute(int id, [FromBody] RejectDisputeRequest request)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.TeacherNotes))
            return BadRequest(ApiResponse<DisputeDto>.Fail("TeacherNotes is required"));

        var teacherId = GetCurrentUserId();
        var dispute = await _context.GradeDisputes.FindAsync(id);

        if (dispute == null)
            return NotFound(ApiResponse<DisputeDto>.Fail("Dispute not found"));

        if (dispute.Status != "under_review")
            return BadRequest(ApiResponse<DisputeDto>.Fail("Dispute must be under_review to reject"));

        dispute.Status = "rejected";
        dispute.TeacherNotes = InputSanitizer.StripHtmlTags(request.TeacherNotes);
        dispute.ResolvedAt = DateTime.UtcNow;
        if (dispute.ReviewedBy == null) dispute.ReviewedBy = teacherId;

        await _context.SaveChangesAsync();

        // Notify student
        _context.Notifications.Add(new Notification
        {
            UserId = dispute.UserId,
            Type = "dispute_rejected",
            Title = "Dispute Rejected",
            Message = $"Your grade dispute has been reviewed and rejected. Teacher feedback: {dispute.TeacherNotes}",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        _logger.LogInformation("Dispute {DisputeId} rejected by teacher {TeacherId}", id, teacherId);
        return Ok(ApiResponse<DisputeDto>.Ok(MapToDto(dispute), "Dispute rejected"));
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private static DisputeDto MapToDto(GradeDispute d)
    {
        return new DisputeDto
        {
            DisputeId = d.DisputeId,
            UserId = d.UserId,
            StudentName = d.User?.Profile?.FullName,
            SubmissionType = d.SubmissionType,
            SubmissionId = d.SubmissionId,
            Reason = d.Reason,
            Status = d.Status,
            ReviewedBy = d.ReviewedBy,
            ReviewerName = d.Reviewer?.Profile?.FullName,
            OriginalScore = (float)d.OriginalScore,
            RevisedScore = (float?)d.RevisedScore,
            TeacherNotes = d.TeacherNotes,
            CreatedAt = d.CreatedAt,
            ResolvedAt = d.ResolvedAt
        };
    }

    #endregion
}
