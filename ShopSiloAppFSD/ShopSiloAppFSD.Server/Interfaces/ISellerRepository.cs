using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface ISellerRepository
    {
        Task AddSellerAsync(Seller seller);
        Task UpdateSellerAsync(Seller seller);
        Task<Seller> GetSellerByIdAsync(int sellerId);
        Task<Seller> GetSellerByLoggedAsync();
        Task<IEnumerable<Seller>> GetSeller();
        Task DeleteSellerAsync(int sellerId);
        Task<IEnumerable<Seller>> GetTopSellersAsync(int limit);
        Task<IEnumerable<SalesData>> GetSalesDataByDateRangeAsync(int sellerId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Seller>> GetAllSellersAsync();
        Task<IEnumerable<Product>> GetSellerProductsAsync(int sellerId);
        Task<IEnumerable<ProductDto>> GetLoggedSellerProductsAsync();
        Task<IEnumerable<Order>> GetSellerOrdersAsync();
    }
}
