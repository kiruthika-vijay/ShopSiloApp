namespace ShopSiloAppFSD.DTO
{
    public class OrderItemDto
    {
        public int OrderItemID { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }
        public string? ProductName { get; set; }
    }
}
