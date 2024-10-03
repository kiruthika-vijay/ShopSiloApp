using Microsoft.AspNetCore.Identity;
using ShopSiloAppFSD.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSiloAppFSD.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters long.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column(TypeName = "datetime")]
        public DateTime? LastLogin { get; set; }

        public virtual CustomerDetail? CustomerDetail { get; set; }
        public virtual Seller? Seller { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public string? ResetToken { get; internal set; }
        public DateTime? TokenExpiration { get; internal set; }
        public bool IsActive { get; set; } = true;
    }
}
