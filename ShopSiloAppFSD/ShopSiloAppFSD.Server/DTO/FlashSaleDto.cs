using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Server.DTO
{
    public class FlashSaleDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discounted price must be greater than zero.")]
        public decimal DiscountedPrice { get; set; }

        [Required]
        public DateTime FlashSaleStart { get; set; }

        [Required]
        public DateTime FlashSaleEnd { get; set; }
    }
}
