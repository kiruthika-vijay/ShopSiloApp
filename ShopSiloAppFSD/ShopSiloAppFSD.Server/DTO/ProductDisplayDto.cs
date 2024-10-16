namespace ShopSiloAppFSD.Server.DTO
{
    public class ProductDisplayDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; } // Nullable in case there's no discount
        public string ImageURL { get; set; }
        public int StockQuantity { get; set; }
        // Category information
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
    }
}
