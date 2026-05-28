using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Utilities;

namespace WebIeltsFree.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class UsersController : Controller
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Index(string searchString, string roleFilter, string statusFilter, int pageNumber = 1)
    {
        int pageSize = 10;
        
        var query = _context.Users.Include(u => u.Profile).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(u => u.Email.Contains(searchString) || 
                                     (u.Profile != null && u.Profile.FullName != null && u.Profile.FullName.Contains(searchString)));
        }

        if (!string.IsNullOrEmpty(roleFilter))
        {
            query = query.Where(u => u.Role == roleFilter);
        }

        if (!string.IsNullOrEmpty(statusFilter))
        {
            query = query.Where(u => u.Status == statusFilter);
        }

        var totalItems = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.SearchString = searchString;
        ViewBag.RoleFilter = roleFilter;
        ViewBag.StatusFilter = statusFilter;

        return View(users);
    }

    // GET: Admin/Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string email, string password, string role, string status)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email and Password are required.");
            return View();
        }

        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError("", "Email already exists.");
            return View();
        }

        var validRoles = new[] { "student", "teacher", "admin", "moderator" };
        if (!validRoles.Contains(role)) role = "student";

        var validStatuses = new[] { "active", "banned", "inactive" };
        if (!validStatuses.Contains(status)) status = "active";

        var user = new User
        {
            Email = InputSanitizer.Sanitize(email),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            Status = status,
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var profile = new UserProfile
        {
            UserId = user.UserId,
            FullName = InputSanitizer.Sanitize(email.Split('@')[0]),
            CreatedAt = DateTime.UtcNow
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "create_user", 
            "User", 
            user.UserId, 
            null, 
            $"{{ \"email\": \"{user.Email}\", \"role\": \"{user.Role}\" }}",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "User created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Users/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();

        return View(user);
    }

    // POST: Admin/Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string role, string status)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (id == GetCurrentUserId())
        {
            TempData["Warning"] = "You cannot edit your own account.";
            return RedirectToAction(nameof(Index));
        }

        var oldValues = $"{{ \"role\": \"{user.Role}\", \"status\": \"{user.Status}\" }}";

        var validRoles = new[] { "student", "teacher", "admin", "moderator" };
        if (validRoles.Contains(role)) user.Role = role;

        var validStatuses = new[] { "active", "banned", "inactive" };
        if (validStatuses.Contains(status)) user.Status = status;

        await _context.SaveChangesAsync();
        
        var newValues = $"{{ \"role\": \"{user.Role}\", \"status\": \"{user.Status}\" }}";
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "edit_user", 
            "User", 
            user.UserId, 
            oldValues, 
            newValues,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Users/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();

        return View(user);
    }

    // POST: Admin/Users/ToggleBan/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBan(int id)
    {
        if (id == GetCurrentUserId())
        {
            TempData["Warning"] = "You cannot ban your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var oldStatus = user.Status;
        user.Status = user.Status == "banned" ? "active" : "banned";
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "toggle_ban", 
            "User", 
            user.UserId, 
            $"{{ \"status\": \"{oldStatus}\" }}", 
            $"{{ \"status\": \"{user.Status}\" }}",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = $"User {(user.Status == "banned" ? "banned" : "unbanned")} successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Users/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();

        return View(user);
    }

    // POST: Admin/Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (id == GetCurrentUserId())
        {
            TempData["Warning"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return RedirectToAction(nameof(Index));

        // Due to lack of cascade deletes, we must clean up some children first depending on DbContext setup
        // But the prompt states "This is the ONLY place .Remove() is permitted in the entire codebase"
        
        await AuditHelper.LogAsync(
            _context, 
            GetCurrentUserId(), 
            "hard_delete_user", 
            "User", 
            user.UserId, 
            $"{{ \"email\": \"{user.Email}\" }}", 
            null,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        if (user.Profile != null)
        {
            _context.UserProfiles.Remove(user.Profile);
        }

        var goals = await _context.UserGoals.Where(g => g.UserId == id).ToListAsync();
        _context.UserGoals.RemoveRange(goals);

        var sessions = await _context.UserSessions.Where(s => s.UserId == id).ToListAsync();
        _context.UserSessions.RemoveRange(sessions);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "User permanently deleted.";
        return RedirectToAction(nameof(Index));
    }
}
