using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;

namespace WebIeltsFree.Middleware;

public class DevMagicLoginMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DevMagicLoginMiddleware> _logger;

    public DevMagicLoginMiddleware(RequestDelegate next, ILogger<DevMagicLoginMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var role = context.Request.Query["dev"].ToString()
                   ?? context.Request.Headers["X-Dev-Login"].ToString();

        if (!string.IsNullOrWhiteSpace(role) &&
            (role.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
             role.Equals("teacher", StringComparison.OrdinalIgnoreCase)))
        {
            var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
            
            // Find a real user with this role to avoid Foreign Key constraint violations
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Role == role.ToLower() && !u.IsDeleted);

            var userIdStr = user != null ? user.UserId.ToString() : (role == "admin" ? "-1" : "-2");
            var email = user?.Email ?? (role == "admin" ? "admin@test.com" : "teacher@test.com");
            var fullName = user != null ? "Dev " + role : (role == "admin" ? "Dev Admin" : "Dev Teacher");

            _logger.LogWarning("[DEV MAGIC LOGIN] Injecting {Role} claims for request: {Path}. Using UserId: {UserId}", role, context.Request.Path, userIdStr);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userIdStr),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role.ToLower())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24),
                    AllowRefresh = true
                });

            context.User = principal;
        }

        await _next(context);
    }
}
