using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task ToggleProductStatusAsync(int productId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDisplayDto>> GetProductsByCategoryAsync(int? categoryId, string categoryName);
        Task<IEnumerable<Product>> GetProductsByParentCategoryIdAsync(int parentCategoryId);
        Task<IEnumerable<Product>> GetTopRatedProductsAsync(int limit);
        Task UpdateStockQuantityAsync(int productId);
        Task<IEnumerable<Product>> GetProductsBySellerAsync(int sellerId);
        Task<List<ProductDto>> GetNewArrivalsAsync(int limit);
        Task<List<Discount>> GetTopDealsAsync(int limit);
        Task<IEnumerable<ProductDealDto>> GetFlashSaleProductsAsync();
        Task AddFlashSaleAsync(int productId, decimal discountedPrice, DateTime flashSaleStart, DateTime flashSaleEnd);
        Task UpdateFlashSaleAsync(int productId, decimal? discountedPrice, DateTime? flashSaleStart, DateTime? flashSaleEnd);
        Task RemoveFlashSaleAsync(int productId);
        Task<IEnumerable<ProductDto>> GetExploreProductsAsync();
        Task<List<Product>> GetSuggestedProducts();
    }
}
