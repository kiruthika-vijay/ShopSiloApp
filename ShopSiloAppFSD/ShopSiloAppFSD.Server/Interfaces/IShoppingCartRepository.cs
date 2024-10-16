using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task AddCartAsync(ShoppingCart cart);
        Task UpdateCartAsync(ShoppingCart cart);
        Task<ShoppingCart?> GetCartByIdAsync(int cartId);
        Task<ShoppingCart?> GetCartByUserIdAsync(int userId);
        Task DeleteCartAsync(int cartId);
        Task ClearCartOfLoggedUserAsync();
        Task<List<CartItemWithSellerDto>> GetCartItemsByLoggedUserAsync();
        Task<int> GetCartCountAsync(string userId);
        Task<int> GetWishlistCountAsync(string userId);
        Task AddItemToCartAsync(int productId, int quantity);
        Task<List<CartItemDTO>> GetAllCartItemsAsync();
    }
}
