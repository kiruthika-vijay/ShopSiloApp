using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public DiscountRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        // Method to add a new discount
        public async Task AddDiscountAsync(Discount discount)
        {
            try
            {
                await _context.Discounts.AddAsync(discount);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"New Discount added {discount.DiscountID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding discount.", ex);
            }
        }


        public async Task<Discount> GetCouponByCodeAsync(string code)
        {
            return await _context.Discounts
                .Where(c => c.DiscountCode == code && c.IsActive && c.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task<UserDiscount> GetUserActiveDiscountAsync(int userId)
        {
            // Fetch the active discount for the user
            return await _context.UserDiscounts
                .Where(ud => ud.UserID == userId && ud.IsActive) // Assuming IsActive indicates the discount is currently applicable
                .FirstOrDefaultAsync();
        }

        public async Task SaveUserDiscountAsync(int userId, string discountCode)
        {
            // Save the applied discount for the user
            var userDiscount = new UserDiscount
            {
                UserID = userId,
                DiscountCode = discountCode,
                IsActive = true,
                AppliedAt = DateOnly.FromDateTime(DateTime.UtcNow) // Save the date of application
            };

            await _context.UserDiscounts.AddAsync(userDiscount);
            await _context.SaveChangesAsync();
        }

        // Method to update an existing discount
        public async Task UpdateDiscountAsync(Discount discount)
        {
            try
            {
                // Only update the existing entity, do not add a new one
                _context.Discounts.Update(discount);

                // Check if audit log is enabled and add an audit log entry if necessary
                if (_auditLogConfig.IsAuditLogEnabled)
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Discount details updated for {discount.DiscountID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating discount.", ex);
            }
        }


        // Method to delete a discount by ID
        public async Task DeleteDiscountAsync(int discountId)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(discountId);
                if (discount != null)
                {
                    _context.Discounts.Remove(discount);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Discount details removed for {discountId}.",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Discount not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting discount.", ex);
            }
        }

        // Method to get a discount by ID
        public async Task<Discount?> GetDiscountByIdAsync(int discountId)
        {
            try
            {
                return await _context.Discounts.FindAsync(discountId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving discount by ID.", ex);
            }
        }

        // Method to apply a discount for a specific user and discount code
        public async Task<decimal> ApplyDiscountAsync(string discountCode)
        {
            // 1. Validate input
            if (string.IsNullOrEmpty(discountCode))
                throw new ArgumentException("Invalid input.");

            // 2. Get the user ID from the claims
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
            if (user == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            int userId = user.UserID;

            // 3. Fetch the coupon from the repository
            var coupon = await GetCouponByCodeAsync(discountCode);
            if (coupon == null)
                throw new ArgumentException("Invalid or expired coupon.");

            // 4. Check if the user has already applied a discount
            var userActiveDiscount = await GetUserActiveDiscountAsync(userId);
            if (userActiveDiscount != null)
            {
                throw new InvalidOperationException("User has already applied a discount for this purchase.");
            }

            // 5. Save the applied discount for the user
            await SaveUserDiscountAsync(userId, coupon.DiscountCode);

            return coupon.DiscountPercentage;
        }

        // Method to validate if a discount is still valid (e.g., not expired, and exists)
        public async Task<bool> ValidateDiscountAsync(string discountCode)
        {
            try
            {
                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.DiscountCode == discountCode);
                return discount != null && discount.ExpiryDate >= DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error validating discount.", ex);
            }
        }

        // Helper method to calculate the user's cart total (asynchronous version)
        public async Task<decimal> GetUserCartTotalAsync(int userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart != null)
            {
                return cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            }

            return 0;
        }
    }
}