using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions; // Assuming exceptions are in this namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopSiloAppFSD.Services;
using System.Security.Claims;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.DTO;
using iText.Commons.Actions.Data;

namespace ShopSiloAppFSD.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string? _userId;
        private readonly User? _user;

        public ProductRepository(
            ShopSiloDBContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditLogConfiguration auditLogConfig)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditLogConfig = auditLogConfig;
            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(_userId))
            {
                _user = _context.Users.FirstOrDefault(u => u.Username == _userId); // Or use FindAsync for async
            }
        }

        public async Task AddProductAsync(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"New product {product.ProductName} added to the database.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding product.", ex);
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.ProductID);
                if (existingProduct == null)
                {
                    throw new NotFoundException($"Product with ID {product.ProductID} not found.");
                }

                existingProduct.ProductName = product.ProductName;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.ImageURL = product.ImageURL;
                existingProduct.SellerID = product.SellerID;
                existingProduct.CategoryID = product.CategoryID;
                existingProduct.LastUpdatedDate = DateTime.UtcNow;

                // Update Flash Sale properties if provided
                if (product.DiscountedPrice.HasValue)
                {
                    existingProduct.DiscountedPrice = product.DiscountedPrice;
                }
                if (product.FlashSaleStart.HasValue)
                {
                    existingProduct.FlashSaleStart = product.FlashSaleStart;
                }
                if (product.FlashSaleEnd.HasValue)
                {
                    existingProduct.FlashSaleEnd = product.FlashSaleEnd;
                }

                await UpdateStockQuantityAsync(product.ProductID);

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Updated Product details for {product.ProductName}.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
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
                throw new RepositoryException("Concurrency error occurred while updating product.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating product.", ex);
            }
        }

        public async Task ToggleProductStatusAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = !product.IsActive; // Toggle the status
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new NotFoundException($"Product with ID {productId} not found.");
                }

                product.IsActive = false;
                product.LastUpdatedDate = DateTime.UtcNow;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Product {product.ProductName} set as inactive for sales.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting product.", ex);
            }
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Seller)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.ProductID == productId);

                if (product == null)
                {
                    throw new NotFoundException($"Product with ID {productId} not found.");
                }

                return product;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving product by ID.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Seller)
                    .Include(p => p.Category).
                    Where(c => c.IsActive == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all products.", ex);
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.ProductName.Contains(searchTerm) || p.Description.Contains(searchTerm))
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Unable to find the searched products.", ex);
            }
        }

        public async Task<IEnumerable<ProductDisplayDto>> GetProductsByCategoryAsync(int? categoryId, string categoryName)
        {
            try
            {
                // Assuming you have a method to get products based on category ID
                var categoryProducts = await _context.Products
                                        .Where(p => p.CategoryID == categoryId && p.IsActive) // Filter by category and active status
                                        .Select(p => new ProductDisplayDto
                                        {
                                            ProductID = p.ProductID,
                                            ProductName = p.ProductName,
                                            Description = p.Description,
                                            Price = p.Price,
                                            DiscountedPrice = p.DiscountedPrice,
                                            StockQuantity = p.StockQuantity,
                                            ImageURL = p.ImageURL
                                        }).ToListAsync();
                return categoryProducts;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving products by category.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByParentCategoryIdAsync(int parentCategoryId)
        {
            // Fetch all subcategories under the specified parent category
            var subCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .Select(c => c.CategoryID) // Select only the Category IDs
                .ToListAsync();

            if (!subCategories.Any())
            {
                return Enumerable.Empty<Product>(); // Return an empty collection if no subcategories are found
            }

            // Fetch all products belonging to the fetched subcategories
            return await _context.Products
                .Where(p => subCategories.Contains(p.CategoryID)) // Filter by Category IDs (assuming products have a CategoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopRatedProductsAsync(int limit)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.ProductReviews)
                    .Where(p => p.ProductReviews.Any()) // Ensure there are reviews to calculate average
                    .OrderByDescending(p => p.ProductReviews.Average(r => r.Rating)) // Order by average rating
                    .Take(limit)
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving top-rated products.", ex);
            }
        }

        public async Task UpdateStockQuantityAsync(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Inventories) // Ensure that Inventories are loaded
                    .FirstOrDefaultAsync(p => p.ProductID == productId);

                if (product == null)
                {
                    throw new NotFoundException($"Product with ID {productId} not found.");
                }

                product.StockQuantity = product.Inventories?.Sum(i => i.Quantity) ?? 0;
                product.LastUpdatedDate = DateTime.UtcNow;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Updated Product stock quantity for {product.ProductName}.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating stock quantity.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsBySellerAsync(int sellerId)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.SellerID == sellerId)
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving products by seller.", ex);
            }
        }

        public async Task<List<ProductDto>> GetNewArrivalsAsync(int limit)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive) // Only include active products
                    .OrderByDescending(p => p.CreatedDate) // Order by creation date
                    .Include(p => p.ColorVariations) // Include color variations
                    .Take(limit) // Limit the number of results
                    .ToListAsync();

                // Map products to ViewModel
                var productViewModels = products.Select(p => new ProductDto
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    ImageURL = p.ImageURL,
                    ColorVariations = p.ColorVariations.Select(cv => new ColorVariationDto
                    {
                        ColorVariationID = cv.ColorVariationID,
                        ColorName = cv.ColorName,
                        ImageURL = cv.ImageURL
                    }).ToList()
                }).ToList();

                return productViewModels; // Return the list of ViewModels
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving new arrivals.", ex);
            }
        }

        public async Task<List<Discount>> GetTopDealsAsync(int limit)
        {
            try
            {
                return await _context.Discounts
                    .OrderByDescending(d => d.DiscountCode) // Adjust ordering as per your logic
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving top deals.", ex);
            }
        }

        // *** New Methods for Flash Sales ***

        public async Task<IEnumerable<ProductDealDto>> GetFlashSaleProductsAsync()
        {
            try
            {
                var currentUtcTime = DateTime.UtcNow;

                return await _context.Products
                    .Where(p => p.IsActive &&
                                p.DiscountedPrice.HasValue &&
                                p.FlashSaleStart <= currentUtcTime &&
                                p.FlashSaleEnd >= currentUtcTime)
                    .Select(p => new ProductDealDto
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName ?? string.Empty,
                        Description = p.Description,
                        Price = p.Price,
                        DiscountedPrice = p.DiscountedPrice,
                        StockQuantity = p.StockQuantity,
                        ImageURL = p.ImageURL,
                        FlashSaleEnd = p.FlashSaleEnd,
                        IsFlashSaleActive = p.IsFlashSaleActive, // Use the IsFlashSaleActive property
                        SellerID = p.SellerID,
                        SellerName = p.Seller != null ? p.Seller.ContactPerson : string.Empty, // Assuming Seller has a Name property

                        // Calculate average rating and review count
                        AverageRating = p.ProductReviews.Any()
                            ? Math.Round(p.ProductReviews.Average(r => r.Rating), 2)
                            : 0,
                        ReviewCount = p.ProductReviews.Count()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving flash sale products.", ex);
            }
        }

        public async Task AddFlashSaleAsync(int productId, decimal discountedPrice, DateTime flashSaleStart, DateTime flashSaleEnd)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null || !product.IsActive)
                {
                    throw new NotFoundException($"Active product with ID {productId} not found.");
                }

                product.DiscountedPrice = discountedPrice;
                product.FlashSaleStart = flashSaleStart;
                product.FlashSaleEnd = flashSaleEnd;
                product.LastUpdatedDate = DateTime.UtcNow;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Flash sale added to product {product.ProductName}.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding flash sale to product.", ex);
            }
        }

        public async Task UpdateFlashSaleAsync(int productId, decimal? discountedPrice, DateTime? flashSaleStart, DateTime? flashSaleEnd)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null || !product.IsActive)
                {
                    throw new NotFoundException($"Active product with ID {productId} not found.");
                }

                if (discountedPrice.HasValue)
                {
                    product.DiscountedPrice = discountedPrice.Value;
                }

                if (flashSaleStart.HasValue)
                {
                    product.FlashSaleStart = flashSaleStart.Value;
                }

                if (flashSaleEnd.HasValue)
                {
                    product.FlashSaleEnd = flashSaleEnd.Value;
                }

                product.LastUpdatedDate = DateTime.UtcNow;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Flash sale updated for product {product.ProductName}.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating flash sale for product.", ex);
            }
        }

        public async Task RemoveFlashSaleAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null || !product.IsActive)
                {
                    throw new NotFoundException($"Active product with ID {productId} not found.");
                }

                product.DiscountedPrice = null;
                product.FlashSaleStart = null;
                product.FlashSaleEnd = null;
                product.LastUpdatedDate = DateTime.UtcNow;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Flash sale removed from product {product.ProductName}.",
                        Timestamp = DateTime.UtcNow,
                        UserId = _user?.UserID ?? 0
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error removing flash sale from product.", ex);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetExploreProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ColorVariations) // Include color variations
                .Include(p => p.ProductReviews) // Include reviews
                .Select(p => new ProductDto
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountedPrice = p.DiscountedPrice,
                    StockQuantity = p.StockQuantity,
                    ImageURL = p.ImageURL,
                    CreatedDate = p.CreatedDate,
                    LastUpdatedDate = p.LastUpdatedDate,
                    IsActive = p.IsActive,
                    SellerID = p.SellerID,
                    CategoryID = p.CategoryID,
                    AverageRating = p.ProductReviews.Any() ? (decimal)p.ProductReviews.Average(r => r.Rating) : 0, // Calculate average rating
                    ReviewCount = p.ProductReviews.Count(), // Count of reviews
                    ColorVariations = p.ColorVariations.Select(cv => new ColorVariationDto
                    {
                        ColorVariationID = cv.ColorVariationID,
                        ColorName = cv.ColorName,
                        ImageURL = cv.ImageURL
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<List<Product>> GetSuggestedProducts()
        {
            // Fetch a larger number of products from the database
            var products = await _context.Products.ToListAsync(); // Get all products
            var random = new Random();

            // Shuffle the products and take the first 8
            return products.OrderBy(x => random.Next()).Take(8).ToList();
        }
    }
}
