namespace ShopSiloAppFSD.DTO
{
    public class ProductReviewDto
    {
        public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public int ProductID { get; set; } 
        public int UserID { get; set; }    

    }
}
