namespace ShopSiloAppFSD.DTO
{
    public class DiscountDto
    {
        public int DiscountID { get; set; }
        public string DiscountCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal MinimumPurchaseAmount { get; set; }
    }
}
