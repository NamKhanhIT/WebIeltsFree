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
public class TestAssignmentsController : Controller
{
    private readonly AppDbContext _context;

    public TestAssignmentsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Teacher/TestAssignments
    public async Task<IActionResult> Index()
    {
        var teacherId = GetCurrentUserId();
        
        var assignedByMe = await _context.TeacherAssignments
            .Include(a => a.Student)
                .ThenInclude(s => s!.Profile)
            .Include(a => a.Test)
            .Include(a => a.Attempt)
            .Where(a => a.TeacherId == teacherId && !a.IsDeleted)
            .OrderByDescending(a => a.AssignedAt)
            .ToListAsync();

        var assignedToMe = await _context.TeacherAssignments
            .Include(a => a.Teacher)
                .ThenInclude(t => t!.Profile)
            .Include(a => a.Test)
            .Include(a => a.Attempt)
            .Where(a => a.StudentId == teacherId && !a.IsDeleted)
            .OrderByDescending(a => a.AssignedAt)
            .ToListAsync();

        ViewBag.AssignedByMe = assignedByMe;
        ViewBag.AssignedToMe = assignedToMe;

        return View(assignedByMe);
    }

    // GET: Teacher/TestAssignments/Assign
    public async Task<IActionResult> Assign(int? testId = null)
    {
        var currentUserId = GetCurrentUserId();

        // Get all students and peer teachers/admins
        var users = await _context.Users
            .Include(u => u.Profile)
            .Where(u => (u.Role == "student" || u.Role == "teacher" || u.Role == "admin") && u.Status == "active" && u.UserId != currentUserId)
            .OrderBy(u => u.Role)
            .ThenBy(u => u.Profile != null ? u.Profile.FullName : u.Email)
            .ToListAsync();

        // Get all tests (public or teacher created)
        var tests = await _context.Tests
            .Where(t => !t.IsDeleted && (t.IsPublic || t.CreatedBy == currentUserId))
            .ToListAsync();

        ViewBag.Students = users;
        ViewBag.Tests = tests;
        ViewBag.SelectedTestId = testId;

        return View();
    }

