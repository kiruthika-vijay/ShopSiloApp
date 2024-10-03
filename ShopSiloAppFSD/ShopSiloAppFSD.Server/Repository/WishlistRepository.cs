using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.Exceptions;

namespace ShopSiloAppFSD.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;

        public WishlistRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<List<WishlistItemDTO>> GetAllWishlistsAsync()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                // Handle the case where the user is not logged in
                return new List<WishlistItemDTO>();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username ==  _userId || u.Email == _userId));
            var wishlists = await _context.Wishlists
                .Where(w => w.UserID == user.UserID)  // Filter wishlists by the logged-in user ID
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .ToListAsync();

            return wishlists.SelectMany(w => w.WishlistItems.Select(wi => new WishlistItemDTO
            {
                WishListID = w.WishListID,
                ProductID = wi.ProductID,
                ProductName = wi.Product?.ProductName ?? "Unknown Product",
                Price = wi.Product?.Price ?? 0,
                DiscountedPrice = wi.Product?.DiscountedPrice,
                DiscountPercentage = wi.Product?.DiscountedPrice.HasValue == true
                    ? (1 - (wi.Product.DiscountedPrice.Value / wi.Product.Price)) * 100
                    : (decimal?)null,
                ImageUrl = wi.Product?.ImageURL ?? "https://via.placeholder.com/300"
            })).ToList();
        }


        public async Task<Wishlist> GetWishlistByIdAsync(int id)
        {
            if (string.IsNullOrEmpty(_userId))
            {
                throw new Exception("User is not authenticated.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return await _context.Wishlists
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.WishListID == id && w.UserID == user.UserID);
        }

        public async Task<Wishlist> GetWishlistByUserIdAsync(int userId)
        {
            return await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserID == userId);
        }

        public async Task<Wishlist> CreateWishlistAsync(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task RemoveItemFromWishlistAsync(int wishlistId, int productId)
        {
            var wishlist = await GetWishlistByIdAsync(wishlistId);
            if (wishlist != null)
            {
                var itemToRemove = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductID == productId);
                if (itemToRemove != null)
                {
                    _context.WishlistItems.Remove(itemToRemove);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ClearWishlistAsync(int wishlistId)
        {
            var wishlist = await GetWishlistByIdAsync(wishlistId);
            if (wishlist != null)
            {
                _context.WishlistItems.RemoveRange(wishlist.WishlistItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MoveAllItemsToBagAsync(int wishlistId, IShoppingCartRepository shoppingCartRepository)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
            var wishlist = await GetWishlistByIdAsync(wishlistId);
            if (wishlist != null)
            {
                foreach (var item in wishlist.WishlistItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID ==  item.ProductID); 
                    // Add each item to the shopping cart
                    await shoppingCartRepository.AddItemToCartAsync(item.ProductID, 1); // Assuming quantity is 1
                }
                await ClearWishlistAsync(wishlistId);
            }
        }

        public async Task<bool> WishlistExistsAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == _userId || u.Email == _userId);
            if (user == null) return false;

            return await _context.Wishlists.AnyAsync(e => e.WishListID == id && e.UserID == user.UserID);
        }

        public async Task AddItemToWishlistAsync(int productId)
        {
            // Validate the user ID from the JWT token
            if (string.IsNullOrEmpty(_userId))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Find the cart for the user
            var wishlist = await _context.Wishlists
                .Include(c => c.WishlistItems)
                .FirstOrDefaultAsync(c => c.UserID == user.UserID);

            if (wishlist == null)
            {
                // Create a new cart if it doesn't exist
                wishlist = new Wishlist { UserID = user.UserID };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            // Find the item in the cart, if it exists
            var existingWishlistItem = wishlist.WishlistItems.FirstOrDefault(ci => ci.ProductID == productId);
            if (existingWishlistItem != null)
            {
                return;   
            }
            else
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
                // Add the item to the cart if it's not present
                wishlist.WishlistItems.Add(new WishlistItem { ProductID = productId});
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

    }
}
