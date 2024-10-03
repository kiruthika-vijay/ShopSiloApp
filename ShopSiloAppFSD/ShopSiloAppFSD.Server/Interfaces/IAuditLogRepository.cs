using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IAuditLogRepository
    {
        Task LogUserActionAsync(string action);
        Task<IEnumerable<AuditLog>> GetUserActivityLogsAsync(int userId);
    }
}
