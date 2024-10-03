namespace ShopSiloAppFSD.Server.DTO
{
    public class WishlistItemDTO
    {
        public int WishListID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string ImageUrl { get; set; }
    }
}
