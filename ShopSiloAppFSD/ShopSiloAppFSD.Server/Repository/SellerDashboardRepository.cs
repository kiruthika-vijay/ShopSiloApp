using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Security.Claims;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Server.DTO; // Assuming exceptions are in this namespace

namespace ShopSiloAppFSD.Repository
{
    public class SellerDashboardRepository : ISellerDashboardRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public SellerDashboardRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        public async Task<SellerDashboard> GetSellerDashboardByIdAsync(int dashboardId)
        {
            try
            {
                var dashboard = await _context.SellerDashboards
                    .Include(sd => sd.Seller) // Include related Seller if needed
                    .SingleOrDefaultAsync(sd => sd.DashboardID == dashboardId);

                if (dashboard == null)
                {
                    throw new NotFoundException($"SellerDashboard with ID {dashboardId} not found.");
                }

                return dashboard;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving SellerDashboard by ID.", ex);
            }
        }

        public async Task<SellerDashboard> GetSellerDashboardByLoggedUserAsync()
        {
            try
            {
                var dashboardId = _user.UserID;
                var dashboard = await _context.SellerDashboards
                    .Include(sd => sd.Seller) // Include related Seller if needed
                    .SingleOrDefaultAsync(sd => sd.DashboardID == dashboardId);

                if (dashboard == null)
                {
                    throw new NotFoundException($"SellerDashboard with ID {dashboardId} not found.");
                }

                return dashboard;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving SellerDashboard by ID.", ex);
            }
        }

        public async Task<IEnumerable<ProductWithReviewsDto>> GetAllSellingProductsAsync()
        {
            try
            {
                // Fetch the current user and corresponding seller information
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));

                if (user == null)
                    throw new RepositoryException("User not found.");

                var seller = await _context.Sellers.FindAsync(user.UserID);
                if (seller == null)
                    throw new RepositoryException("Seller not found.");

                var sellerId = seller.SellerID;

                // Query to get products sold by the seller, grouped by ProductID and ranked by sales count
                var query = _context.OrderItems
                    .Join(_context.Orders,
                        oi => oi.OrderID,
                        o => o.OrderID,
                        (oi, o) => new { oi, o }) // Joining OrderItems and Orders
                    .Where(joined => joined.o.SellerID == sellerId) // Filter for the seller
                    .GroupBy(joined => joined.oi.ProductID) // Group by ProductID
                    .Select(g => new
                    {
                        ProductID = g.Key, // The ProductID
                        SalesCount = g.Count() // Count how many times this product was sold
                    })
                    .OrderByDescending(g => g.SalesCount); // Order by sales count in descending order

                // Fetch the actual product data, including reviews and category, for all products sold by the seller
                var productsWithSalesCount = await query.ToListAsync();

                var products = new List<ProductWithReviewsDto>();

                foreach (var result in productsWithSalesCount)
                {
                    var product = await _context.Products
                        .Include(p => p.ProductReviews) // Include reviews
                        .Include(p => p.Category) // Include the Category to fetch the category name
                        .FirstOrDefaultAsync(p => p.ProductID == result.ProductID);

                    if (product != null)
                    {
                        var averageRating = product.ProductReviews.Any()
                            ? product.ProductReviews.Average(r => (double?)r.Rating) ?? 0
                            : 0;

                        var reviewCount = product.ProductReviews.Count();

                        products.Add(new ProductWithReviewsDto
                        {
                            Product = product,
                            AverageRating = averageRating,
                            ReviewCount = reviewCount,
                            SalesCount = result.SalesCount, // Include the total sales count
                            CategoryName = product.Category.CategoryName // Get the category name
                        });
                    }
                }

                // Return the list of products with their sales count
                return products;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all selling products.", ex);
            }
        }


        // Fetch a single product by ID for the logged-in seller
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var user = _user.UserID;
            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.SellerID == user);
            if (seller == null)
            {
                throw new NotFoundException("User not authenticated");
            }
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductReviews)
                .Include(p => p.OrderItems)
                .Where(p => p.SellerID == seller.SellerID) // Ensure only products of the logged-in seller are fetched
                .FirstOrDefaultAsync(p => p.ProductID == productId);
        }

        // Fetch products based on filtering conditions for the logged-in seller
        public async Task<IEnumerable<Product>> GetFilteredProductsAsync(string productName, string categoryName, int? rating)
        {
            var user = _user.UserID;
            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.SellerID == user);
            if(seller == null)
            {
                throw new NotFoundException("User not authenticated");
            }
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductReviews)
                .Include(p => p.OrderItems)
                .Where(p => p.SellerID == seller.SellerID) // Filter by SellerID
                .AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(p => p.ProductName.Contains(productName));
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category.CategoryName.Contains(categoryName));
            }

            if (rating.HasValue)
            {
                query = query.Where(p => p.ProductReviews.Any() &&
                                         p.ProductReviews.Average(r => r.Rating) >= rating.Value);
            }

            return await query.ToListAsync();
        }
        // Method to calculate total orders per product ID
        public async Task<int> GetTotalOrdersPerProductAsync(int productId)
        {
            var totalOrders = await _context.OrderItems
                .Where(oi => oi.ProductID == productId)
                .CountAsync();

            return totalOrders;
        }

        public async Task<IEnumerable<string>> GetProductNamesBySellerAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
            var seller = await _context.Sellers.FindAsync(user.UserID);
            return await _context.Products
                .Where(p => p.SellerID == seller.SellerID)
                .Select(p => p.ProductName)
                .ToListAsync();
        }
    }
}
