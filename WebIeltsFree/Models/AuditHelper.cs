using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebIeltsFree.Models;

public static class AuditHelper
{
    /// <param name="context">The AppDbContext instance</param>
    /// <param name="userId">The user performing the action (teacher/admin). Nullified if user doesn't exist in DB.</param>
    /// <param name="action">Short description, e.g. "create_writing_prompt"</param>
    /// <param name="entityType">Entity type, e.g. "WritingPrompt"</param>
    /// <param name="entityId">PK of the affected entity</param>
    /// <param name="oldValues">JSON string of old values (null for creates)</param>
    /// <param name="newValues">JSON string of new values (null for deletes)</param>
    /// <param name="ipAddress">Optional IP address from HttpContext</param>
    /// <param name="userAgent">Optional User-Agent from HttpContext</param>
    /// <param name="logger">Optional logger for diagnostics</param>
    public static async Task LogAsync(
        AppDbContext context,
        int? userId,
        string action,
        string? entityType = null,
        int? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        ILogger? logger = null)
    {
        if (userId.HasValue)
        {
            bool userExists = await context.Users.AnyAsync(u => u.UserId == userId.Value && !u.IsDeleted);
            if (!userExists)
            {
                logger?.LogDebug("[AuditHelper] userId {UserId} not found in tb_users — logging with null user_id.", userId);
                userId = null;
            }
        }

        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValuesJson = oldValues,
            NewValuesJson = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Status = "success",
            CreatedAt = DateTime.UtcNow
        };

        context.AuditLogs.Add(log);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Audit logging should NEVER crash the main flow
            logger?.LogWarning(ex, "[AuditHelper] Failed to save audit log for action '{Action}'. Skipping.", action);
            context.Entry(log).State = EntityState.Detached; // detach to keep context clean
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "[AuditHelper] Unexpected error saving audit log for action '{Action}'.", action);
            context.Entry(log).State = EntityState.Detached;
        }
    }
}
