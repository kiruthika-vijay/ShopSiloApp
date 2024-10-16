using System.Net.Mail;
using System.Net;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Server.DTO;

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

        public async Task SendOrderConfirmationEmail(string userEmail, OrderDetailDto orderDetails, Seller sellerDetails)
        {
            var subject = "Order Confirmation - Your Order has been Placed";

            // HTML body for the email
            var body = $@"
                    <html>
                        <head>
                            <style>
                                body {{
                                    font-family: Arial, sans-serif;
                                    background-color: #f8f8f8;
                                    margin: 0;
                                    padding: 20px;
                                }}
                                .container {{
                                    background-color: #ffffff;
                                    padding: 20px;
                                    border-radius: 5px;
                                    box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                                }}
                                h2 {{
                                    color: #333333;
                                }}
                                .order-details {{
                                    margin-top: 20px;
                                    margin-bottom: 20px;
                                    border: 1px solid #dddddd;
                                    border-radius: 5px;
                                    padding: 15px;
                                }}
                                .seller-details {{
                                    margin-top: 20px;
                                    padding: 15px;
                                    background-color: #f2f2f2;
                                    border-radius: 5px;
                                }}
                                .footer {{
                                    margin-top: 20px;
                                    font-size: 14px;
                                    color: #777777;
                                }}
                                .thank-you {{
                                    font-weight: bold;
                                    color: #5cb85c;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Order Confirmation</h2>
                                <p>Dear Customer,</p>
                                <p class='thank-you'>Thank you for your order!</p>
                                <div class='order-details'>
                                    <h3>Order Details</h3>
                                    <p><strong>Order ID:</strong> {orderDetails.OrderID}</p>
                                    <p><strong>Total Amount:</strong> ₹{orderDetails.TotalAmount.ToString("N2")}</p> <!-- Manual Formatting -->
                                </div>
                                <div class='seller-details'>
                                    <h3>Seller Details</h3>
                                    <p><strong>Seller Name:</strong> {sellerDetails.ContactPerson}</p>
                                    <p><strong>Company Name:</strong> {sellerDetails.CompanyName}</p>
                                    <p><strong>Contact Number:</strong> {sellerDetails.ContactNumber}</p>
                                    <p><strong>Shipping Address ID:</strong> {orderDetails.ShippingAddressID}</p>
                                </div>
                                <p>Thank you for shopping with us!</p>
                                <p class='footer'>Best Regards,<br>Your ShopSilo Team</p>
                            </div>
                        </body>
                    </html>";

            // Use your preferred method to send the email (SMTP client, SendGrid, etc.)
            SendEmail(userEmail, subject, body);
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
