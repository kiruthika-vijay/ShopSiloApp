using ShopSiloAppFSD.Interfaces;

namespace ShopSiloAppFSD.Services
{
    public class TestAuditLogConfiguration : IAuditLogConfiguration
    {
        public bool IsAuditLogEnabled => false; // Disable logging in unit tests
    }
}
