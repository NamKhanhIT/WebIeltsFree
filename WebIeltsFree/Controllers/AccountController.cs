using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AppDbContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ── GET /Account/Login ──
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return Redirect(GetRedirectUrl(User.FindFirst(ClaimTypes.Role)?.Value ?? ""));
        return View();
    }

    // ── POST /Account/Login  — THE SIMPLEST POSSIBLE LOGIN ──
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Email and password are required.";
            return View();
        }

        // 1. Find user in DB
        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email.Trim() && !u.IsDeleted);

        // 2. Verify password (with BCrypt crash protection)
        bool ok = false;
        if (user != null && !string.IsNullOrEmpty(user.PasswordHash))
        {
            try { ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash); }
            catch { ok = false; }
        }

        if (user == null || !ok)
        {
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        if (user.Status != "active")
        {
            ViewBag.Error = $"Account is {user.Status}.";
            return View();
        }

        // 3. Build claims — bare minimum for [Authorize] and User.Claims
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

        // 4. Sign in — no extra properties, let Program.cs defaults handle expiry
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        _logger.LogInformation("✅ LOGIN OK: {Email} as {Role}", user.Email, user.Role);

        // 5. Redirect by role
        return Redirect(GetRedirectUrl(user.Role));
    }

    // ── GET /Account/Logout ──
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // ── POST /Account/Logout ──
    [HttpPost]
    public async Task<IActionResult> LogoutPost()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // ── Role → URL ──
    private static string GetRedirectUrl(string role)
    {
        return role.ToLower() switch
        {
            "admin"   => "/Admin/Users/Index",
            "teacher" => "/Teacher/Home/Index",
            _         => "/Home/Index"
        };
    }
}
