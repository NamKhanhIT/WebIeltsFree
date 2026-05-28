using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Web;

namespace WebIeltsFree.Middleware;

/// Rate limiting middleware to prevent abuse
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, RateLimitInfo> _clients = new();
    
    // Configuration
    private readonly int _requestsPerMinute = 60;
    private readonly int _requestsPerHour = 500;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var now = DateTime.UtcNow;

        if (!_clients.TryGetValue(clientId, out var info))
        {
            info = new RateLimitInfo();
            _clients[clientId] = info;
        }

        lock (info)
        {
            info.MinuteRequests.RemoveAll(t => (now - t).TotalMinutes > 1);
            info.HourRequests.RemoveAll(t => (now - t).TotalHours > 1);
        }

        // Check rate limits
        if (info.MinuteRequests.Count >= _requestsPerMinute)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId} (per minute)", clientId);
            context.Response.StatusCode = 429;
            context.Response.Headers["Retry-After"] = "60";
            await context.Response.WriteAsJsonAsync(new { error = "Too many requests. Please wait 1 minute." });
            return;
        }

        if (info.HourRequests.Count >= _requestsPerHour)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId} (per hour)", clientId);
            context.Response.StatusCode = 429;
            context.Response.Headers["Retry-After"] = "3600";
            await context.Response.WriteAsJsonAsync(new { error = "Too many requests. Please wait 1 hour." });
            return;
        }

        // Record request
        lock (info)
        {
            info.MinuteRequests.Add(now);
            info.HourRequests.Add(now);
        }

        await _next(context);
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID if authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                return $"user:{userId}";
        }

        // Fall back to IP address
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
            ip = forwarded.Split(',')[0].Trim();

        return $"ip:{ip}";
    }

    private class RateLimitInfo
    {
        public List<DateTime> MinuteRequests { get; } = new();
        public List<DateTime> HourRequests { get; } = new();
    }
}

/// <summary>
/// Security headers middleware
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Security headers
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(self), geolocation=()";

        // Content Security Policy (adjust as needed)
        context.Response.Headers["Content-Security-Policy"] = 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
            "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
            "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net; " +
            "img-src 'self' data: https:; " +
            "connect-src 'self' https://api.openai.com;";

        await _next(context);
    }
}

/// <summary>
/// Request logging middleware
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var path = context.Request.Path;
        var method = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            var elapsed = DateTime.UtcNow - startTime;
            var statusCode = context.Response.StatusCode;

            if (statusCode >= 400)
            {
                _logger.LogWarning("{Method} {Path} - {StatusCode} ({Elapsed}ms)",
                    method, path, statusCode, elapsed.TotalMilliseconds);
            }
            else if (elapsed.TotalMilliseconds > 1000) // Log slow requests
            {
                _logger.LogWarning("Slow request: {Method} {Path} - {StatusCode} ({Elapsed}ms)",
                    method, path, statusCode, elapsed.TotalMilliseconds);
            }
        }
    }
}

/// <summary>
/// Extension methods for middleware
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}

/// <summary>
/// Input sanitization utilities to prevent XSS and injection attacks
/// </summary>
public static class InputSanitizer
{
    private static readonly Regex HtmlTagsRegex = new(@"<[^>]*>", RegexOptions.Compiled);
    private static readonly Regex ScriptRegex = new(@"<script[^>]*>[\s\S]*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex SqlInjectionRegex = new(@"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|UNION|ALTER|CREATE|EXEC)\b)", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Sanitize user input by encoding HTML entities
    /// </summary>
    public static string SanitizeHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        return HttpUtility.HtmlEncode(input);
    }

    /// <summary>
    /// Strip all HTML tags from input
    /// </summary>
    public static string StripHtmlTags(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        // Remove script tags first (including content)
        input = ScriptRegex.Replace(input, string.Empty);
        // Remove all other HTML tags
        return HtmlTagsRegex.Replace(input, string.Empty).Trim();
    }

    /// <summary>
    /// Check if input contains potential SQL injection patterns
    /// </summary>
    public static bool ContainsSqlInjection(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;
        
        return SqlInjectionRegex.IsMatch(input);
    }

    /// <summary>
    /// Sanitize input for safe database queries (use parameterized queries primarily)
    /// </summary>
    public static string SanitizeForDatabase(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        // Replace single quotes to prevent SQL injection
        return input.Replace("'", "''").Trim();
    }

    /// <summary>
    /// Validate and sanitize email address
    /// </summary>
    public static string? SanitizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;
        
        email = email.Trim().ToLowerInvariant();
        
        // Basic email validation pattern
        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            return null;
        
        return email;
    }

    /// <summary>
    /// Truncate string to maximum length
    /// </summary>
    public static string Truncate(string? input, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        return input.Length > maxLength ? input[..maxLength] : input;
    }
}

/// <summary>
/// Security validation utilities
/// </summary>
public static class SecurityValidator
{
    /// <summary>
    /// Validate password strength
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidatePassword(string password)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return (false, errors);
        }
        
        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters");
        
        if (password.Length > 100)
            errors.Add("Password cannot exceed 100 characters");
        
        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");
        
        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");
        
        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one number");
        
        if (!password.Any(c => "@$!%*?&".Contains(c)))
            errors.Add("Password must contain at least one special character (@$!%*?&)");
        
        // Check for common weak passwords
        var commonPasswords = new[] { "password", "123456", "qwerty", "admin", "welcome" };
        if (commonPasswords.Any(p => password.ToLowerInvariant().Contains(p)))
            errors.Add("Password is too common and easily guessable");
        
        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Generate a secure random string
    /// </summary>
    public static string GenerateSecureToken(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[System.Security.Cryptography.RandomNumberGenerator.GetInt32(chars.Length)];
        }
        return new string(result);
    }

    /// <summary>
    /// Hash sensitive data for logging (prevents accidental exposure)
    /// </summary>
    public static string HashForLogging(string? sensitive)
    {
        if (string.IsNullOrEmpty(sensitive))
            return "[empty]";
        
        if (sensitive.Length <= 4)
            return "****";
        
        return $"{sensitive[..2]}***{sensitive[^2..]}";
    }
}
