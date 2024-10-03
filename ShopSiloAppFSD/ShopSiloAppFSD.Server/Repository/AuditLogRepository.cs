using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using System.Security.Claims;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ShopSiloDBContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogConfiguration _auditLogConfig;

    public AuditLogRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _auditLogConfig = auditLogConfig;
    }

    // Logs a user action if audit logging is enabled
    public async Task LogUserActionAsync(string action)
    {
        if (!_auditLogConfig.IsAuditLogEnabled) return; // Skip logging if disabled in config

        if (string.IsNullOrEmpty(action))
        {
            throw new ArgumentException("Action cannot be null or empty.", nameof(action));
        }

        try
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID cannot be determined.");
            }
            var _user = _context.Users.FirstOrDefault(u => u.Username == userId);
            var auditLog = new AuditLog
            {
                UserId = _user.UserID,
                Action = action,
                Timestamp = DateTime.UtcNow
            };
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            throw new RepositoryException("Error logging user action.", ex);
        }
    }

    // Retrieves user activity logs
    public async Task<IEnumerable<AuditLog>> GetUserActivityLogsAsync(int userId)
    {
        try
        {
            return await _context.AuditLogs
                                 .Where(log => log.UserId == userId)
                                 .OrderByDescending(log => log.Timestamp)
                                 .ToListAsync();
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error retrieving user activity logs.", ex);
        }
    }
}
