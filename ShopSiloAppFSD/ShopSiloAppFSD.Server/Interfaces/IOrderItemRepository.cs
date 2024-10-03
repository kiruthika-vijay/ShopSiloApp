using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IOrderItemRepository
    {
        Task AddOrderItemAsync(OrderItem orderItem);
        Task UpdateOrderItemAsync(OrderItem orderItem);
        Task DeleteOrderItemAsync(int orderItemId);
        Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId);
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
    }
}
