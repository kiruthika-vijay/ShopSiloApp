using ShopSiloAppFSD.Enums;

namespace ShopSiloAppFSD.DTO
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
    }
}
