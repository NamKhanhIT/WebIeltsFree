using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Achievements & Gamification API - Track user achievements and badges
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AchievementsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AchievementsController> _logger;

    public AchievementsController(AppDbContext context, ILogger<AchievementsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all available achievements
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<AchievementDto>>>> GetAllAchievements()
    {
        try
        {
            var achievements = await _context.Achievements
                .Where(a => a.IsActive)
                .OrderBy(a => a.Points)
                .ToListAsync();

            var dtos = achievements.Select(MapToDto).ToList();
            return Ok(ApiResponse<List<AchievementDto>>.Ok(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching achievements: {ex.Message}");
            return StatusCode(500, ApiResponse<List<AchievementDto>>.Error("Failed to fetch achievements"));
        }
    }

    /// <summary>
    /// Get current user's earned achievements
    /// </summary>
    [HttpGet("earned")]
    public async Task<ActionResult<ApiResponse<List<UserAchievementDto>>>> GetEarnedAchievements()
    {
        try
        {
            var userId = GetCurrentUserId();
            var userAchievements = await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.UnlockedAt)
                .ToListAsync();

            var dtos = userAchievements.Select(ua => new UserAchievementDto
            {
                UserAchievementId = ua.UserAchievementId,
                Achievement = MapToDto(ua.Achievement),
                UnlockedAt = ua.UnlockedAt
            }).ToList();

            return Ok(ApiResponse<List<UserAchievementDto>>.Ok(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching earned achievements: {ex.Message}");
            return StatusCode(500, ApiResponse<List<UserAchievementDto>>.Error("Failed to fetch earned achievements"));
        }
    }

    /// <summary>
    /// Get user's total points from achievements
    /// </summary>
    [HttpGet("points")]
    public async Task<ActionResult<ApiResponse<int>>> GetTotalPoints()
    {
        try
        {
            var userId = GetCurrentUserId();
            var totalPoints = await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Where(ua => ua.UserId == userId)
                .SumAsync(ua => ua.Achievement.Points);

            return Ok(ApiResponse<int>.Ok(totalPoints));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calculating total points: {ex.Message}");
            return StatusCode(500, ApiResponse<int>.Error("Failed to calculate total points"));
        }
    }

    /// <summary>
    /// Check if user has earned specific achievement
    /// </summary>
    [HttpGet("check/{achievementCode}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckAchievement(string achievementCode)
    {
        try
        {
            var userId = GetCurrentUserId();
            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementCode == achievementCode && a.IsActive);

            if (achievement == null)
                return NotFound(ApiResponse<bool>.Error("Achievement not found"));

            var hasAchievement = await _context.UserAchievements
                .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.AchievementId);

            return Ok(ApiResponse<bool>.Ok(hasAchievement));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking achievement: {ex.Message}");
            return StatusCode(500, ApiResponse<bool>.Error("Failed to check achievement"));
        }
    }

    /// <summary>
    /// Get achievement statistics for user
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<AchievementStatsDto>>> GetAchievementStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var totalEarned = await _context.UserAchievements
                .CountAsync(ua => ua.UserId == userId);

            var totalPoints = await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Where(ua => ua.UserId == userId)
                .SumAsync(ua => ua.Achievement.Points);

            var totalAvailable = await _context.Achievements
                .CountAsync(a => a.IsActive);

            var progressPercent = totalAvailable > 0 ? (totalEarned * 100) / totalAvailable : 0;

            var stats = new AchievementStatsDto
            {
                TotalEarned = totalEarned,
                TotalAvailable = totalAvailable,
                TotalPoints = totalPoints,
                ProgressPercent = progressPercent
            };

            return Ok(ApiResponse<AchievementStatsDto>.Ok(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching achievement stats: {ex.Message}");
            return StatusCode(500, ApiResponse<AchievementStatsDto>.Error("Failed to fetch achievement stats"));
        }
    }

    /// <summary>
    /// Award achievement to user (Admin only)
    /// </summary>
    [HttpPost("award/{achievementCode}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> AwardAchievement(string achievementCode, [FromQuery] int userId)
    {
        try
        {
            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementCode == achievementCode && a.IsActive);

            if (achievement == null)
                return NotFound(ApiResponse<bool>.Error("Achievement not found"));

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(ApiResponse<bool>.Error("User not found"));

            var alreadyEarned = await _context.UserAchievements
                .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.AchievementId);

            if (alreadyEarned)
                return BadRequest(ApiResponse<bool>.Error("User already has this achievement"));

            var userAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievement.AchievementId,
                UnlockedAt = DateTime.UtcNow
            };

            _context.UserAchievements.Add(userAchievement);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Achievement {achievementCode} awarded to user {userId}");
            return Ok(ApiResponse<bool>.Ok(true, $"Achievement '{achievement.AchievementName}' awarded"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error awarding achievement: {ex.Message}");
            return StatusCode(500, ApiResponse<bool>.Error("Failed to award achievement"));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private AchievementDto MapToDto(Achievement achievement)
    {
        return new AchievementDto
        {
            AchievementId = achievement.AchievementId,
            AchievementCode = achievement.AchievementCode,
            AchievementName = achievement.AchievementName,
            Description = achievement.Description,
            IconUrl = achievement.IconUrl,
            Points = achievement.Points,
            BadgeColor = achievement.BadgeColor
        };
    }

    #endregion
}
