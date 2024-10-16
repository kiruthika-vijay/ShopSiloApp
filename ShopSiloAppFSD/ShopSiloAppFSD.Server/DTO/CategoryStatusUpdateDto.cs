using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.DTO
{
    public class CategoryStatusUpdateDto
    {
        public ApprovalStatus Status { get; set; } // Expected values: "Approved" or "Rejected"
    }
}
