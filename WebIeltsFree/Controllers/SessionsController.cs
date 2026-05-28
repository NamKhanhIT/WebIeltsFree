using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// User Sessions API - Manage refresh tokens and sessions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(AppDbContext context, ILogger<SessionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all active sessions for current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserSessionDto>>>> GetSessions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var sessions = await _context.UserSessions
                .Where(s => s.UserId == userId && !s.Revoked && s.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var dtos = sessions.Select(MapToDto).ToList();
            return Ok(ApiResponse<List<UserSessionDto>>.Ok(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching sessions: {ex.Message}");
            return StatusCode(500, ApiResponse<List<UserSessionDto>>.Error("Failed to fetch sessions"));
        }
    }

    /// <summary>
    /// Get current session info
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<UserSessionDto>>> GetCurrentSession()
    {
        try
        {
            var userId = GetCurrentUserId();
            var jti = User.FindFirst("jti")?.Value ?? "";

            // Try to find matching session by user ID (simplified - in production, match by token)
            var session = await _context.UserSessions
                .Where(s => s.UserId == userId && !s.Revoked && s.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (session == null)
                return NotFound(ApiResponse<UserSessionDto>.Error("Session not found"));

            return Ok(ApiResponse<UserSessionDto>.Ok(MapToDto(session)));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching current session: {ex.Message}");
            return StatusCode(500, ApiResponse<UserSessionDto>.Error("Failed to fetch current session"));
        }
    }

    /// <summary>
    /// Revoke specific session (logout from that device)
    /// </summary>
    [HttpPost("{sessionId}/revoke")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeSession(int sessionId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId);

            if (session == null)
                return NotFound(ApiResponse<bool>.Error("Session not found"));

            session.Revoked = true;
            session.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Session {sessionId} revoked for user {userId}");
            return Ok(ApiResponse<bool>.Ok(true, "Session revoked"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error revoking session: {ex.Message}");
            return StatusCode(500, ApiResponse<bool>.Error("Failed to revoke session"));
        }
    }

    /// <summary>
    /// Revoke all sessions for current user (logout all devices)
    /// </summary>
    [HttpPost("revoke-all")]
    public async Task<ActionResult<ApiResponse<int>>> RevokeAllSessions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var activeSessions = await _context.UserSessions
                .Where(s => s.UserId == userId && !s.Revoked)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.Revoked = true;
                session.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Revoked {activeSessions.Count} sessions for user {userId}");
            return Ok(ApiResponse<int>.Ok(activeSessions.Count, $"Revoked {activeSessions.Count} sessions"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error revoking all sessions: {ex.Message}");
            return StatusCode(500, ApiResponse<int>.Error("Failed to revoke all sessions"));
        }
    }

    /// <summary>
    /// Get session count
    /// </summary>
    [HttpGet("count")]
    public async Task<ActionResult<ApiResponse<int>>> GetSessionCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _context.UserSessions
                .CountAsync(s => s.UserId == userId && !s.Revoked && s.ExpiresAt > DateTime.UtcNow);

            return Ok(ApiResponse<int>.Ok(count));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error counting sessions: {ex.Message}");
            return StatusCode(500, ApiResponse<int>.Error("Failed to count sessions"));
        }
    }

    /// <summary>
    /// Create new session (admin only - for session management)
    /// </summary>
    [HttpPost("create")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<UserSessionDto>>> CreateSession(
        [FromQuery] int userId,
        [FromBody] CreateSessionRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(ApiResponse<UserSessionDto>.Error("User not found"));

            var session = new UserSession
            {
                UserId = userId,
                RefreshToken = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(request.ExpiryDays ?? 7),
                DeviceInfo = request.DeviceInfo,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Session created for user {userId}");
            return Ok(ApiResponse<UserSessionDto>.Ok(MapToDto(session), "Session created"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating session: {ex.Message}");
            return StatusCode(500, ApiResponse<UserSessionDto>.Error("Failed to create session"));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private UserSessionDto MapToDto(UserSession session)
    {
        return new UserSessionDto
        {
            SessionId = session.SessionId,
            ExpiresAt = session.ExpiresAt,
            Revoked = session.Revoked,
            RevokedAt = session.RevokedAt,
            DeviceInfo = session.DeviceInfo,
            IpAddress = session.IpAddress,
            CreatedAt = session.CreatedAt
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    #endregion
}
