using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [StringLength(100)]
        public string? CategoryName { get; set; }

        [StringLength(100)]
        public string? Icon {  get; set; }

        public int? ParentCategoryId { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public bool IsActive { get; set; } = true;
        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }
}
