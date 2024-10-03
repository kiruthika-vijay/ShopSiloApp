using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface ICartItemRepository
    {
        Task AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
            Task<bool> DeleteCartItemAsync(int cartItemId);
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
    }
}
