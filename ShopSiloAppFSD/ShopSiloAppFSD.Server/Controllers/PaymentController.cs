using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.Server.Interfaces;
using ShopSiloAppFSD.Server.Models.Payment;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ShopSiloDBContext _context;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogConfiguration _auditLogConfig;
    private readonly IEmailNotificationService _emailService;
    private readonly string? _userId;
    private readonly User? _user;

    public PaymentController(ShopSiloDBContext context, IConfiguration configuration, IPaymentRepository paymentRepository, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig, IOrderRepository orderRepository, ISellerRepository sellerRepository, IEmailNotificationService emailService)
    {
        _context = context;
        _configuration = configuration;
        _paymentRepository = paymentRepository;
        _httpContextAccessor = httpContextAccessor;
        _auditLogConfig = auditLogConfig;
        _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(_userId))
        {
            _user = _context.Users.FirstOrDefault(u => u.Username == _userId);
        }
        _orderRepository = orderRepository;
        _sellerRepository = sellerRepository;
        _emailService = emailService;
    }

    private string RazorpayKey => _configuration["Razorpay:ApiKey"];
    private string RazorpaySecret => _configuration["Razorpay:ApiSecret"];

    [HttpPost("create-order")]
    public IActionResult CreateOrder([FromBody] RegistrationModel registration)
    {
        try
        {
            OrderModel order = new OrderModel
            {
                OrderAmount = registration.Amount,
                Currency = "INR",
                Payment_Capture = 1,
                Notes = new Dictionary<string, string>
                {
                    {"note 1", "first note while creating order" },
                    {"note 2", "you can add max 15 notes" }
                }
            };

            string orderId = CreateRazorpayOrder(order);

            RazorPayOptionsModel razorPayOptions = new RazorPayOptionsModel
            {
                Key = RazorpayKey,
                AmountInSubUnits = order.OrderAmountInSubUnits,
                Currency = order.Currency,
                Name = "ShopSilo Ecommerce",
                Description = "Ecommerce payment gateway",
                OrderId = orderId,
                ProfileName = registration.Name,
                ProfileContact = registration.Mobile,
                ProfileEmail = registration.Email
            };

            return Ok(razorPayOptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private string CreateRazorpayOrder(OrderModel order)
    {
        try
        {
            RazorpayClient client = new RazorpayClient(RazorpayKey, RazorpaySecret);
            var options = new Dictionary<string, object>
            {
                { "amount", order.OrderAmountInSubUnits },
                { "currency", order.Currency },
                { "payment_capture", order.Payment_Capture },
                { "notes", order.Notes }
            };

            var orderResponse = client.Order.Create(options);
            return orderResponse.Attributes["id"].ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create Razorpay order.", ex);
        }
    }

    [HttpPost("capture")]
    public IActionResult CapturePayment([FromBody] CapturePaymentModel capturePaymentModel)
    {
        try
        {
            RazorpayClient client = new RazorpayClient(RazorpayKey, RazorpaySecret);
            Razorpay.Api.Payment payment = client.Payment.Fetch(capturePaymentModel.PaymentId);

            var options = new Dictionary<string, object>
            {
                { "amount", payment.Attributes["amount"] },
                { "currency", payment.Attributes["currency"] }
            };

            var paymentCaptured = payment.Capture(options);
            return Ok(new { message = "Payment Captured!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("verify-payment")]
    public IActionResult VerifyPayment([FromBody] PaymentVerificationModel paymentVerification)
    {
        try
        {
            var validSignature = CompareSignatures(paymentVerification.OrderId, paymentVerification.PaymentId, paymentVerification.Signature);
            if (validSignature)
            {
                return Ok(new { message = "Payment successful" });
            }
            else
            {
                return BadRequest(new { error = "Invalid payment signature" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("signature")]
    private bool CompareSignatures(string orderId, string paymentId, string razorPaySignature)
    {
        var text = orderId + "|" + paymentId;
        var secret = RazorpaySecret;
        var generatedSignature = CalculateSHA256(text, secret);
        if (generatedSignature == razorPaySignature)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private string CalculateSHA256(string text, string secret)
    {
        byte[] baTextToBeHashed = Encoding.Default.GetBytes(text);
        byte[] baSalt = Encoding.Default.GetBytes(secret);

        using (var hasher = new HMACSHA256(baSalt))
        {
            byte[] baHashedText = hasher.ComputeHash(baTextToBeHashed);
            return string.Join("", baHashedText.Select(b => b.ToString("x2")));
        }
    }

    [HttpGet("userpayments")]
    public async Task<ActionResult<IEnumerable<PaymentTransactionDto>>> GetPaymentForLoggedUser()
    {
        try
        {
            var payments = await _paymentRepository.GetPaymentTransactionsByLoggedUserAsync();
            return Ok(payments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("complete-payment")]
    public async Task<IActionResult> CompletePayment(CompleteTransactionDto paymentTransactionDto)
    {
        // Validate incoming data
        if (paymentTransactionDto == null || paymentTransactionDto.Amount <= 0)
        {
            return BadRequest("Invalid payment data.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
        var userId = user.UserID;
        // Create payment transaction object
        var paymentTransaction = new PaymentTransaction
        {
            UserId = userId,
            RazorpayPaymentId = paymentTransactionDto.RazorpayPaymentId,
            RazorpayOrderId = paymentTransactionDto.RazorpayOrderId,
            Amount = paymentTransactionDto.Amount,
            PaymentStatus = paymentTransactionDto.PaymentStatus,
            Timestamp = DateTime.UtcNow
        };

        // Create order object
        var order = new ShopSiloAppFSD.Models.Order
        {
            UserID = user.UserID,
            OrderDate = DateTime.Now,
            TotalAmount = paymentTransactionDto.Amount,
            OrderStatus = OrderStatus.Pending, // Initial status
            ShippingAddressID = paymentTransactionDto.ShippingAddressID,
            BillingAddressID = paymentTransactionDto.BillingAddressID,
            SellerID = paymentTransactionDto.SellerID
        };

        // Save the payment transaction and create the order
        var (isSuccess, orderId) = await _paymentRepository.SavePaymentAndCreateOrderAsync(paymentTransaction, order, paymentTransactionDto.OrderItems);

        if (!isSuccess)
        {
            return StatusCode(500, "Failed to process payment and create order.");
        }

        // Retrieve the newly created order to get additional details like tracking number and seller information
        var orderDetails = await _orderRepository.GetOrderDetailByIdAsync(orderId); // Assuming you have a method to fetch order details

        if (orderDetails == null)
        {
            return NotFound("Order not found after creation.");
        }

        // Fetch seller details using the seller ID
        var sellerDetails = await _sellerRepository.GetSellerByIdAsync(orderDetails.SellerID);
        Console.WriteLine(sellerDetails);
        if (sellerDetails == null)
        {
            return NotFound(new { Message = "Seller not found." });
        }

        // Send confirmation email
        await _emailService.SendOrderConfirmationEmail(user.Email, orderDetails, sellerDetails);

        // Return success response with order details
        return Ok(new
        {
            Message = "Payment and order created successfully",
            OrderId = orderId,
            TrackingNumber = orderDetails.TrackingNumber, // Return tracking number
            Seller = new
            {
                SellerId = sellerDetails.SellerID,
                SellerName = sellerDetails.ContactPerson,
                CompanyName = sellerDetails.CompanyName,
                ContactNumber = sellerDetails.ContactNumber,
                Address = sellerDetails.Address
            }
        });
    }
}
