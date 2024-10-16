using System.Collections.Generic;
using System.Threading.Tasks;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IWishlistRepository
    {
        Task<List<WishlistItemDTO>> GetAllWishlistsAsync();
        Task<Wishlist> GetWishlistByIdAsync(int id);
        Task<Wishlist> GetWishlistByUserIdAsync(int userId);
        Task<Wishlist> CreateWishlistAsync(Wishlist wishlist);
        Task RemoveItemFromWishlistAsync(int wishlistId, int productId);
        Task ClearWishlistAsync(int wishlistId);
        Task MoveAllItemsToBagAsync(int wishlistId, IShoppingCartRepository shoppingCartRepository);
        Task<bool> WishlistExistsAsync(int id);
        Task AddItemToWishlistAsync(int productId);
    }
}
