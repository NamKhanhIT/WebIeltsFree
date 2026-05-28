using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;

namespace WebIeltsFree.Services;

/// <summary>
/// Background service that periodically checks for overdue teacher assignments.
/// Runs every 30 minutes. When an assignment has status="pending" and its deadline
/// has passed, the service updates it to status="overdue".
///
/// Design decision: Uses IServiceScopeFactory to create a scoped DbContext per tick,
/// since BackgroundService is a singleton and DbContext is scoped.
/// </summary>
public class OverdueAssignmentService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueAssignmentService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

    public OverdueAssignmentService(
        IServiceScopeFactory scopeFactory,
        ILogger<OverdueAssignmentService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[OverdueAssignmentService] Started. Checking every {Interval} minutes.", _checkInterval.TotalMinutes);

        try
        {
            // Initial delay — let the app fully start before first check
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var updatedCount = await CheckAndMarkOverdueAsync(stoppingToken);

                    if (updatedCount > 0)
                    {
                        _logger.LogInformation("[OverdueAssignmentService] Marked {Count} assignment(s) as overdue.", updatedCount);
                    }
                    else
                    {
                        _logger.LogDebug("[OverdueAssignmentService] No overdue assignments found.");
                    }
                }
                catch (Exception ex)
                {
                    // Log but never crash the service — it will retry on next tick
                    _logger.LogError(ex, "[OverdueAssignmentService] Error during overdue check. Will retry in {Interval} minutes.", _checkInterval.TotalMinutes);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown — do not log as error
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[OverdueAssignmentService] Fatal error in background service.");
        }

        _logger.LogInformation("[OverdueAssignmentService] Stopped.");
    }

    /// <summary>
    /// Scans for pending assignments past their deadline and marks them overdue.
    /// Returns the number of assignments updated.
    /// </summary>
    private async Task<int> CheckAndMarkOverdueAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        // Find all pending assignments whose deadline has passed
        var overdueAssignments = await context.TeacherAssignments
            .Where(a => a.Status == "pending"
                     && !a.IsDeleted
                     && a.Deadline.HasValue
                     && a.Deadline.Value < now)
            .ToListAsync(cancellationToken);

        if (overdueAssignments.Count == 0)
        {
            return 0;
        }

        foreach (var assignment in overdueAssignments)
        {
            assignment.Status = "overdue";
        }

        await context.SaveChangesAsync(cancellationToken);

        // Log each update to audit trail (system action — no userId)
        foreach (var assignment in overdueAssignments)
        {
            try
            {
                await AuditHelper.LogAsync(
                    context,
                    userId: null,
                    action: "mark_assignment_overdue",
                    entityType: "TeacherAssignment",
                    entityId: assignment.AssignmentId,
                    oldValues: "{\"status\":\"pending\"}",
                    newValues: "{\"status\":\"overdue\"}",
                    ipAddress: "system",
                    userAgent: "OverdueAssignmentService"
                );
            }
            catch (Exception ex)
            {
                // Audit failure should not prevent the overdue marking
                _logger.LogWarning(ex, "[OverdueAssignmentService] Failed to log audit for assignment {AssignmentId}.", assignment.AssignmentId);
            }
        }

        return overdueAssignments.Count;
    }
}
