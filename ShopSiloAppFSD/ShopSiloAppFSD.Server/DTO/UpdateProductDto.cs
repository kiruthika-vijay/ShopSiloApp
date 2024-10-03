namespace ShopSiloAppFSD.Server.DTO
{
    public class UpdateProductDto
    {
        public int ProductID { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? CategoryName { get; set; } // Add this line

        public int StockQuantity { get; set; }

        public IFormFile? Image { get; set; }

        public int SellerID { get; set; }

        public int CategoryID { get; set; }

        public string? ImageURL { get; set; } // Property to hold the uploaded image URL
        public bool RemoveImage { get; set; } // Indicates if the image should be removed
    }
}
