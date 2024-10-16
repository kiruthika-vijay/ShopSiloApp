using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IEmailNotificationService
    {
        Task SendOrderConfirmationEmail(string userEmail, OrderDetailDto orderDetails, Seller sellerDetails);
        void SendEmail(string recipientEmail, string subject, string body);
    }
}
