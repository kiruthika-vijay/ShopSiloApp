using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IDiscountRepository
    {
        Task AddDiscountAsync(Discount discount);
        Task UpdateDiscountAsync(Discount discount);
        Task DeleteDiscountAsync(int discountId);
        Task<Discount?> GetDiscountByIdAsync(int discountId);
        Task<Discount> GetCouponByCodeAsync(string code);
        Task<UserDiscount> GetUserActiveDiscountAsync(int userId); // Check if the user has an active discount
        Task SaveUserDiscountAsync(int userId, string discountCode); // Save the applied discount for the user
        Task<decimal> ApplyDiscountAsync(string discountCode);
        Task<bool> ValidateDiscountAsync(string discountCode);
        Task<decimal> GetUserCartTotalAsync(int userId);
    }

}
