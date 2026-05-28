using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Utilities;
using System.Text.Json;

namespace WebIeltsFree.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class CoursesController : Controller
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/Courses
    public async Task<IActionResult> Index(string searchString, string skillFilter, int pageNumber = 1)
    {
        int pageSize = 10;
        
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(c => c.Title.Contains(searchString));
        }

        if (!string.IsNullOrEmpty(skillFilter))
        {
            query = query.Where(c => c.SkillType == skillFilter);
        }

        var totalItems = await query.CountAsync();
        var courses = await query
            .Include(c => c.Modules)
            .OrderByDescending(c => c.CourseId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.SearchString = searchString;
        ViewBag.SkillFilter = skillFilter;

        return View(courses);
    }

    // GET: Admin/Courses/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Courses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            course.Slug = course.Title.ToLower().Replace(" ", "-").Replace(":", "");
            
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "create_course",
                "Course",
                course.CourseId,
                null,
                JsonSerializer.Serialize(new { course.Title, course.SkillType }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Course created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    // GET: Admin/Courses/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        return View(course);
    }

    // POST: Admin/Courses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.CourseId) return NotFound();

        if (ModelState.IsValid)
        {
            var existingCourse = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == id);
            var oldValues = JsonSerializer.Serialize(new { existingCourse?.Title, existingCourse?.SkillType, existingCourse?.IsPublished });
            
            course.Slug = course.Title.ToLower().Replace(" ", "-").Replace(":", "");
            
            _context.Update(course);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "edit_course",
                "Course",
                course.CourseId,
                oldValues,
                JsonSerializer.Serialize(new { course.Title, course.SkillType, course.IsPublished }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Course updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    // GET: Admin/Courses/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null) return NotFound();

        return View(course);
    }

    // POST: Admin/Courses/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        var oldValues = JsonSerializer.Serialize(new { course.Title });
        
        _context.Courses.Remove(course);
        
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "delete_course",
            "Course",
            course.CourseId,
            oldValues,
            null,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Course deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
