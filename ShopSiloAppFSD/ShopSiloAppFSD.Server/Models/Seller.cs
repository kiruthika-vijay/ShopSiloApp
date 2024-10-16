using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class Seller
    {
        [Key]
        public int SellerID { get; set; }

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public string? StoreDescription { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("SellerID")]
        public virtual User? User { get; set; }

        public virtual SellerDashboard? SellerDashboard { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
