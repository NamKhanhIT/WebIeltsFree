using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebIeltsFree.Models;
using WebIeltsFree.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext context, IConfiguration config, IEmailService emailService, ILogger<AuthController> logger)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new AuthResponse 
            { 
                Success = false, 
                Message = errors.FirstOrDefault() ?? "Invalid request"
            });
        }

        // Check if email exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new AuthResponse { Success = false, Message = "Email already registered" });

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "student",
            Status = "active",
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create profile
        var profile = new UserProfile
        {
            UserId = user.UserId,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);

        var goal = new UserGoal
        {
            UserId = user.UserId,
            TargetBand = request.TargetBand, 
            CurrentBand = null
        };
        _context.UserGoals.Add(goal);

        // Generate verification token
        var tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(tokenBytes);
        var verifyToken = Convert.ToBase64String(tokenBytes);
        var safeToken = Uri.EscapeDataString(verifyToken);

        var verifySession = new UserSession
        {
            UserId = user.UserId,
            RefreshToken = "verify_" + verifyToken,
            TokenFamily = "email_verification",
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow
        };
        _context.UserSessions.Add(verifySession);

        await _context.SaveChangesAsync();

        // Send verification email
        var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:5000";
        var verifyUrl = $"{frontendUrl}/verify-email?token={safeToken}";
        var body = $"<h1>Welcome to IELTS Platform</h1><p>Please click <a href='{verifyUrl}'>here</a> to verify your email address.</p><p>This link will expire in 24 hours.</p>";
        await _emailService.SendGenericEmailAsync(user.Email, "Verify Your Email", body);

        // Generate token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Registration successful. Please check your email to verify your account.",
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiry()),
            User = MapToUserDto(user, profile, goal)
        });
    }

    /// <summary>
    /// Verify email address
    /// </summary>
    [HttpGet("verify-email")]
    public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest(ApiResponse<bool>.Fail("Token is required"));

        var verifyToken = "verify_" + token;
        var session = await _context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == verifyToken && s.TokenFamily == "email_verification");

        if (session == null || session.Revoked)
            return BadRequest(ApiResponse<bool>.Fail("Invalid or already used verification token"));

        if (session.ExpiresAt < DateTime.UtcNow)
            return BadRequest(ApiResponse<bool>.Fail("Verification token has expired"));

        var user = session.User;
        if (user == null)
            return BadRequest(ApiResponse<bool>.Fail("User not found"));

        user.EmailVerified = true;
        session.Revoked = true;
        session.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Ok(true, "Email verified successfully"));
    }

    /// <summary>
    /// Login with email and password (called by site.js via AJAX)
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Email and password are required."
            });
        }

        // 1. Find user by email
        var sanitizedEmail = InputSanitizer.StripHtmlTags(request.Email.Trim());
        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Goal)
            .FirstOrDefaultAsync(u => u.Email == sanitizedEmail && !u.IsDeleted);

        // 2. Verify password (crash-safe for corrupt hashes)
        bool isPasswordValid = false;
        if (user != null && !string.IsNullOrEmpty(user.PasswordHash))
        {
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password.Trim(), user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "BCrypt error for {Email} — hash may be corrupt", sanitizedEmail);
            }
        }

        if (user == null || !isPasswordValid)
        {
            return Unauthorized(new AuthResponse { Success = false, Message = "Invalid email or password" });
        }

        if (user.Status != "active")
        {
            return Unauthorized(new AuthResponse { Success = false, Message = $"Account is {user.Status}" });
        }

        // 3. Build claims and issue cookie
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Profile?.FullName ?? user.Username ?? "User"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("FullName", user.Profile?.FullName ?? user.Username ?? "User"),
            new("Avatar", user.Profile?.AvatarUrl ?? "")
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        _logger.LogInformation("✅ API LOGIN OK: {Email} as {Role}", user.Email, user.Role);

        // 4. Return JSON for site.js
        var redirectUrl = user.Role.ToLower() switch
        {
            "admin"   => "/Admin/Users/Index",
            "teacher" => "/Teacher/Home/Index",
            _         => "/Home/Dashboard"
        };

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Token = "cookie-auth",  // No JWT needed — cookie is the auth
            User = MapToUserDto(user, user.Profile, user.Goal),
            RedirectUrl = redirectUrl
        });
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var session = await _context.UserSessions
            .Include(s => s.User)
            .ThenInclude(u => u!.Profile)
            .Include(s => s.User)
            .ThenInclude(u => u!.Goal)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken);

        if (session == null)
            return Unauthorized(new AuthResponse { Success = false, Message = "Invalid refresh token" });

        if (session.User == null || session.User.Status == "suspended")
            return Unauthorized(new AuthResponse { Success = false, Message = "User account is suspended or invalid" });

        if (session.Revoked)
        {
            // Token reuse detected! Revoke the entire token family
            var familySessions = await _context.UserSessions
                .Where(s => s.TokenFamily == session.TokenFamily && !s.Revoked)
                .ToListAsync();
            
            foreach(var s in familySessions)
            {
                s.Revoked = true;
                s.RevokedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            return Unauthorized(new AuthResponse { Success = false, Message = "Session revoked due to token reuse" });
        }

        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            session.Revoked = true;
            session.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Unauthorized(new AuthResponse { Success = false, Message = "Refresh token expired" });
        }

        // Rotate token
        session.Revoked = true;
        session.RevokedAt = DateTime.UtcNow;

        var tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(tokenBytes);
        var newRefreshToken = Convert.ToBase64String(tokenBytes);

        var newSession = new UserSession
        {
            UserId = session.UserId,
            RefreshToken = newRefreshToken,
            TokenFamily = session.TokenFamily,
            DeviceInfo = Request.Headers["User-Agent"].ToString(),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSessions.Add(newSession);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(session.User);

        return Ok(new AuthResponse
        {
            Success = true,
            Token = token,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiry()),
            User = MapToUserDto(session.User, session.User.Profile, session.User.Goal)
        });
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] RefreshTokenRequest request)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken);
            
        if (session != null)
        {
            session.Revoked = true;
            session.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        
        return Ok(ApiResponse<bool>.Ok(true, "Logged out successfully"));
    }

    /// <summary>
    /// Change password (requires authentication)
    /// </summary>
    [HttpPost("change-password")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound(ApiResponse<bool>.Fail("User not found"));

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest(ApiResponse<bool>.Fail("Current password is incorrect"));

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Ok(true, "Password changed successfully"));
    }

    /// <summary>
    /// Forgot password - sends reset link
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var sanitizedEmail = InputSanitizer.StripHtmlTags(request.Email);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == sanitizedEmail);

        if (user != null)
        {
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var resetToken = Convert.ToBase64String(tokenBytes);
            
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordExpires = DateTime.UtcNow.AddHours(24);
            
            await _context.SaveChangesAsync();

            var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:5000";
            var resetUrl = $"{frontendUrl}/reset-password";
            
            await _emailService.SendPasswordResetEmailAsync(user.Email, Uri.EscapeDataString(resetToken), resetUrl);
        }

        // Always return success to prevent email enumeration
        return Ok(ApiResponse<bool>.Ok(true, "If the email exists, a reset link has been sent"));
    }

    /// <summary>
    /// Reset password using token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<bool>.Fail(errors.FirstOrDefault() ?? "Invalid request data"));
        }

        var unescapedToken = Uri.UnescapeDataString(request.Token);
        var sanitizedToken = InputSanitizer.StripHtmlTags(unescapedToken);
        
        var user = await _context.Users.FirstOrDefaultAsync(u => 
            u.ResetPasswordToken == sanitizedToken && 
            u.ResetPasswordExpires > DateTime.UtcNow);

        if (user == null)
            return BadRequest(ApiResponse<bool>.Fail("Invalid or expired password reset token"));

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        
        // Invalidate token
        user.ResetPasswordToken = null;
        user.ResetPasswordExpires = null;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<bool>.Ok(true, "Password has been successfully reset"));
    }

    #region Private Methods

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32Characters!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "WebIeltsFree",
            audience: _config["Jwt:Audience"] ?? "WebIeltsFreeUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetTokenExpiry()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

#if DEBUG
    /// <summary>DEV-ONLY: Generate a JWT for a fake user without a DB entity.</summary>
    private string GenerateJwtTokenDev(string email, string role, int fakeId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32Characters!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, fakeId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "WebIeltsFree",
            audience: _config["Jwt:Audience"] ?? "WebIeltsFreeUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
#endif

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Allow expired tokens
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32Characters!"))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private int GetTokenExpiry() => int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private static UserDto MapToUserDto(User user, UserProfile? profile, UserGoal? goal)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            Profile = profile != null ? new UserProfileDto
            {
                FullName = profile.FullName,
                AvatarUrl = profile.AvatarUrl,
                Country = profile.Country,
                Timezone = profile.Timezone,
                PreferredLanguage = profile.PreferredLanguage
            } : null,
            Goal = goal != null ? new UserGoalDto
            {
                CurrentBand = goal.CurrentBand,
                TargetBand = goal.TargetBand,
                ExamDate = goal.ExamDate,
                StudyHoursPerDay = goal.StudyHoursPerDay
            } : null
        };
    }

    #endregion
}
