using ShopSiloAppFSD.Interfaces;

namespace ShopSiloAppFSD.Services
{
    public class TestEmailServiceConfiguration : IEmailServiceConfiguration
    {
        public bool IsEmailServiceEnabled => false; // Disable logging in unit tests
    }
}
