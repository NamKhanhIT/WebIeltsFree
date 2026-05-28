using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Teacher Dashboard API — teacher/admin only
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "teacher,admin")]
public class TeacherController : ControllerBase
{
    private readonly AppDbContext _context;

    public TeacherController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Dashboard summary stats for the teacher landing page
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<TeacherDashboardDto>>> GetDashboard()
    {
        var weekAgo = DateTime.UtcNow.AddDays(-7);

        var pendingDisputes = await _context.GradeDisputes
            .CountAsync(d => d.Status == "pending" || d.Status == "under_review");

        var openConversations = await _context.Conversations
            .CountAsync(c => c.Status == "open" || c.Status == "active");

        var totalStudents = await _context.Users
            .CountAsync(u => u.Role == "student" && !u.IsDeleted);

        var resolvedThisWeek = await _context.GradeDisputes
            .CountAsync(d => d.Status == "resolved" && d.ResolvedAt >= weekAgo);

        return Ok(ApiResponse<TeacherDashboardDto>.Ok(new TeacherDashboardDto
        {
            PendingDisputes = pendingDisputes,
            OpenConversations = openConversations,
            TotalStudents = totalStudents,
            ResolvedDisputesThisWeek = resolvedThisWeek
        }));
    }

    /// <summary>
    /// Paginated list of all students with basic academic info
    /// </summary>
    [HttpGet("students")]
    public async Task<ActionResult<ApiResponse<List<TeacherStudentListDto>>>> GetStudents(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = _context.Users
            .Where(u => u.Role == "student" && !u.IsDeleted)
            .Include(u => u.Profile)
            .Include(u => u.Goal);

        var total = await query.CountAsync();
        var students = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new TeacherStudentListDto
            {
                UserId = u.UserId,
                FullName = u.Profile != null ? u.Profile.FullName : null,
                Email = u.Email,
                CurrentBand = u.Goal != null ? u.Goal.CurrentBand : null,
                TargetBand = u.Goal != null ? u.Goal.TargetBand : null,
                LastActiveAt = u.LastLoginAt,
                TotalPracticeAttempts = u.PracticeAttempts.Count
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TeacherStudentListDto>>.OkWithPaging(students, total, pageNumber, pageSize));
    }

    /// <summary>
    /// Full academic profile for one student — teacher sees everything
    /// </summary>
    [HttpGet("students/{id}/detail")]
    public async Task<ActionResult<ApiResponse<TeacherStudentDetailDto>>> GetStudentDetail(int id)
    {
        var student = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Goal)
            .FirstOrDefaultAsync(u => u.UserId == id && u.Role == "student");

        if (student == null)
            return NotFound(ApiResponse<TeacherStudentDetailDto>.Fail("Student not found"));

        var latestWriting = await _context.WritingSubmissions
            .Where(w => w.UserId == id)
            .OrderByDescending(w => w.CreatedAt)
            .Take(5)
            .Select(w => new WritingResultDto
            {
                SubmissionId = w.SubmissionId,
                Prompt = w.Prompt,
                BandScore = (float?)w.BandScore,
                TaskAchievementScore = (float?)w.TaScore,
                CoherenceCohesionScore = (float?)w.CcScore,
                LexicalResourceScore = (float?)w.LrScore,
                GrammarAccuracyScore = (float?)w.GraScore,
                AiFeedback = w.AiFeedback,
                WordCount = w.WordCount,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync();

        var latestSpeaking = await _context.SpeakingSessions
            .Where(s => s.UserId == id)
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .Select(s => new SpeakingResultDto
            {
                SessionId = s.SessionId,
                Topic = s.Topic,
                FluencyScore = s.FluencyScore,
                PronunciationScore = s.PronunciationScore,
                GrammarScore = s.GrammarScore,
                OverallBand = (s.FluencyScore + s.PronunciationScore + s.GrammarScore) / 3,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        var testCount = await _context.UserTestAttempts.CountAsync(t => t.UserId == id);
        var practiceCount = await _context.UserPracticeAttempts.CountAsync(p => p.UserId == id);

        var activeDisputes = await _context.GradeDisputes
            .Where(d => d.UserId == id && (d.Status == "pending" || d.Status == "under_review"))
            .Select(d => new DisputeDto
            {
                DisputeId = d.DisputeId,
                UserId = d.UserId,
                SubmissionType = d.SubmissionType,
                SubmissionId = d.SubmissionId,
                Reason = d.Reason,
                Status = d.Status,
                OriginalScore = (float)d.OriginalScore,
                RevisedScore = (float?)d.RevisedScore,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();

        var detail = new TeacherStudentDetailDto
        {
            Profile = new UserDto
            {
                UserId = student.UserId,
                Email = student.Email,
                Role = student.Role,
                Status = student.Status,
                CreatedAt = student.CreatedAt,
                Profile = student.Profile != null ? new UserProfileDto
                {
                    FullName = student.Profile.FullName,
                    AvatarUrl = student.Profile.AvatarUrl,
                    Country = student.Profile.Country
                } : null
            },
            Goals = student.Goal != null ? new UserGoalDto
            {
                CurrentBand = student.Goal.CurrentBand,
                TargetBand = student.Goal.TargetBand,
                ExamDate = student.Goal.ExamDate,
                StudyHoursPerDay = student.Goal.StudyHoursPerDay
            } : null,
            LatestWritingSubmissions = latestWriting,
            LatestSpeakingSessions = latestSpeaking,
            TotalTestAttempts = testCount,
            TotalPracticeAttempts = practiceCount,
            ActiveDisputes = activeDisputes
        };

        return Ok(ApiResponse<TeacherStudentDetailDto>.Ok(detail));
    }
}
