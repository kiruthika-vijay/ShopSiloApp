using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.Server.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Order>> GenerateSalesReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MonthlySalesReport>> GenerateMonthlySalesReportByMonthAsync();
        Task<IEnumerable<ProductWithReviewsDto>> GetTopSellingProductsAsync(int limit);
        Task UpdateCategoryStatusAsync(int categoryId, ApprovalStatus newStatus);
        Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsChartAsync(int limit);
    }
}
