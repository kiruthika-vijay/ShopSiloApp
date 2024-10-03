namespace ShopSiloAppFSD.Server.DTO
{
    public class CartItemWithSellerDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int SellerId { get; set; }
    }
}
