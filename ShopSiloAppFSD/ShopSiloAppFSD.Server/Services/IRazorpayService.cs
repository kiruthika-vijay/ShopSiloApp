using Razorpay.Api;

namespace ShopSiloAppFSD.Services
{
    public interface IRazorpayService
    {
        Task<Dictionary<string, string>> CreateOrderAsync(decimal amount, string receiptId);
        Task RefundPaymentAsync(string paymentId, decimal amount);
        void VerifyPayment(string paymentId, string orderId, string signature);
        string GenerateSignature(string body);
    }
}
