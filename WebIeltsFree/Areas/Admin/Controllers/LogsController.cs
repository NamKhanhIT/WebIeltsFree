using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;

namespace WebIeltsFree.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class LogsController : Controller
{
    private readonly AppDbContext _context;

    public LogsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Logs
    public async Task<IActionResult> Index(string actionFilter, string entityFilter, string dateFrom, string dateTo, int pageNumber = 1)
    {
        int pageSize = 20;
        var query = _context.AuditLogs.Include(l => l.TargetUser).AsQueryable();

        if (!string.IsNullOrEmpty(actionFilter))
        {
            query = query.Where(l => l.Action.Contains(actionFilter));
        }

        if (!string.IsNullOrEmpty(entityFilter))
        {
            query = query.Where(l => l.EntityType.Contains(entityFilter));
        }

        if (DateTime.TryParse(dateFrom, out DateTime fromDate))
        {
            query = query.Where(l => l.CreatedAt >= fromDate.ToUniversalTime());
        }

        if (DateTime.TryParse(dateTo, out DateTime toDate))
        {
            // Include the entire toDate
            toDate = toDate.AddDays(1).AddSeconds(-1);
            query = query.Where(l => l.CreatedAt <= toDate.ToUniversalTime());
        }

        var totalItems = await query.CountAsync();
        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.ActionFilter = actionFilter;
        ViewBag.EntityFilter = entityFilter;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        return View(logs);
    }
}
