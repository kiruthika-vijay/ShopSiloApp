using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Server.DTO
{
    public class FlashSaleUpdateDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Discounted price must be greater than zero.")]
        public decimal? DiscountedPrice { get; set; }

        public DateTime? FlashSaleStart { get; set; }

        public DateTime? FlashSaleEnd { get; set; }
    }
}
