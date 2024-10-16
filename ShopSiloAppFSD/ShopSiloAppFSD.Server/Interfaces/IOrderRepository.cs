using Razorpay.Api;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using Order = ShopSiloAppFSD.Models.Order;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IOrderRepository
    {
        Task PlaceOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<OrderDetailDto?> GetOrderDetailByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderDto>> GetOrdersByLoggedUserAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task CancelOrderAsync(int orderId);
        Task UpdateTrackingNumberAsync(int orderId, string trackingNumber);
        Task<string?> GetTrackingNumberAsync(int orderId);
        Task AddTrackingNumberAsync(int orderId, string trackingNumber);
        Task<IEnumerable<OrderDto>> GetSellerOrdersAsync(int sellerId);
    }

}
