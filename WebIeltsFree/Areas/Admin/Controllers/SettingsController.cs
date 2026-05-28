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
public class SettingsController : Controller
{
    private readonly AppDbContext _context;

    public SettingsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/Settings
    public async Task<IActionResult> Index()
    {
        var settings = await _context.SystemSettings.ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue);

        // Ensure default keys exist in memory for the view if they aren't in DB yet
        var defaultKeys = new[] 
        { 
            "gemini_api_key", "gemini_model_version", 
            "site_name", "contact_email", "audio_storage_path",
            "teacher_review_enabled"
        };

        foreach (var key in defaultKeys)
        {
            if (!settings.ContainsKey(key))
            {
                settings[key] = "";
            }
        }

        return View(settings);
    }

    // POST: Admin/Settings/Save
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Dictionary<string, string> settings)
    {
        var oldSettings = await _context.SystemSettings.ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue);
        var changedSettings = new Dictionary<string, string>();

        foreach (var kvp in settings)
        {
            var key = kvp.Key;
            var value = kvp.Value ?? "";

            var existingSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
            
            if (existingSetting == null)
            {
                _context.SystemSettings.Add(new SystemSetting
                {
                    SettingKey = key,
                    SettingValue = value
                });
                changedSettings[key] = value;
            }
            else if (existingSetting.SettingValue != value)
            {
                existingSetting.SettingValue = value;
                changedSettings[key] = value;
            }
        }

        if (changedSettings.Any())
        {
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "update_system_settings",
                "SystemSetting",
                0,
                JsonSerializer.Serialize(oldSettings.Where(k => changedSettings.ContainsKey(k.Key))),
                JsonSerializer.Serialize(changedSettings),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "System settings updated successfully.";
        }
        else
        {
            TempData["Info"] = "No changes were made to settings.";
        }

        return RedirectToAction(nameof(Index));
    }
}
