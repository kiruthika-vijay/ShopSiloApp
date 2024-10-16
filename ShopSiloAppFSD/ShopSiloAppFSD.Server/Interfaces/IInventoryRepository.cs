using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IInventoryRepository
    {
        Task AddInventoryAsync(Inventory inventory);
        Task UpdateInventoryAsync(Inventory inventory);
        Task DeleteInventoryAsync(int inventoryId);
        Task<Inventory?> GetInventoryByProductIdAsync(int productId);
        Task<IEnumerable<Product?>> GetLowStockProductsAsync(int threshold);
        Task RestockProductAsync(int productId, int quantity);
        Task<IEnumerable<InventoryDto>> GetInventoriesBySellerAsync();
    }

}
