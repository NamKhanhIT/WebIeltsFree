using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Text.Json;

namespace WebIeltsFree.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // 1. Summary Cards Data
        ViewBag.TotalStudents = await _context.Users.CountAsync(u => u.Role == "student" && !u.IsDeleted);
        ViewBag.ActiveTeachers = await _context.Users.CountAsync(u => u.Role == "teacher" && u.Status == "active" && !u.IsDeleted);
        ViewBag.PendingSuggestions = await _context.RoadmapSuggestions.CountAsync(s => s.Status == "pending");
        ViewBag.ActiveTests = await _context.Tests.CountAsync(t => !t.IsDeleted);

        // 2. Chart.js Data: User Registration Trends (last 30 days)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        
        // Group by date, but EF Core has issues with client-side grouping on Date portion.
        // We fetch raw data and group in memory since it's just 30 days.
        var recentUsers = await _context.Users
            .Where(u => u.CreatedAt >= thirtyDaysAgo && !u.IsDeleted)
            .Select(u => new { u.CreatedAt })
            .ToListAsync();

        var registrationData = recentUsers
            .GroupBy(u => u.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key.ToString("MMM dd"),
                Count = g.Count()
            })
            .ToList();

        // Fill in missing days
        var dates = new List<string>();
        var counts = new List<int>();
        
        for (int i = 29; i >= 0; i--)
        {
            var day = DateTime.UtcNow.Date.AddDays(-i);
            var dayString = day.ToString("MMM dd");
            var match = registrationData.FirstOrDefault(d => d.Date == dayString);
            
            dates.Add(dayString);
            counts.Add(match?.Count ?? 0);
        }

        ViewBag.ChartLabels = JsonSerializer.Serialize(dates);
        ViewBag.ChartData = JsonSerializer.Serialize(counts);

        return View();
    }
}
