using ShopSiloAppFSD.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Server.Models
{
    public class ColorVariation
    {
        [Key]
        public int ColorVariationID { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        [Required]
        [StringLength(50)]
        public string ColorName { get; set; } // Name of the color

        [StringLength(255)]
        public string ImageURL { get; set; } // Image URL for the color variation
    }
}
