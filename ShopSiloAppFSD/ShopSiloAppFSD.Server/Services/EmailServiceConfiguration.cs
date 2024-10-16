using ShopSiloAppFSD.Interfaces;

namespace ShopSiloAppFSD.Services
{
    public class EmailServiceConfiguration : IEmailServiceConfiguration
    {
        public bool IsEmailServiceEnabled => true; // Always enabled in production
    }
}
