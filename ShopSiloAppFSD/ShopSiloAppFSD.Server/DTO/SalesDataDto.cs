namespace ShopSiloAppFSD.DTO
{
    public class SalesDataDto
    {
        public int SalesDataID { get; set; }
        public int OrdersID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
