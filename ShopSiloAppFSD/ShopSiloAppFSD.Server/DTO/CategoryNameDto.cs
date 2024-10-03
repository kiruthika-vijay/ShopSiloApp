namespace ShopSiloAppFSD.Server.DTO
{
    public class CategoryNameDto
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Icon { get; set; } // Add this line
        public List<CategoryNameDto> SubCategories { get; set; } = new List<CategoryNameDto>();
    }
}
