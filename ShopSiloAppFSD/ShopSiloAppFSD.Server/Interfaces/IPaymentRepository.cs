using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Server.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentTransaction>> GetPaymentTransactionsByUserIdAsync(int userId);
        Task<IEnumerable<PaymentTransactionDto>> GetPaymentTransactionsByLoggedUserAsync();
        Task<(bool, int)> SavePaymentAndCreateOrderAsync(PaymentTransaction transaction, Order order, List<OrderItemDto> orderItems);
    }
}
