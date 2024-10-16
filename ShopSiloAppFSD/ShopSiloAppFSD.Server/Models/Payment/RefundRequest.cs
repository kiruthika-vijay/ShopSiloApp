namespace ShopSiloAppFSD.Server.Models.Payment
{
    public class RefundRequest
    {
        public string PaymentId { get; set; }
        public string ReceiptId { get; set; }
    }
}
