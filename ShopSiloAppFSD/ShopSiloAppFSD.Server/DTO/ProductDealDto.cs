using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Server.DTO
{
    public class ProductDealDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty; // Assuming it's required
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public DateTime? FlashSaleStart { get; set; }
        public DateTime? FlashSaleEnd { get; set; }
        public bool IsFlashSaleActive { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageURL { get; set; }
        public int SellerID { get; set; }
        public string SellerName { get; set; } = string.Empty; // Assuming you want to include seller's name

        // Optionally include average rating or review count
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Constructor to initialize with product information
        public ProductDealDto() { }
        public ProductDealDto(Product product)
        {
            ProductID = product.ProductID;
            ProductName = product.ProductName ?? string.Empty;
            Description = product.Description;
            Price = product.Price;
            DiscountedPrice = product.DiscountedPrice;
            FlashSaleStart = product.FlashSaleStart;
            FlashSaleEnd = product.FlashSaleEnd;
            IsFlashSaleActive = product.IsFlashSaleActive;
            StockQuantity = product.StockQuantity;
            ImageURL = product.ImageURL;
            SellerID = product.SellerID;
            SellerName = product.Seller?.ContactPerson ?? string.Empty; // Assuming Seller has a Name property

            // Calculate average rating and review count
            if (product.ProductReviews.Any())
            {
                AverageRating = Math.Round(product.ProductReviews.Average(r => r.Rating), 2);
                ReviewCount = product.ProductReviews.Count;
            }
            else
            {
                AverageRating = 0;
                ReviewCount = 0;
            }
        }
    }
}
