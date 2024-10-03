using ShopSiloAppFSD.Interfaces;

namespace ShopSiloAppFSD.Services
{
    public class AuditLogConfiguration : IAuditLogConfiguration
    {
        public bool IsAuditLogEnabled => true; // Always enabled in production
    }
}
