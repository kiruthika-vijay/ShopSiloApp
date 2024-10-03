using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShopSiloAppFSD.Server.Models;

namespace ShopSiloAppFSD.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(100)]
        public string? ProductName { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; } = 0;

        [StringLength(255)]
        public string? ImageURL { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public int SellerID { get; set; }
        [ForeignKey("SellerID")]
        public virtual Seller? Seller { get; set; }

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        // *** Flash Sale Properties ***
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountedPrice { get; set; } // Discounted Price for Flash Sale

        [Column(TypeName = "datetime")]
        public DateTime? FlashSaleStart { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FlashSaleEnd { get; set; }

        [NotMapped]
        public bool IsFlashSaleActive => FlashSaleStart.HasValue && FlashSaleEnd.HasValue &&
                                         DateTime.UtcNow >= FlashSaleStart.Value &&
                                         DateTime.UtcNow <= FlashSaleEnd.Value;

        [NotMapped] // Not stored in the database
        public bool IsNewArrival => (DateTime.UtcNow - CreatedDate).TotalDays <= 30; // Adjust the days as needed
                                                                                     // Stores color variations (could be a JSON string or a separate entity)
        public virtual ICollection<ColorVariation> ColorVariations { get; set; } = new List<ColorVariation>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
