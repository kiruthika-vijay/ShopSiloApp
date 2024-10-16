using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Interfaces
{
    public interface ISellerDashboardRepository
    {
        Task<SellerDashboard> GetSellerDashboardByIdAsync(int dashboardId);
        Task<SellerDashboard> GetSellerDashboardByLoggedUserAsync();
        Task<IEnumerable<ProductWithReviewsDto>> GetAllSellingProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<int> GetTotalOrdersPerProductAsync(int productId);
        Task<IEnumerable<Product>> GetFilteredProductsAsync(string productName, string categoryName, int? rating);
        Task<IEnumerable<string>> GetProductNamesBySellerAsync();
    }
}
