using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Server.DTO
{
    public class ProductWithReviewsDto
    {
        public Product Product { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int Rank { get; set; } // The rank of the product
        public int SalesCount { get; set; } // The number of times this product was sold
        public string CategoryName { get; set; } // The name of the product category
    }
}
