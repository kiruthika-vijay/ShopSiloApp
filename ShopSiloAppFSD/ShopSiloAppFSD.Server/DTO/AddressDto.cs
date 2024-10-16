namespace ShopSiloAppFSD.DTO
{
    public class AddressDto
    {
        public int AddressID { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public bool IsBillingAddress { get; set; }
        public bool IsShippingAddress { get; set; }
        public int CustomerID { get; set; }
    }
}

