namespace ShopSiloAppFSD.DTO
{
    public class CartItemDTO
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string ImageUrl { get; set; }
    }
}
