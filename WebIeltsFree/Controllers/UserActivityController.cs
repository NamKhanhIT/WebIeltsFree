using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// User Activity & Streaks API - Track daily learning activity and streaks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserActivityController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserActivityController> _logger;

    public UserActivityController(AppDbContext context, ILogger<UserActivityController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get today's activity
    /// </summary>
    [HttpGet("today")]
    public async Task<ActionResult<ApiResponse<UserActivityDto>>> GetTodayActivity()
    {
        try
        {
            var userId = GetCurrentUserId();
            var today = DateTime.UtcNow.Date;

            var activity = await _context.UserDailyActivities
                .FirstOrDefaultAsync(a => a.UserId == userId && a.ActivityDate.Date == today);

            if (activity == null)
                return Ok(ApiResponse<UserActivityDto>.Ok(CreateEmptyActivity(userId)));

            return Ok(ApiResponse<UserActivityDto>.Ok(MapToDto(activity)));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching today's activity: {ex.Message}");
            return StatusCode(500, ApiResponse<UserActivityDto>.Error("Failed to fetch today's activity"));
        }
    }

    /// <summary>
    /// Get activity for a specific date range
    /// </summary>
    [HttpGet("range")]
    public async Task<ActionResult<ApiResponse<List<UserActivityDto>>>> GetActivityRange(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (fromDate > toDate)
                return BadRequest(ApiResponse<List<UserActivityDto>>.Error("FromDate must be before ToDate"));

            var activities = await _context.UserDailyActivities
                .Where(a => a.UserId == userId && 
                           a.ActivityDate >= fromDate && 
                           a.ActivityDate <= toDate)
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync();

            var dtos = activities.Select(MapToDto).ToList();
            return Ok(ApiResponse<List<UserActivityDto>>.Ok(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching activity range: {ex.Message}");
            return StatusCode(500, ApiResponse<List<UserActivityDto>>.Error("Failed to fetch activity range"));
        }
    }

    /// <summary>
    /// Get current streak information
    /// </summary>
    [HttpGet("streak")]
    public async Task<ActionResult<ApiResponse<StreakInfoDto>>> GetStreakInfo()
    {
        try
        {
            var userId = GetCurrentUserId();
            var today = DateTime.UtcNow.Date;

            // Get all activities ordered by date descending
            var recentActivities = await _context.UserDailyActivities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.ActivityDate)
                .Take(366) // Scan up to a year
                .ToListAsync();

            if (!recentActivities.Any())
                return Ok(ApiResponse<StreakInfoDto>.Ok(new StreakInfoDto { CurrentStreak = 0, LongestStreak = 0 }));

            int currentStreak = 0;
            int longestStreak = 0;
            int tempStreak = 0;

            var currentDate = today;
            var activities = recentActivities.OrderBy(a => a.ActivityDate).ToList();

            foreach (var activity in activities)
            {
                if (activity.ActivityDate.Date == currentDate.Date)
                {
                    tempStreak++;
                    longestStreak = Math.Max(longestStreak, tempStreak);
                }
                else if ((currentDate - activity.ActivityDate.Date).Days == 1)
                {
                    tempStreak++;
                    longestStreak = Math.Max(longestStreak, tempStreak);
                }
                else
                {
                    tempStreak = 1;
                }
                currentDate = activity.ActivityDate.Date;
            }

            // Check if user has activity today
            var hasActivityToday = recentActivities.Any(a => a.ActivityDate.Date == today);
            if (hasActivityToday)
            {
                currentStreak = tempStreak;
            }
            else
            {
                currentStreak = 0;
            }

            var streakInfo = new StreakInfoDto
            {
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                LastActivityDate = recentActivities.FirstOrDefault()?.ActivityDate
            };

            return Ok(ApiResponse<StreakInfoDto>.Ok(streakInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calculating streak: {ex.Message}");
            return StatusCode(500, ApiResponse<StreakInfoDto>.Error("Failed to calculate streak"));
        }
    }

    /// <summary>
    /// Log activity for current user
    /// </summary>
    [HttpPost("log")]
    public async Task<ActionResult<ApiResponse<UserActivityDto>>> LogActivity([FromBody] LogActivityRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var today = DateTime.UtcNow.Date;

            // Get or create today's activity
            var activity = await _context.UserDailyActivities
                .FirstOrDefaultAsync(a => a.UserId == userId && a.ActivityDate.Date == today);

            if (activity == null)
            {
                activity = new UserDailyActivity
                {
                    UserId = userId,
                    ActivityDate = today,
                    LessonsCompleted = 0,
                    QuestionsAnswered = 0,
                    WritingSubmissions = 0,
                    SpeakingSessions = 0,
                    MinutesSpent = 0,
                    XpEarned = 0,
                    StreakDay = 1
                };
                _context.UserDailyActivities.Add(activity);
            }

            // Update activity counts
            if (request.LessonsCompleted > 0)
                activity.LessonsCompleted += request.LessonsCompleted;
            if (request.QuestionsAnswered > 0)
                activity.QuestionsAnswered += request.QuestionsAnswered;
            if (request.WritingSubmissions > 0)
                activity.WritingSubmissions += request.WritingSubmissions;
            if (request.SpeakingSessions > 0)
                activity.SpeakingSessions += request.SpeakingSessions;
            if (request.MinutesSpent > 0)
                activity.MinutesSpent += request.MinutesSpent;
            if (request.XpEarned > 0)
                activity.XpEarned += request.XpEarned;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Activity logged for user {userId}");
            return Ok(ApiResponse<UserActivityDto>.Ok(MapToDto(activity), "Activity recorded"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error logging activity: {ex.Message}");
            return StatusCode(500, ApiResponse<UserActivityDto>.Error("Failed to log activity"));
        }
    }

    /// <summary>
    /// Get weekly summary
    /// </summary>
    [HttpGet("week-summary")]
    public async Task<ActionResult<ApiResponse<WeeklySummaryDto>>> GetWeeklySummary()
    {
        try
        {
            var userId = GetCurrentUserId();
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var weekActivities = await _context.UserDailyActivities
                .Where(a => a.UserId == userId && 
                           a.ActivityDate >= weekStart && 
                           a.ActivityDate <= today)
                .ToListAsync();

            var summary = new WeeklySummaryDto
            {
                TotalLessons = weekActivities.Sum(a => a.LessonsCompleted),
                TotalQuestions = weekActivities.Sum(a => a.QuestionsAnswered),
                TotalWriting = weekActivities.Sum(a => a.WritingSubmissions),
                TotalSpeaking = weekActivities.Sum(a => a.SpeakingSessions),
                TotalMinutes = weekActivities.Sum(a => a.MinutesSpent),
                TotalXp = weekActivities.Sum(a => a.XpEarned),
                ActiveDays = weekActivities.Count,
                AverageMinutesPerDay = weekActivities.Any() ? 
                    (int)(weekActivities.Sum(a => a.MinutesSpent) / (double)weekActivities.Count) : 0
            };

            return Ok(ApiResponse<WeeklySummaryDto>.Ok(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching weekly summary: {ex.Message}");
            return StatusCode(500, ApiResponse<WeeklySummaryDto>.Error("Failed to fetch weekly summary"));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private UserActivityDto MapToDto(UserDailyActivity activity)
    {
        return new UserActivityDto
        {
            ActivityId = activity.ActivityId,
            ActivityDate = activity.ActivityDate,
            LessonsCompleted = activity.LessonsCompleted,
            QuestionsAnswered = activity.QuestionsAnswered,
            WritingSubmissions = activity.WritingSubmissions,
            SpeakingSessions = activity.SpeakingSessions,
            MinutesSpent = activity.MinutesSpent,
            XpEarned = activity.XpEarned,
            StreakDay = activity.StreakDay
        };
    }

    private UserActivityDto CreateEmptyActivity(int userId)
    {
        return new UserActivityDto
        {
            ActivityId = 0,
            ActivityDate = DateTime.UtcNow,
            LessonsCompleted = 0,
            QuestionsAnswered = 0,
            WritingSubmissions = 0,
            SpeakingSessions = 0,
            MinutesSpent = 0,
            XpEarned = 0,
            StreakDay = 0
        };
    }

    #endregion
}
