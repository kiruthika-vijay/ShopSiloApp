using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class ProductReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? ReviewText { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }

}
