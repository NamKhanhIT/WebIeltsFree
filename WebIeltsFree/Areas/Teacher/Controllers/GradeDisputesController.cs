using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Middleware;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text.Json;

namespace WebIeltsFree.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = "teacher,admin")]
public class GradeDisputesController : Controller
{
    private readonly AppDbContext _context;

    public GradeDisputesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Teacher/GradeDisputes
    public async Task<IActionResult> Index(string activeTab = "pending")
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("admin");

        var allDisputesQuery = _context.GradeDisputes
            .Include(d => d.User)
            .ThenInclude(u => u.Profile)
            .AsQueryable();

        var pendingCount = await allDisputesQuery.CountAsync(d => d.Status == "pending");
        var underReviewCount = await allDisputesQuery.CountAsync(d => d.Status == "under_review" && (isAdmin || d.ReviewedBy == currentUserId));
        var resolvedCount = await allDisputesQuery.CountAsync(d => d.Status == "resolved");
        var rejectedCount = await allDisputesQuery.CountAsync(d => d.Status == "rejected");

        ViewBag.PendingCount = pendingCount;
        ViewBag.UnderReviewCount = underReviewCount;
        ViewBag.ResolvedCount = resolvedCount;
        ViewBag.RejectedCount = rejectedCount;
        ViewBag.ActiveTab = activeTab;

        IQueryable<GradeDispute> query = allDisputesQuery;

        if (activeTab == "pending")
        {
            query = query.Where(d => d.Status == "pending");
        }
        else if (activeTab == "under_review")
        {
            query = query.Where(d => d.Status == "under_review" && (isAdmin || d.ReviewedBy == currentUserId));
        }
        else if (activeTab == "resolved")
        {
            query = query.Where(d => d.Status == "resolved");
        }
        else if (activeTab == "rejected")
        {
            query = query.Where(d => d.Status == "rejected");
        }
        else
        {
            query = query.Where(d => d.Status == "pending");
        }

        var disputes = await query.OrderByDescending(d => d.CreatedAt).ToListAsync();

