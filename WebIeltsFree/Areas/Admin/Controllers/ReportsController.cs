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
public class ReportsController : Controller
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/Reports
    public async Task<IActionResult> Index(string statusFilter, string typeFilter, int pageNumber = 1)
    {
        int pageSize = 15;
        
        var query = _context.Reports.Include(r => r.User).AsQueryable();

        if (!string.IsNullOrEmpty(statusFilter))
        {
            query = query.Where(r => r.Status == statusFilter);
        }

        if (!string.IsNullOrEmpty(typeFilter))
        {
            query = query.Where(r => r.ReportType == typeFilter);
        }

        var totalItems = await query.CountAsync();
        var reports = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.StatusFilter = statusFilter;
        ViewBag.TypeFilter = typeFilter;

        return View(reports);
    }

    // GET: Admin/Reports/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var report = await _context.Reports
            .Include(r => r.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(r => r.ReportId == id);

        if (report == null) return NotFound();

        return View(report);
    }

    // POST: Admin/Reports/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        var validStatuses = new[] { "open", "in_progress", "resolved", "closed" };
        if (validStatuses.Contains(status))
        {
            var oldStatus = report.Status;
            report.Status = status;
            report.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "update_report_status",
                "Report",
                report.ReportId,
                $"{{ \"status\": \"{oldStatus}\" }}",
                $"{{ \"status\": \"{status}\" }}",
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = $"Report status updated to {status.Replace('_', ' ')}.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Admin/Reports/UpdatePriority
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePriority(int id, string priority)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        var validPriorities = new[] { "low", "medium", "high", "critical" };
        if (validPriorities.Contains(priority))
        {
            var oldPriority = report.Priority;
            report.Priority = priority;
            report.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "update_report_priority",
                "Report",
                report.ReportId,
                $"{{ \"priority\": \"{oldPriority}\" }}",
                $"{{ \"priority\": \"{priority}\" }}",
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = $"Report priority updated to {priority}.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: Admin/Reports/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return NotFound();

        // Soft delete for reports
        var oldStatus = report.Status;
        report.Status = "closed";
        report.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        await AuditHelper.LogAsync(
            _context,
            GetCurrentUserId(),
            "close_report",
            "Report",
            report.ReportId,
            $"{{ \"status\": \"{oldStatus}\" }}",
            $"{{ \"status\": \"closed\" }}",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString()
        );

        TempData["Success"] = "Report has been closed.";
        return RedirectToAction(nameof(Index));
    }
}
