using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IEmailNotificationService _emailService;

        public ProfileController(IEmailNotificationService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public IActionResult SendContactForm([FromBody] ContactFormDto contactForm)
        {
            if (contactForm == null || string.IsNullOrWhiteSpace(contactForm.Name) ||
                string.IsNullOrWhiteSpace(contactForm.Email) || string.IsNullOrWhiteSpace(contactForm.Message))
            {
                return BadRequest("All fields are required.");
            }

            // Prepare the email content
            string subject = $"New Contact Form Submission from {contactForm.Name}";
            string body = $@"
                <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                margin: 0;
                                padding: 20px;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                background-color: #ffffff;
                                border-radius: 8px;
                                padding: 20px;
                                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                            }}
                            h3 {{
                                color: #333333;
                            }}
                            p {{
                                font-size: 14px;
                                color: #666666;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #999999;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h3>Contact Form Details</h3>
                            <p><strong>Name:</strong> {contactForm.Name}</p>
                            <p><strong>Email:</strong> {contactForm.Email}</p>
                            <p><strong>Message:</strong><br>{contactForm.Message}</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent from the ShopSilo contact form.</p>
                        </div>
                    </body>
                </html>
            ";

            // Send the email
            _emailService.SendEmail("shopsilo.ecomofficial@gmail.com", subject, body); // Replace with your official email

            return Ok("Your message has been sent successfully.");
        }
    }
}
