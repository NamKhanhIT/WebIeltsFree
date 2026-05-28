using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

/// <summary>
/// Public-facing teacher directory for students and guests.
/// Students browse this to verify teacher credentials before trusting grading feedback.
/// NOT inside any Area — routes to /Teachers and /Teachers/Details/{id}.
/// </summary>
public class TeachersController : Controller
{
    private readonly AppDbContext _context;

    public TeachersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Teachers
    public async Task<IActionResult> Index()
    {
        var teachers = await _context.TeacherProfiles
            .Include(tp => tp.User)
            .ThenInclude(u => u!.Profile)
            .Where(tp => tp.IsPublic)
            .OrderByDescending(tp => tp.YearsExperience ?? 0)
            .ThenBy(tp => tp.User!.Profile!.FullName)
            .AsNoTracking()
            .ToListAsync();

        return View(teachers);
    }

    // GET: /Teachers/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var profile = await _context.TeacherProfiles
            .Include(tp => tp.User)
            .ThenInclude(u => u!.Profile)
            .Where(tp => tp.TeacherId == id && tp.IsPublic)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (profile == null)
        {
            return NotFound();
        }

        return View(profile);
    }
}
