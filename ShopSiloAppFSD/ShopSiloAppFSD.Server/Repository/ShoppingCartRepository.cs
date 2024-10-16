using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly ShopSiloDBContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogConfiguration _auditLogConfig;
    private readonly string _userId;
    private readonly User _user;
    public ShoppingCartRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _auditLogConfig = auditLogConfig;
        _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(_userId))
        {
            _user = _context.Users.FirstOrDefault(u => u.Username == _userId); // Or await FindAsync for async
        }
    }

    // Add a new ShoppingCart
    public async Task AddCartAsync(ShoppingCart cart)
    {
        try
        {
            await _context.ShoppingCarts.AddAsync(cart);
            if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
            {
                AuditLog auditLog = new AuditLog()
                {
                    Action = $"Shopping Cart added for Cart ID : {cart.CartID}",
                    Timestamp = DateTime.Now,
                    UserId = _user.UserID
                };
                await _context.AuditLogs.AddAsync(auditLog);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error adding shopping cart.", ex);
        }
    }

    // CartRepository.cs
    public async Task<List<CartItemWithSellerDto>> GetCartItemsByLoggedUserAsync()
    {
        // Fetch the user's ShoppingCart based on username or email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == _userId || u.Email == _userId);

        if (user == null)
        {
            // Handle the case where the user is not found
            return new List<CartItemWithSellerDto>();
        }

        // Get the shopping cart associated with the user
        var shoppingCart = await _context.ShoppingCarts
            .Include(cart => cart.CartItems) // Include the cart items
            .ThenInclude(cartItem => cartItem.Product) // Include the product details for each cart item
            .FirstOrDefaultAsync(cart => cart.UserID == user.UserID);

        // Map the cart items to a DTO to return
        var cartItemsDto = shoppingCart?.CartItems.Select(cartItem => new CartItemWithSellerDto
        {
            CartItemId = cartItem.CartItemID,
            Quantity = cartItem.Quantity,
            ProductId = cartItem.Product.ProductID,
            ProductName = cartItem.Product.ProductName,
            ProductPrice = cartItem.Product.Price,
            SellerId = cartItem.Product.SellerID
        }).ToList() ?? new List<CartItemWithSellerDto>();

        return cartItemsDto;
    }


    // Update an existing ShoppingCart
    public async Task UpdateCartAsync(ShoppingCart cart)
    {
        try
        {
            var existingCart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartID == cart.CartID);

            if (existingCart != null)
            {
                _context.Entry(existingCart).CurrentValues.SetValues(cart);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shopping Cart updated for Cart ID : {cart.CartID}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException("Shopping cart not found.");
            }
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error updating shopping cart.", ex);
        }
    }

    // Get a ShoppingCart by ID
    public async Task<ShoppingCart?> GetCartByIdAsync(int cartId)
    {
        try
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CartID == cartId);

            if (cart == null)
            {
                throw new NotFoundException($"Shopping cart with ID {cartId} not found.");
            }

            return cart;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error retrieving shopping cart by ID.", ex);
        }
    }

    // Get a ShoppingCart by User ID
    public async Task<ShoppingCart?> GetCartByUserIdAsync(int userId)
    {
        try
        {
            return await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) // Include Product details for each CartItem
                .FirstOrDefaultAsync(c => c.UserID == userId);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error retrieving shopping cart by user ID.", ex);
        }
    }

    // Delete a ShoppingCart by ID
    public async Task DeleteCartAsync(int cartId)
    {
        try
        {
            var cart = await _context.ShoppingCarts.FindAsync(cartId);
            if (cart != null)
            {
                _context.ShoppingCarts.Remove(cart);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shopping Cart deleted for Cart ID : {cart.CartID}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException("Shopping cart not found.");
            }
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error deleting shopping cart.", ex);
        }
    }

    // Clear all items from a ShoppingCart
    public async Task ClearCartOfLoggedUserAsync()
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
            var userId = user.UserID;

            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems); // Remove all items from cart
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shopping Cart cleared cart items for Cart {cart.CartID}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException("Shopping cart not found.");
            }
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error clearing shopping cart.", ex);
        }
    }
    public async Task<int> GetCartCountAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email == userId || u.Username == userId));
        return await _context.ShoppingCarts
            .Where(c => c.UserID == user.UserID)
            .Select(c => c.CartItems.Count)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetWishlistCountAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email == userId || u.Username == userId));
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }
        return await _context.Wishlists
            .Where(w => w.UserID == user.UserID)
            .Select(w => w.WishlistItems.Count)
            .FirstOrDefaultAsync();
    }

    public async Task AddItemToCartAsync(int productId, int quantity)
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
        var cart = await _context.ShoppingCarts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserID == user.UserID);

        if (cart == null)
        {
            // Create a new cart if it doesn't exist
            cart = new ShoppingCart { UserID = user.UserID };
            _context.ShoppingCarts.Add(cart);
            await _context.SaveChangesAsync();
        }

        // Find the item in the cart, if it exists
        var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductID == productId);
        if (existingCartItem != null)
        {
            // Update quantity if the item already exists in the cart
            existingCartItem.Quantity += quantity;
        }
        else
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
            // Add the item to the cart if it's not present
            cart.CartItems.Add(new CartItem { ProductID = productId, Quantity = quantity, Price = product.Price });
        }

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Optionally log the action for auditing purposes
        if (_auditLogConfig.IsAuditLogEnabled)
        {
            AuditLog auditLog = new AuditLog()
            {
                Action = $"Added {quantity} of product {productId} to cart for user {user.UserID}.",
                Timestamp = DateTime.Now,
                UserId = user.UserID
            };
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<CartItemDTO>> GetAllCartItemsAsync()
    {
        if (string.IsNullOrEmpty(_userId))
        {
            // Handle the case where the user is not logged in
            return new List<CartItemDTO>();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
        var shoppingCart = await _context.ShoppingCarts
            .Include(sc => sc.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(sc => sc.UserID == user.UserID); // Get the user's shopping cart

        if (shoppingCart == null)
        {
            return new List<CartItemDTO>(); // No cart found for the user
        }

        return shoppingCart.CartItems.Select(ci => new CartItemDTO
        {
            CartItemID = ci.CartItemID,
            ProductID = ci.ProductID,
            ProductName = ci.Product?.ProductName ?? "Unknown Product",
            Quantity = ci.Quantity,
            Price = ci.Price,
            DiscountedPrice = ci.Product?.DiscountedPrice,
            DiscountPercentage = ci.Product?.DiscountedPrice.HasValue == true
                ? (1 - (ci.Product.DiscountedPrice.Value / ci.Product.Price)) * 100
                : (decimal?)null,
            ImageUrl = ci.Product?.ImageURL ?? "D:\\HEXA-SEGUE 201 - .NET FSD\\FullStackSHOPSILO\\ShopSiloAppFSD\\shopsiloappfsd.client\\public\\images\\defaultProductImage.jpg"
        }).ToList();
    }
}