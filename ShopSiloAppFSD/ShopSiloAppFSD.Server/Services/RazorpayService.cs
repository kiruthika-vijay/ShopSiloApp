using Microsoft.AspNetCore.Authentication;
using Razorpay.Api;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Services;
using System.Security.Cryptography;
using System.Text;

public class RazorpayService : IRazorpayService
{
    private readonly RazorpayClient _client;
    private readonly string _apiSecret;

    public RazorpayService(string apiKey, string apiSecret)
    {
        _client = new RazorpayClient(apiKey, apiSecret);
        _apiSecret = apiSecret;
    }

    public async Task<Dictionary<string, string>> CreateOrderAsync(decimal amount, string receiptId)
    {
        var options = new Dictionary<string, object>
        {
            { "amount", (int)(amount * 100) }, // Amount in paise for INR
            { "currency", "INR" },
            { "receipt", receiptId }, // Pass the dynamic receipt ID
            { "payment_capture", 1 } // Set payment_capture to 1 to auto-capture
        };

        try
        {
            var order = await Task.Run(() => _client.Order.Create(options));
            var response = new Dictionary<string, string>
            {
                { "orderId", order["id"].ToString() },
                { "entity", order["entity"].ToString() },
                { "amount", order["amount"].ToString() },
                { "amount_paid", order["amount_paid"].ToString() },
                { "amount_due", order["amount_due"].ToString() },
                { "currency", order["currency"].ToString() },
                { "receipt", order["receipt"].ToString() },
                { "offer_id", order["offer_id"].ToString() },
                { "status", order["status"].ToString() },
                { "attempts", order["attempts"].ToString() },
                { "notes", order["notes"].ToString() },
                { "created_at", order["created_at"].ToString() }
            };

            return response;
        }
        catch (RazorpayException ex)
        {
            throw new Exception($"Razorpay Error: {ex.Message}", ex);
        }
    }

    public virtual async Task RefundPaymentAsync(string paymentId, decimal amount)
    {
        Dictionary<string, string> notes = new Dictionary<string, string>()
                {
                    { "note 1", "this is a refund note" },
                    { "note 2", "you can add max 15 notes" }
                };
        var refundRequest = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) }, // Refund amount in paise
                { "speed", "optimum" },
                { "notes", notes },
                { "receipt", "receiptId" }
            };
        try
        {
            Refund refund = _client.Payment.Fetch(paymentId).Refund(refundRequest);
            var response = new Dictionary<string, string>
            {
                { "refundId", refundRequest["id"].ToString() },
                { "entity", refundRequest["entity"].ToString() },
                { "amount", refundRequest["amount"].ToString() },
                { "currency", refundRequest["currency"].ToString() },
                { "receipt", refundRequest["receipt"].ToString() },
                { "status", refundRequest["status"].ToString() },
                { "speed_processed", refundRequest["speed_processed"].ToString() },
                { "speed_requested", refundRequest["speed_requested"].ToString() },
                { "created_at", refundRequest["created_at"].ToString() }
            };

            Console.WriteLine(response);
        }
        catch (Exception ex)
        {
            throw new Exception("Error processing refund with Razorpay.", ex);
        }
    }

    public void VerifyPayment(string paymentId, string orderId, string signature)
    {
        var body = $"{orderId}|{paymentId}";
        var generatedSignature = GenerateSignature(body);

        if (generatedSignature != signature)
        {
            throw new Exception("Payment verification failed.");
        }
    }

    public string GenerateSignature(string body)
    {
        using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret)))
        {
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(body));
            return Convert.ToBase64String(hash); // Razorpay uses Base64 encoded signature
        }
    }
}
