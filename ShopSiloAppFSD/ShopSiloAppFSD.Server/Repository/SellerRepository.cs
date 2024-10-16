using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Security.Claims;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.DTO; // Assuming exceptions are in this namespace

namespace ShopSiloAppFSD.Repository
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public SellerRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditLogConfig = auditLogConfig;
            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(_userId))
            {
                _user = _context.Users.FirstOrDefault(u => (u.Username == _userId || u.Email == _userId)); // Or await FindAsync for async
            }
        }

        public async Task AddSellerAsync(Seller seller)
        {
            try
            {
                await _context.Sellers.AddAsync(seller);
                await _context.SaveChangesAsync();

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "Seller detail added to the database.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Error adding seller to the database.", ex);
            }
        }

        public async Task UpdateSellerAsync(Seller seller)
        {
            try
            {
                var existingSeller = await _context.Sellers.FindAsync(seller.SellerID);
                if (existingSeller == null)
                {
                    throw new NotFoundException($"Seller with ID {seller.SellerID} not found.");
                }

                existingSeller.CompanyName = seller.CompanyName;
                existingSeller.ContactPerson = seller.ContactPerson;
                existingSeller.ContactNumber = seller.ContactNumber;
                existingSeller.Address = seller.Address;
                existingSeller.StoreDescription = seller.StoreDescription;

                _context.Sellers.Update(existingSeller);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "Seller detail updated in the database.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency error occurred while updating seller.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating seller.", ex);
            }
        }

        public async Task<Seller> GetSellerByIdAsync(int sellerId)
        {
            try
            {
                var seller = await _context.Sellers.FindAsync(sellerId);
                if (seller == null)
                {
                    throw new NotFoundException($"Seller with ID {sellerId} not found.");
                }

                return seller;
            }
            catch(NotFoundException)
            {
                throw; // Rethrow NotFoundException directly without wrapping it
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving seller by ID.", ex);
            }
        }

        public async Task<Seller> GetSellerByLoggedAsync()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
                var seller = await _context.Sellers.FindAsync(user.UserID);
                if (seller == null)
                {
                    throw new NotFoundException($"Seller with ID {seller.SellerID} not found.");
                }

                return seller;
            }
            catch (NotFoundException)
            {
                throw; // Rethrow NotFoundException directly without wrapping it
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving seller by ID.", ex);
            }
        }

        public async Task<IEnumerable<Seller>> GetSeller()
        {
            try
            {
                var seller = await _context.Sellers.ToListAsync();


                return seller;
            }
            catch (NotFoundException)
            {
                throw; // Rethrow NotFoundException directly without wrapping it
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving seller by ID.", ex);
            }
        }

        public async Task DeleteSellerAsync(int sellerId)
        {
            try
            {
                var seller = await _context.Sellers
                    .Include(s => s.Products)
                    .Include(s => s.SellerDashboard)
                    .FirstOrDefaultAsync(s => s.SellerID == sellerId);

                if (seller == null)
                {
                    throw new NotFoundException($"Seller with ID {sellerId} not found.");
                }

                // Check for pending orders
                var pendingOrders = await _context.Orders
                    .Where(o => o.SellerID == sellerId && o.OrderStatus == OrderStatus.Pending)
                    .ToListAsync();

                if (pendingOrders.Any())
                {
                    throw new RepositoryException($"Cannot delete Seller data, because the seller {sellerId} has pending orders to complete.");
                }

                // Handle related data in CartItems that reference the seller's products
                foreach (var product in seller.Products)
                {
                    var relatedCartItems = await _context.CartItems
                        .Where(ci => ci.ProductID == product.ProductID)
                        .ToListAsync();

                    _context.CartItems.RemoveRange(relatedCartItems);
                }

                // Now perform the soft delete
                seller.IsActive = false;

                // Soft delete related entities
                if (seller.SellerDashboard != null)
                {
                    seller.SellerDashboard.IsActive = false;
                }

                if (_user != null)
                {
                    _user.IsActive = false;
                }

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    var auditLog = new AuditLog
                    {
                        Action = $"Seller detail set inactive for SellerID {sellerId}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID // Use null conditional operator to avoid null reference issues
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Error deleting seller from the database.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Cannot delete Seller data, because the seller {sellerId} has pending orders to complete", ex);
            }
        }

        public async Task<IEnumerable<Seller>> GetTopSellersAsync(int limit)
        {
            try
            {
                return await _context.Sellers
                    .OrderByDescending(s => s.SellerDashboard.TotalSales) // Ordering by TotalSales from SellerDashboard
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving top sellers.", ex);
            }
        }

        public async Task<IEnumerable<SalesData>> GetSalesDataByDateRangeAsync(int sellerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.SellerID == sellerId && o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .Select(o => new SalesData
                    {
                        OrderID = o.OrderID,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving sales data by date range.", ex);
            }
        }

        public async Task<IEnumerable<Seller>> GetAllSellersAsync()
        {
            try
            {
                return await _context.Sellers.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all sellers.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetSellerProductsAsync(int sellerId)
        {
            try
            {
                return await _context.Products
                    .Where(p => p.SellerID == sellerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving products for seller.", ex);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetLoggedSellerProductsAsync()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => (s.Username == _userId || s.Email == _userId));
                var seller = await _context.Sellers.FindAsync(user.UserID);

                var products = await _context.Products
                    .Include(p => p.Category) // Include category details
                    .Where(p => p.IsActive && p.SellerID == seller.SellerID)
                    .Select(p => new ProductDto
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        ImageURL = p.ImageURL,
                        CreatedDate = p.CreatedDate,
                        IsActive = p.IsActive,
                        SellerID = p.SellerID,

                        // Additional Fields
                        CategoryName = p.Category.CategoryName, // Assuming a relation to Category
                        TotalOrders = _context.OrderItems.Where(o => o.ProductID == p.ProductID).Sum(o => o.Quantity),
                        ReviewCount = _context.ProductReviews.Count(r => r.ProductID == p.ProductID),
                        AverageRating = _context.ProductReviews.Where(r => r.ProductID == p.ProductID).Average(r => (decimal?)r.Rating) ?? 0
                    })
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving products for seller.", ex);
            }
        }
        public async Task<IEnumerable<Order>> GetSellerOrdersAsync()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => (s.Username == _userId || s.Email == _userId));
                var seller = await _context.Sellers.FindAsync(_user.UserID);
                return await _context.Orders
                    .Where(o => o.SellerID == seller.SellerID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving orders for seller.", ex);
            }
        }
    }
}
