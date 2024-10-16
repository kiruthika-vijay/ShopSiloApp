using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSiloAppFSD.Models
{
    public class SellerDashboard
    {
        [Key]
        public int DashboardID { get; set; }

        public decimal TotalSales { get; set; }

        public int TotalOrders { get; set; }

        public int TotalProducts { get; set; }

        public decimal TotalRevenue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("DashboardID")]
        public virtual Seller? Seller { get; set; }
    }
}