        return View(disputes);
    }

    // GET: Teacher/GradeDisputes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var dispute = await _context.GradeDisputes
            .Include(d => d.User)
            .ThenInclude(u => u.Profile)
            .Include(d => d.Reviewer)
            .ThenInclude(r => r.Profile)
            .FirstOrDefaultAsync(d => d.DisputeId == id);

        if (dispute == null) return NotFound();

        if (dispute.SubmissionType == "writing")
        {
            var writing = await _context.WritingSubmissions.FirstOrDefaultAsync(w => w.SubmissionId == dispute.SubmissionId);
            ViewBag.WritingSubmission = writing;
        }
        else if (dispute.SubmissionType == "speaking")
        {
            var speaking = await _context.SpeakingSessions.FirstOrDefaultAsync(s => s.SessionId == dispute.SubmissionId);
            ViewBag.SpeakingSession = speaking;
        }

        return View(dispute);
    }

    // POST: Teacher/GradeDisputes/Claim/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Claim(int id)
    {
        var dispute = await _context.GradeDisputes.FindAsync(id);
        if (dispute == null) return NotFound();

        if (dispute.Status != "pending")
        {
            TempData["Error"] = "Only pending disputes can be claimed.";
            return RedirectToAction(nameof(Index));
        }

        dispute.Status = "under_review";
        dispute.ReviewedBy = GetCurrentUserId();

        _context.Notifications.Add(new Notification
        {
            UserId = dispute.UserId,
            Type = "dispute_claimed",
            Title = "Your dispute is being reviewed",
            Message = "A teacher has picked up your grade dispute and will respond shortly.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "claim_dispute", 
            "GradeDispute", 
            dispute.DisputeId, 
            null, 
            JsonSerializer.Serialize(dispute),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Dispute successfully claimed.";
        return RedirectToAction(nameof(Details), new { id = dispute.DisputeId });
    }

    // GET: Teacher/GradeDisputes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var dispute = await _context.GradeDisputes
            .Include(d => d.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(d => d.DisputeId == id);

        if (dispute == null) return NotFound();

        if (dispute.Status != "under_review" || (!User.IsInRole("admin") && dispute.ReviewedBy != GetCurrentUserId()))
        {
            TempData["Error"] = "You can only resolve disputes that are under your review.";
            return RedirectToAction(nameof(Index));
        }

        if (dispute.SubmissionType == "writing")
        {
            var writing = await _context.WritingSubmissions.FirstOrDefaultAsync(w => w.SubmissionId == dispute.SubmissionId);
            ViewBag.WritingSubmission = writing;
        }
        else if (dispute.SubmissionType == "speaking")
        {
            var speaking = await _context.SpeakingSessions.FirstOrDefaultAsync(s => s.SessionId == dispute.SubmissionId);
            ViewBag.SpeakingSession = speaking;
        }

        return View(dispute);
    }

    // POST: Teacher/GradeDisputes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string action, string teacherNotes, decimal? revisedScore, decimal? taScore, decimal? ccScore, decimal? lrScore, decimal? graScore, string? feedbackDetails)
    {
        var dispute = await _context.GradeDisputes.FindAsync(id);
        if (dispute == null) return NotFound();

        if (string.IsNullOrWhiteSpace(teacherNotes))
        {
            ModelState.AddModelError("TeacherNotes", "Teacher Notes are required.");
            // Re-load view data
            return await Edit(id);
        }

        teacherNotes = InputSanitizer.StripHtmlTags(teacherNotes);

        if (action == "reject")
        {
            dispute.Status = "rejected";
            dispute.TeacherNotes = teacherNotes;
            dispute.ResolvedAt = DateTime.UtcNow;

            _context.Notifications.Add(new Notification
            {
                UserId = dispute.UserId,
                Type = "dispute_rejected",
                Title = "Grade dispute rejected",
                Message = $"Teacher notes: {teacherNotes}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });

            await _context.SaveChangesAsync();
            await AuditHelper.LogAsync(
                _context, 
                GetCurrentUserId(), 
                "reject_dispute", 
                "GradeDispute", 
                dispute.DisputeId, 
                null, 
                JsonSerializer.Serialize(dispute),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Dispute rejected.";
            return RedirectToAction(nameof(Index), new { activeTab = "rejected" });
        }
        else if (action == "resolve")
        {
            dispute.Status = "resolved";
            dispute.TeacherNotes = teacherNotes;
            dispute.RevisedScore = revisedScore;
            dispute.ResolvedAt = DateTime.UtcNow;

            if (revisedScore.HasValue)
            {
                if (dispute.SubmissionType == "writing")
                {
                    var submission = await _context.WritingSubmissions.FindAsync(dispute.SubmissionId);
                    if (submission != null) 
                    {
                        submission.BandScore = revisedScore.Value;
                        if (taScore.HasValue) submission.TaScore = taScore.Value;
                        if (ccScore.HasValue) submission.CcScore = ccScore.Value;
                        if (lrScore.HasValue) submission.LrScore = lrScore.Value;
                        if (graScore.HasValue) submission.GraScore = graScore.Value;
                        if (!string.IsNullOrWhiteSpace(feedbackDetails)) submission.FeedbackDetails = InputSanitizer.StripHtmlTags(feedbackDetails);
                    }
                }
                else if (dispute.SubmissionType == "speaking")
                {
                    var session = await _context.SpeakingSessions.FindAsync(dispute.SubmissionId);
                    // SpeakingSession doesn't have a BandScore to update directly as it's computed.
                }
            }

            var msg = revisedScore.HasValue ? $"Revised score: {revisedScore.Value}. Notes: {teacherNotes}" : $"Original score upheld. Notes: {teacherNotes}";

            _context.Notifications.Add(new Notification
            {
                UserId = dispute.UserId,
                Type = "dispute_resolved",
                Title = "Grade dispute resolved",
                Message = msg,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });

            await _context.SaveChangesAsync();
            await AuditHelper.LogAsync(
                _context, 
                GetCurrentUserId(), 
                "resolve_dispute", 
                "GradeDispute", 
                dispute.DisputeId, 
                null, 
                JsonSerializer.Serialize(dispute),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Dispute resolved successfully.";
            return RedirectToAction(nameof(Index), new { activeTab = "resolved" });
        }

        return BadRequest("Invalid action.");
    }
}
