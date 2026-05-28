using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Goal)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound(ApiResponse<UserDto>.Fail("User not found"));

        return Ok(ApiResponse<UserDto>.Ok(MapToUserDto(user)));
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Goal)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound(ApiResponse<UserDto>.Fail("User not found"));

        if (user.Profile == null)
        {
            user.Profile = new UserProfile { UserId = userId };
            _context.UserProfiles.Add(user.Profile);
        }

        if (request.FullName != null) user.Profile.FullName = request.FullName;
        if (request.AvatarUrl != null) user.Profile.AvatarUrl = request.AvatarUrl;
        if (request.Country != null) user.Profile.Country = request.Country;
        if (request.Timezone != null) user.Profile.Timezone = request.Timezone;
        if (request.PreferredLanguage != null) user.Profile.PreferredLanguage = request.PreferredLanguage;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<UserDto>.Ok(MapToUserDto(user), "Profile updated"));
    }

    /// <summary>
    /// Get user's learning goals (includes configurable target band)
    /// </summary>
    [HttpGet("goals")]
    public async Task<ActionResult<ApiResponse<UserGoalDto>>> GetGoals()
    {
        var userId = GetCurrentUserId();
        var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);

        if (goal == null)
            return Ok(ApiResponse<UserGoalDto>.Ok(new UserGoalDto { TargetBand = 6.5f }));

        return Ok(ApiResponse<UserGoalDto>.Ok(new UserGoalDto
        {
            CurrentBand = goal.CurrentBand,
            TargetBand = goal.TargetBand,
            ExamDate = goal.ExamDate,
            StudyHoursPerDay = goal.StudyHoursPerDay
        }));
    }

    [HttpPost("goals")]
    public async Task<ActionResult<ApiResponse<UserGoalDto>>> SetGoals([FromBody] SetGoalRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserGoalDto>.Fail("Invalid request"));

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(ApiResponse<UserGoalDto>.Fail("Please log in to set goals"));

            var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);

            if (goal == null)
            {
                goal = new UserGoal { UserId = userId };
                _context.UserGoals.Add(goal);
            }

            // User can set any target band from 0 to 9 (e.g., 5.0, 5.5, 6.0, 6.5, 7.0, 7.5, 8.0)
            goal.TargetBand = request.TargetBand;
            goal.ExamDate = request.ExamDate;
            goal.StudyHoursPerDay = request.StudyHoursPerDay;
            
            // Try to set learning reason - may fail if column doesn't exist
            try
            {
                if (!string.IsNullOrEmpty(request.LearningReason))
                    goal.LearningReason = request.LearningReason;
            }
            catch { /* Column may not exist yet */ }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<UserGoalDto>.Ok(new UserGoalDto
            {
                CurrentBand = goal.CurrentBand,
                TargetBand = goal.TargetBand,
                ExamDate = goal.ExamDate,
                StudyHoursPerDay = goal.StudyHoursPerDay
            }, "Goals updated"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SetGoals error: {ex.Message}");
            return StatusCode(500, ApiResponse<UserGoalDto>.Fail("Failed to save goals. Please try again."));
        }
    }

    /// <summary>
    /// Get user progress summary
    /// </summary>
    [HttpGet("progress")]
    public async Task<ActionResult<ApiResponse<UserProgressSummary>>> GetProgress()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Ok(ApiResponse<UserProgressSummary>.Ok(new UserProgressSummary
                {
                    CurrentBand = null,
                    TargetBand = 6.5f,
                    ProgressPercent = 0,
                    TotalLessonsCompleted = 0,
                    TotalTestsTaken = 0,
                    CurrentStreak = 0,
                    TotalXP = 0
                }));
            }

            var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
            var lessonsCompleted = await _context.UserLearningProgress
                .CountAsync(p => p.UserId == userId && p.CompletionPercent >= 100);
            var testsTaken = await _context.UserTestAttempts
                .CountAsync(a => a.UserId == userId && a.FinishedAt != null);

            // Get skill analysis if exists - wrapped in try/catch for schema safety
            AISkillAnalysis? skillAnalysis = null;
            try
            {
                skillAnalysis = await _context.AISkillAnalyses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.AnalyzedAt)
                    .FirstOrDefaultAsync();
            }
            catch { /* Schema may not be updated */ }

            var targetBand = goal?.TargetBand ?? 6.5f;
            var currentBand = goal?.CurrentBand ?? skillAnalysis?.OverallBand;

            // Calculate progress percentage towards target band
            float progressPercent = 0;
            if (currentBand.HasValue && targetBand > 0)
            {
                progressPercent = Math.Min(100, (currentBand.Value / targetBand) * 100);
            }

            return Ok(ApiResponse<UserProgressSummary>.Ok(new UserProgressSummary
            {
                CurrentBand = currentBand,
                TargetBand = targetBand,
                ProgressPercent = progressPercent,
                TotalLessonsCompleted = lessonsCompleted,
                TotalTestsTaken = testsTaken,
                CurrentStreak = 0, // TODO: Calculate actual streak
                TotalXP = lessonsCompleted * 50 + testsTaken * 100, // Simple XP calculation
                Skills = skillAnalysis != null ? new SkillBreakdown
                {
                    Reading = skillAnalysis.ReadingScore,
                    Listening = skillAnalysis.ListeningScore,
                    Writing = skillAnalysis.WritingScore,
                    Speaking = skillAnalysis.SpeakingScore
                } : null
            }));
        }
        catch
        {
            return Ok(ApiResponse<UserProgressSummary>.Ok(new UserProgressSummary
            {
                CurrentBand = null,
                TargetBand = 6.5f,
                ProgressPercent = 0,
                TotalLessonsCompleted = 0,
                TotalTestsTaken = 0,
                CurrentStreak = 0,
                TotalXP = 0
            }));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    #endregion

    /// <summary>
    /// Get user's onboarding status
    /// </summary>
    [HttpGet("onboarding-status")]
    public async Task<ActionResult<ApiResponse<OnboardingStatusDto>>> GetOnboardingStatus()
    {
        var userId = GetCurrentUserId();
        
        // Check if user has taken placement test
        var hasPlacement = await _context.UserTestAttempts
            .AnyAsync(a => a.UserId == userId && a.Test.Title != null && a.Test.Title.Contains("Placement"));
        
        // Check if user has set goals (exam date, study hours, etc.)
        var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
        var hasGoals = goal != null && goal.ExamDate.HasValue && goal.StudyHoursPerDay > 0;
        
        // Check if user has AI roadmap generated
        var hasRoadmap = await _context.AIRoadmaps.AnyAsync(r => r.UserId == userId);
        
        // Check if user has completed onboarding tutorial
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        var hasCompletedTutorial = user?.HasCompletedOnboarding ?? false;
        
        return Ok(ApiResponse<OnboardingStatusDto>.Ok(new OnboardingStatusDto
        {
            HasCompletedTutorial = hasCompletedTutorial,
            HasPlacement = hasPlacement,
            HasGoals = hasGoals,
            HasRoadmap = hasRoadmap,
            CurrentBand = goal?.CurrentBand,
            TargetBand = goal?.TargetBand ?? 6.5f
        }));
    }

    /// <summary>
    /// Mark onboarding tutorial as completed
    /// </summary>
    [HttpPost("complete-tutorial")]
    public async Task<ActionResult<ApiResponse<bool>>> CompleteTutorial()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user == null)
            return NotFound(ApiResponse<bool>.Fail("User not found"));
        
        user.HasCompletedOnboarding = true;
        await _context.SaveChangesAsync();
        
        return Ok(ApiResponse<bool>.Ok(true));
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            Profile = user.Profile != null ? new UserProfileDto
            {
                FullName = user.Profile.FullName,
                AvatarUrl = user.Profile.AvatarUrl,
                Country = user.Profile.Country,
                Timezone = user.Profile.Timezone,
                PreferredLanguage = user.Profile.PreferredLanguage
            } : null,
            Goal = user.Goal != null ? new UserGoalDto
            {
                CurrentBand = user.Goal.CurrentBand,
                TargetBand = user.Goal.TargetBand,
                ExamDate = user.Goal.ExamDate,
                StudyHoursPerDay = user.Goal.StudyHoursPerDay
            } : null
        };
    }

    /// <summary>
    /// Admin-only: Change a user's role (e.g., promote to teacher)
    /// </summary>
    [HttpPatch("/api/admin/users/{id}/role")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeUserRole(int id, [FromBody] ChangeRoleRequest request)
    {
        var allowedRoles = new[] { "student", "teacher", "admin", "moderator" };
        if (string.IsNullOrEmpty(request.Role) || !allowedRoles.Contains(request.Role.ToLower()))
            return BadRequest(ApiResponse<bool>.Fail("Invalid role. Allowed: student, teacher, admin, moderator"));

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(ApiResponse<bool>.Fail("User not found"));

        var oldRole = user.Role;
        user.Role = request.Role.ToLower();
        user.UpdatedAt = DateTime.UtcNow;

        var adminId = GetCurrentUserId();
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(_context, adminId, "change_user_role", "User", id,
            $"{{\"role\":\"{oldRole}\"}}", $"{{\"role\":\"{user.Role}\"}}");

        return Ok(ApiResponse<bool>.Ok(true, $"User role changed from {oldRole} to {user.Role}"));
    }
}

