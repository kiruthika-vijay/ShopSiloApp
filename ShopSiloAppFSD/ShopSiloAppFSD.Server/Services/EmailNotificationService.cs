using System.Net.Mail;
using System.Net;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ShopSiloAppFSD.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public EmailNotificationService()
        {

        }
        public void SendEmail(string recipientEmail, string subject, string body)
        {
            // Step 1: Define your SMTP server settings
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587) // Replace with your SMTP server
            {
                Credentials = new NetworkCredential("shopsilo.ecomofficial@gmail.com", "xzeczjosoifjarug"), // Replace with your email and password
                EnableSsl = true // Ensure SSL is enabled for security
            };

            // Step 2: Create the email message
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("shopsilo.ecomofficial@gmail.com"), // Replace with your "from" email address
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Set to true if the body contains HTML content
            };

            // Step 3: Add the recipient email
            mailMessage.To.Add(recipientEmail);

            try
            {
                // Step 4: Send the email
                smtpClient.Send(mailMessage);
                Console.WriteLine($"Email sent successfully to {recipientEmail}");
            }
            catch (Exception ex)
            {
                // Handle any email sending exceptions here
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