    // POST: Teacher/TestAssignments/Assign
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int testId, int studentId, string title, string? instructions, DateTime? deadline)
    {
        var currentUserId = GetCurrentUserId();
        var test = await _context.Tests.FindAsync(testId);
        if (test == null || test.IsDeleted)
        {
            TempData["Error"] = "Test not found.";
            return RedirectToAction(nameof(Index));
        }

        var recipient = await _context.Users.FindAsync(studentId);
        if (recipient == null || recipient.IsDeleted)
        {
            TempData["Error"] = "Recipient user not found.";
            return RedirectToAction(nameof(Index));
        }

        // Check if recipient is a peer teacher/admin
        bool isPeerReview = recipient.Role == "teacher" || recipient.Role == "admin";

        if (!isPeerReview)
        {
            // Regular student assignment: Test must be published
            if (test.IsTeacherCreated && test.Status != "published")
            {
                TempData["Error"] = "This test is not published yet and cannot be assigned to students.";
                return RedirectToAction(nameof(Index));
            }
        }
        else
        {
            // Peer teacher review assignment: Set test status to pending_review
            test.Status = "pending_review";
            _context.Tests.Update(test);
        }

        var assignment = new TeacherAssignment
        {
            TestId = testId,
            TeacherId = currentUserId,
            StudentId = studentId,
            Title = InputSanitizer.Sanitize(title),
            Instructions = InputSanitizer.Sanitize(instructions),
            Deadline = deadline,
            Status = "pending",
            AssignedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.TeacherAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            currentUserId,
            "create_assignment",
            "TeacherAssignment",
            assignment.AssignmentId,
            null,
            JsonSerializer.Serialize(new { assignment.AssignmentId, assignment.TestId, assignment.StudentId, assignment.Title, assignment.Status }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = isPeerReview ? "Test sent for peer review successfully." : "Test assigned to student successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Teacher/TestAssignments/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var assignment = await _context.TeacherAssignments.FindAsync(id);
        if (assignment == null || assignment.TeacherId != GetCurrentUserId()) return NotFound();

        return View(assignment);
    }

    // POST: Teacher/TestAssignments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DateTime? deadline, string? instructions)
    {
        var assignment = await _context.TeacherAssignments.FindAsync(id);
        if (assignment == null || assignment.TeacherId != GetCurrentUserId()) return NotFound();

        assignment.Deadline = deadline;
        assignment.Instructions = InputSanitizer.Sanitize(instructions);

        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "edit_assignment",
            "TeacherAssignment",
            assignment.AssignmentId,
            null,
            JsonSerializer.Serialize(new { assignment.AssignmentId, assignment.Deadline, assignment.Instructions }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Assignment updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Teacher/TestAssignments/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var assignment = await _context.TeacherAssignments.FindAsync(id);
        if (assignment == null || assignment.TeacherId != GetCurrentUserId()) return NotFound();

        assignment.IsDeleted = true;
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "revoke_assignment",
            "TeacherAssignment",
            assignment.AssignmentId,
            null,
            JsonSerializer.Serialize(new { assignment.AssignmentId, assignment.TestId, assignment.StudentId, assignment.Status }),
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Assignment revoked successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Teacher/TestAssignments/CreateTest
    public IActionResult CreateTest()
    {
        return View();
    }

    // POST: Teacher/TestAssignments/CreateTest
    [HttpPost]
    public async Task<IActionResult> CreateTest([FromBody] CreateTestModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Title))
        {
            return Json(new { success = false, message = "Title is required and test data must be valid." });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var test = new Test
            {
                Title = InputSanitizer.Sanitize(model.Title),
                Difficulty = model.Difficulty,
                DurationMinutes = model.DurationMinutes,
                IsTeacherCreated = true,
                CreatedBy = GetCurrentUserId(),
                IsPublic = false,
                Status = "draft",
                IsDeleted = false
            };

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            foreach (var secModel in model.Sections)
            {
                var section = new TestSection
                {
                    TestId = test.TestId,
                    SkillType = secModel.SkillType.ToLower(),
                    AudioUrl = InputSanitizer.Sanitize(secModel.AudioUrl)
                };

                _context.TestSections.Add(section);
                await _context.SaveChangesAsync();

                foreach (var qModel in secModel.Questions)
                {
                    var question = new Question
                    {
                        SectionId = section.SectionId,
                        QuestionText = InputSanitizer.Sanitize(qModel.QuestionText),
                        Difficulty = qModel.Difficulty
                    };

                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrWhiteSpace(qModel.CorrectAnswer))
                    {
                        var answer = new Answer
                        {
                            QuestionId = question.QuestionId,
                            CorrectAnswer = InputSanitizer.Sanitize(qModel.CorrectAnswer)
                        };
                        _context.Answers.Add(answer);
                    }
                }
            }

            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "create_test",
                "Test",
                test.TestId,
                null,
                System.Text.Json.JsonSerializer.Serialize(new { test.TestId, test.Title, test.Difficulty, test.DurationMinutes }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            await transaction.CommitAsync();

            return Json(new { success = true, testId = test.TestId, message = "Test created successfully!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateTest ERROR] Original Exception: {ex}");
            try
            {
                await transaction.RollbackAsync();
            }
            catch (Exception rollbackEx)
            {
                Console.WriteLine($"[CreateTest ERROR] Rollback Exception: {rollbackEx.Message}");
            }
            return Json(new { success = false, message = ex.Message });
        }
    }

    public class CreateQuestionModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public int Difficulty { get; set; } = 6;
    }

    public class CreateSectionModel
    {
        public string SkillType { get; set; } = string.Empty;
        public string? AudioUrl { get; set; }
        public List<CreateQuestionModel> Questions { get; set; } = new();
    }

    public class CreateTestModel
    {
        public string Title { get; set; } = string.Empty;
        public int Difficulty { get; set; } = 6;
        public int DurationMinutes { get; set; } = 60;
        public List<CreateSectionModel> Sections { get; set; } = new();
    }

    // GET: Teacher/TestAssignments/Review/5
    public async Task<IActionResult> Review(int id)
    {
        var teacherId = GetCurrentUserId();
        var assignment = await _context.TeacherAssignments
            .Include(a => a.Teacher)
                .ThenInclude(t => t!.Profile)
            .Include(a => a.Student)
                .ThenInclude(s => s!.Profile)
            .Include(a => a.Test)
                .ThenInclude(t => t!.Sections)
                    .ThenInclude(s => s.Questions)
                        .ThenInclude(q => q.Answer)
            .FirstOrDefaultAsync(a => a.AssignmentId == id && !a.IsDeleted);

        if (assignment == null) return NotFound();

        // Security check: Only the assigned peer reviewer (student_id in assignment) can review it
        if (assignment.StudentId != teacherId)
        {
            return Forbid();
        }

        return View(assignment);
    }

    // POST: Teacher/TestAssignments/Approve
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? reviewerNote)
    {
        var teacherId = GetCurrentUserId();
        var assignment = await _context.TeacherAssignments
            .Include(a => a.Test)
            .FirstOrDefaultAsync(a => a.AssignmentId == id && !a.IsDeleted);

        if (assignment == null) return NotFound();

        if (assignment.StudentId != teacherId) return Forbid();

        var test = assignment.Test;
        if (test == null) return NotFound();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Update test
            test.Status = "published";
            test.IsPublic = true;
            test.ReviewedBy = teacherId;
            test.ReviewedAt = DateTime.UtcNow;
            test.ReviewerNote = InputSanitizer.Sanitize(reviewerNote);
            _context.Tests.Update(test);

            // Complete assignment
            assignment.Status = "completed";
            assignment.CompletedAt = DateTime.UtcNow;
            _context.TeacherAssignments.Update(assignment);

            await _context.SaveChangesAsync();

            // Create notification for the creator
            var notification = new Notification
            {
                UserId = assignment.TeacherId,
                Title = "Test Approved & Published",
                Message = $"Your test '{test.Title}' has been approved and published by {User.Identity?.Name}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Type = "test_approved"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                teacherId,
                "approve_test",
                "Test",
                test.TestId,
                null,
                JsonSerializer.Serialize(new { test.TestId, test.Title, test.Status, reviewerNote }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            await transaction.CommitAsync();

            TempData["Success"] = "Test approved and published successfully!";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Error approving test: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Teacher/TestAssignments/Reject
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string? reviewerNote)
    {
        var teacherId = GetCurrentUserId();
        var assignment = await _context.TeacherAssignments
            .Include(a => a.Test)
            .FirstOrDefaultAsync(a => a.AssignmentId == id && !a.IsDeleted);

        if (assignment == null) return NotFound();

        if (assignment.StudentId != teacherId) return Forbid();

        var test = assignment.Test;
        if (test == null) return NotFound();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Update test
            test.Status = "rejected";
            test.IsPublic = false;
            test.ReviewedBy = teacherId;
            test.ReviewedAt = DateTime.UtcNow;
            test.ReviewerNote = InputSanitizer.Sanitize(reviewerNote);
            _context.Tests.Update(test);

            // Complete assignment
            assignment.Status = "completed";
            assignment.CompletedAt = DateTime.UtcNow;
            _context.TeacherAssignments.Update(assignment);

            await _context.SaveChangesAsync();

            // Create notification for the creator
            var notification = new Notification
            {
                UserId = assignment.TeacherId,
                Title = "Test Rejected",
                Message = $"Your test '{test.Title}' has been rejected by {User.Identity?.Name}. Feedback: '{reviewerNote}'",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Type = "test_rejected"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                teacherId,
                "reject_test",
                "Test",
                test.TestId,
                null,
                JsonSerializer.Serialize(new { test.TestId, test.Title, test.Status, reviewerNote }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            await transaction.CommitAsync();

            TempData["Success"] = "Test review submitted: Rejected.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Error rejecting test: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
