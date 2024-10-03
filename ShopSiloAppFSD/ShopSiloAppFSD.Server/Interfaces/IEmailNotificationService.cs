namespace ShopSiloAppFSD.Interfaces
{
    public interface IEmailNotificationService
    {
        void SendEmail(string recipientEmail, string subject, string body);
    }
}
