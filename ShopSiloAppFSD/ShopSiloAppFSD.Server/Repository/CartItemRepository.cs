using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopSiloAppFSD.Exceptions;
using Razorpay.Api;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ShopSiloAppFSD.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;

        public CartItemRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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


        // Add a new cart item
        public async Task AddCartItemAsync(CartItem cartItem)
        {
            try
            {
                await _context.CartItems.AddAsync(cartItem);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Cart item {cartItem.ProductID} added to cart {cartItem.CartID}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                throw new RepositoryException("Error adding cart item.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred.", ex);
            }
        }

        // Update an existing cart item
        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            try
            {
                var existingCartItem = await _context.CartItems.FindAsync(cartItem.CartItemID);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity = cartItem.Quantity;
                    existingCartItem.Price = cartItem.Price;
                    existingCartItem.ProductID = cartItem.ProductID;
                    existingCartItem.CartID = cartItem.CartID;

                    _context.CartItems.Update(existingCartItem);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Cart item {cartItem.ProductID} updated in cart {cartItem.CartID}",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Cart item not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating cart item.", ex);
            }
        }

        public async Task<CartItem> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                return cartItem;
            }
            return null; // Return null if the cart item was not found
        }


        // Delete a cart item by its ID
        // Delete a cart item by its ID
        public async Task<bool> DeleteCartItemAsync(int cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(cartItemId);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Cart item {cartItemId} deleted from cart",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                    return true; // Indicate success
                }
                return false; // Indicate failure (item not found)
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting cart item.", ex);
            }
        }

        // Get cart items by Cart ID (retrieve all items in a specific shopping cart)
        public async Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId)
        {
            try
            {
                return await _context.CartItems
                                     .Where(ci => ci.CartID == cartId)
                                     .Include(ci => ci.Product)  // Eager loading related Product
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving cart items by cart ID.", ex);
            }
        }

        // Get a specific cart item by its ID
        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        {
            try
            {
                return await _context.CartItems
                                     .Include(ci => ci.Product)       // Eager load related Product
                                     .Include(ci => ci.ShoppingCart)  // Eager load related ShoppingCart
                                     .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving cart item by ID.", ex);
            }
        }
    }
}
