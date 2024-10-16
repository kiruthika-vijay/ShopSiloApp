namespace ShopSiloAppFSD.Server.Models
{
    public class UserDiscount
    {
        public int UserDiscountID { get; set; }
        public int UserID { get; set; }
        public string DiscountCode { get; set; }
        public bool IsActive { get; set; }
        public DateOnly AppliedAt { get; set; }
    }

}
