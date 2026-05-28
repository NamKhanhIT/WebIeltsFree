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
public class TeacherProfilesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<TeacherProfilesController> _logger;

    public TeacherProfilesController(AppDbContext context, ILogger<TeacherProfilesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // ──────────────────────────────────────────────
    //  GET: /Teacher/TeacherProfiles
    // ──────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var teacherId = GetCurrentUserId();

        var profile = await _context.TeacherProfiles
            .Include(p => p.Certificates)
            .FirstOrDefaultAsync(p => p.TeacherId == teacherId);

        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == teacherId);

        ViewBag.FullName = user?.Profile?.FullName ?? user?.Username ?? "User";
        ViewBag.AvatarUrl = user?.Profile?.AvatarUrl ?? "";

        // If no profile record exists yet, create a transient object for the form
        if (profile == null)
        {
            profile = new TeacherProfile
            {
                TeacherId = teacherId,
                IsPublic = true,
                Specialties = "",
                Bio = "",
                YearsExperience = null,
                CreatedAt = DateTime.UtcNow
            };
        }

        return View(profile);
    }

    // GET: Teacher/TeacherProfiles/Preview
    public async Task<IActionResult> Preview()
    {
        var teacherId = GetCurrentUserId();

        var profile = await _context.TeacherProfiles
            .Include(p => p.Certificates)
            .FirstOrDefaultAsync(p => p.TeacherId == teacherId);

        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == teacherId);

        if (profile == null)
        {
            TempData["Error"] = "Please create your profile first before previewing.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.FullName = user?.Profile?.FullName ?? user?.Username ?? "User";
        ViewBag.AvatarUrl = user?.Profile?.AvatarUrl ?? "";
        ViewBag.Email = user?.Email ?? "";

        return View(profile);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> Save(
        string? specialties,
        int? yearsExperience,
        string? bio,
        bool isPublic,
        string? fullName,
        string? currentPassword,
        string? newPassword,
        IFormFile? avatarFile,
        List<IFormFile>? certificateFiles)
    {
        var teacherId = GetCurrentUserId();
        if (teacherId == 0)
        {
            TempData["Error"] = "Authentication expired. Please log in again.";
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // ── Load entities for modification ──
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserId == teacherId);

            if (user == null)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var existingProfile = await _context.TeacherProfiles
                .FirstOrDefaultAsync(p => p.TeacherId == teacherId);

            // ── Snapshot old state for audit ──
            var oldState = new
            {
                FullName        = user.Profile?.FullName,
                AvatarUrl       = user.Profile?.AvatarUrl,
                Bio             = existingProfile?.Bio,
                Specialties     = existingProfile?.Specialties,
                YearsExperience = existingProfile?.YearsExperience,
                IsPublic        = existingProfile?.IsPublic
            };

            // ── 1. Password change (tb_users) ──
            if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword))
            {
                if (string.IsNullOrEmpty(user.PasswordHash) ||
                    !BCrypt.Net.BCrypt.Verify(currentPassword.Trim(), user.PasswordHash))
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Current password is incorrect.";
                    return RedirectToAction(nameof(Index));
                }

                if (newPassword.Trim().Length < 6)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "New password must be at least 6 characters.";
                    return RedirectToAction(nameof(Index));
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword.Trim());
                _logger.LogInformation("Teacher {TeacherId} changed their password.", teacherId);
            }

            // ── 2. User Profile: FullName & Avatar (tb_user_profiles) ──
            if (user.Profile == null)
            {
                user.Profile = new UserProfile { UserId = teacherId, CreatedAt = DateTime.UtcNow };
                _context.UserProfiles.Add(user.Profile);
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                user.Profile.FullName = InputSanitizer.Sanitize(fullName);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var ext = Path.GetExtension(avatarFile.FileName);
                var safeName = $"avatar_{teacherId}_{DateTime.UtcNow.Ticks}{ext}";
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(dir);

                var filePath = Path.Combine(dir, safeName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                user.Profile.AvatarUrl = $"/uploads/avatars/{safeName}";
            }

            // ── 3. Teacher Profile (tb_teacher_profiles) ──
            if (existingProfile != null)
            {
                existingProfile.Specialties     = InputSanitizer.Sanitize(specialties ?? "");
                existingProfile.YearsExperience = yearsExperience;
                existingProfile.Bio             = InputSanitizer.Sanitize(bio ?? "");
                existingProfile.IsPublic        = isPublic;
                existingProfile.UpdatedAt       = DateTime.UtcNow;
            }
            else
            {
                existingProfile = new TeacherProfile
                {
                    TeacherId       = teacherId,
                    Specialties     = InputSanitizer.Sanitize(specialties ?? ""),
                    YearsExperience = yearsExperience,
                    Bio             = InputSanitizer.Sanitize(bio ?? ""),
                    IsPublic        = isPublic,
                    CreatedAt       = DateTime.UtcNow
                };
                _context.TeacherProfiles.Add(existingProfile);
            }

            // ── 4. Certificate uploads (tb_teacher_certificates) ──
            if (certificateFiles != null && certificateFiles.Count > 0)
            {
                var certDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "certificates");
                Directory.CreateDirectory(certDir);

                foreach (var file in certificateFiles)
                {
                    if (file.Length <= 0) continue;

                    var ext = Path.GetExtension(file.FileName);
                    var safeName = $"cert_{teacherId}_{DateTime.UtcNow.Ticks}{ext}";
                    var filePath = Path.Combine(certDir, safeName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.TeacherCertificates.Add(new TeacherCertificate
                    {
                        TeacherId = teacherId,
                        Title     = Path.GetFileNameWithoutExtension(file.FileName),
                        ImageUrl  = $"/uploads/certificates/{safeName}",
                        IssueDate = DateTime.UtcNow
                    });
                }
            }

            // ── 5. Commit all changes atomically ──
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // ── 6. Audit log (separate SaveChanges — outside transaction) ──
            try
            {
                var newState = new
                {
                    FullName        = user.Profile?.FullName,
                    AvatarUrl       = user.Profile?.AvatarUrl,
                    Bio             = existingProfile.Bio,
                    Specialties     = existingProfile.Specialties,
                    YearsExperience = existingProfile.YearsExperience,
                    IsPublic        = existingProfile.IsPublic
                };

                await AuditHelper.LogAsync(
                    _context,
                    teacherId,
                    "edit_teacher_profile",
                    "tb_teacher_profiles",
                    teacherId,
                    JsonSerializer.Serialize(oldState),
                    JsonSerializer.Serialize(newState),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    HttpContext.Request.Headers["User-Agent"].ToString());
            }
            catch (Exception auditEx)
            {
                _logger.LogWarning(auditEx, "Audit log failed for teacher {TeacherId} profile save.", teacherId);
                // Audit failure should not block the user
            }

            TempData["Success"] = "Profile saved successfully.";
        }
        catch (Exception ex)
        {
            try { await transaction.RollbackAsync(); } catch { /* already rolled back */ }
            _logger.LogError(ex, "Failed to save teacher profile for TeacherId {TeacherId}.", teacherId);
            TempData["Error"] = $"Failed to save profile: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
