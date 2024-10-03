namespace ShopSiloAppFSD.DTO
{
    public class ProductDto
    {
        public int ProductID { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? CategoryName { get; set; } // Add this line

        public int TotalOrders { get; set; } // Add this line

        public decimal? DiscountedPrice { get; set; } // Include if needed for flash sales

        public int StockQuantity { get; set; }

        public string? ImageURL { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public int SellerID { get; set; }

        public int CategoryID { get; set; }

        // New properties for reviews
        public decimal AverageRating { get; set; } // Average rating of the product
        public int ReviewCount { get; set; } // Total number of reviews
        // New property for color variations
        public List<ColorVariationDto> ColorVariations { get; set; } = new List<ColorVariationDto>();
    }
}
