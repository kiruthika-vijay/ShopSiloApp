using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSiloAppFSD.Models
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string? Action { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
