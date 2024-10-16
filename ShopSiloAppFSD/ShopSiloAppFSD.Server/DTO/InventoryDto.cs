namespace ShopSiloAppFSD.DTO
{
    public class InventoryDto
    {
        public int InventoryID { get; set; }       // Unique identifier for the inventory
        public int Quantity { get; set; }           // Current quantity of the product in stock
        public int ProductID { get; set; }          // Foreign key to the product
        public string? ProductName { get; set; }     // Name of the product
        public string? ProductImageUrl { get; set; } // URL to the product's image
        public string? Category { get; set; }        // Category of the product
        public decimal Price { get; set; }          // Price of the product
        public DateTime DateAdded { get; set; }     // Date when the inventory item was added
        public bool IsLowStock { get; set; }        // Indicates if the stock is low (threshold < 20)
        public bool IsActive { get; set; }          // Indicates if the inventory item is active
    }
}
