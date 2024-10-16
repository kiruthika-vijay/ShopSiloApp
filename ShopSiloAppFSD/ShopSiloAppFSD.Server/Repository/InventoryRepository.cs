using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Product = ShopSiloAppFSD.Models.Product;

namespace ShopSiloAppFSD.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IEmailNotificationService _emailService;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly IEmailServiceConfiguration _emailServiceConfig;
        private readonly string _userId;
        private readonly User _user;
        public InventoryRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IEmailNotificationService emailService, IAuditLogConfiguration auditLogConfig, IEmailServiceConfiguration emailServiceConfig, IProductRepository productRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _auditLogConfig = auditLogConfig;
            _emailServiceConfig = emailServiceConfig;
            _productRepository = productRepository; // Initialize here

            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(_userId))
            {
                _user = _context.Users.FirstOrDefault(u => u.Username == _userId);
            }

            // Ensure that _productRepository is properly initialized or mock it if needed
        }

        // Add Inventory
        public async Task AddInventoryAsync(Inventory inventory)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == inventory.ProductID);
            try
            {
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Inventory details added for product {inventory.ProductID} - {product.ProductName}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                // Update StockQuantity in Product table
                await _productRepository.UpdateStockQuantityAsync(inventory.ProductID);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding inventory.", ex);
            }
        }

        // Update Inventory
        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            try
            {
                if (inventory == null)
                {
                    throw new ArgumentNullException(nameof(inventory), "Inventory cannot be null.");
                }

                _context.Inventories.Update(inventory);
                await _context.SaveChangesAsync();

                if (_auditLogConfig != null && _auditLogConfig.IsAuditLogEnabled) // Check if audit log config is not null and audit log is enabled
                {
                    if (_user == null)
                    {
                        throw new InvalidOperationException("User context is not available.");
                    }

                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Inventory details updated for product {inventory.ProductID} - {(inventory.Product?.ProductName ?? "Unknown Product")}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                // Check if _productRepository is not null before calling it
                if (_productRepository != null)
                {
                    await _productRepository.UpdateStockQuantityAsync(inventory.ProductID);
                }
                else
                {
                    throw new InvalidOperationException("Product repository is not initialized.");
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating inventory.", ex);
            }
        }


        // Delete Inventory
        public async Task DeleteInventoryAsync(int inventoryId)
        {
            try
            {
                var inventory = await _context.Inventories
                    .Include(i => i.Product) // Include Product to avoid null reference on inventory.Product
                    .FirstOrDefaultAsync(i => i.InventoryID == inventoryId); // Use FirstOrDefaultAsync for clarity

                if (inventory != null)
                {
                    inventory.IsActive = false;

                    if (_auditLogConfig != null && _auditLogConfig.IsAuditLogEnabled) // Check if _auditLogConfig is not null and audit log is enabled
                    {
                        if (_user == null)
                        {
                            throw new InvalidOperationException("User context is not available.");
                        }

                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Inventory details set in-active for product {inventory.ProductID} - {(inventory.Product?.ProductName ?? "Unknown Product")}",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Inventory not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting inventory.", ex);
            }
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoriesBySellerAsync()
        {
            try
            {
                // Get the current user
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == _userId || u.Email == _userId);

                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }

                // Find the seller associated with the user
                var seller = await _context.Sellers
                    .FindAsync(user.UserID);

                if (seller == null)
                {
                    throw new NotFoundException("Seller not found.");
                }

                // Retrieve inventories for the seller
                var inventories = await _context.Inventories
                    .Include(i => i.Product) // Include related product data
                    .Where(i => i.Product.SellerID == seller.SellerID && i.IsActive) // Filter by seller ID and active status
                    .Select(i => new InventoryDto
                    {
                        InventoryID = i.InventoryID,
                        Quantity = i.Quantity,
                        ProductID = i.ProductID,
                        ProductName = i.Product.ProductName,
                        ProductImageUrl = i.Product.ImageURL, // Add Product Image URL
                        Category = i.Product.Category.CategoryName, // Add Product Category
                        Price = i.Product.Price, // Add Product Price
                        DateAdded = i.LastUpdatedDate, // Assuming you have a DateAdded field in Inventory
                        IsLowStock = i.Quantity < 20, // Set low stock threshold
                        IsActive = i.IsActive // Indicate if the inventory item is active
                    })
                    .ToListAsync();

                return inventories;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving inventories for the seller.", ex);
            }
        }


        // Get Inventory by Product ID
        public async Task<Inventory?> GetInventoryByProductIdAsync(int productId)
        {
            try
            {
                return await _context.Inventories
                                     .Include(i => i.Product)
                                     .FirstOrDefaultAsync(i => i.ProductID == productId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving inventory by product ID.", ex);
            }
        }

        // Restock Product
        public async Task RestockProductAsync(int productId, int quantity)
        {
            try
            {
                // Load the inventory along with the product to avoid null references
                var inventory = await _context.Inventories
                    .Include(i => i.Product) // Ensure Product is loaded
                    .FirstOrDefaultAsync(i => i.ProductID == productId);

                if (inventory != null)
                {
                    inventory.Quantity += quantity;
                    inventory.LastUpdatedDate = DateTime.Now;
                    _context.Inventories.Update(inventory);

                    if (_auditLogConfig != null && _auditLogConfig.IsAuditLogEnabled) // Check if audit log config is not null and enabled
                    {
                        if (_user == null)
                        {
                            throw new InvalidOperationException("User context is not available.");
                        }

                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Inventory details updated for product {inventory.ProductID} - {(inventory.Product?.ProductName ?? "Unknown Product")}",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Inventory not found for the specified product.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error restocking product.", ex);
            }
        }

        // Get Products with low stock and send an alert
        public async Task<IEnumerable<Product?>> GetLowStockProductsAsync(int threshold)
        {
            try
            {
                var lowStockProducts = await _context.Inventories
                    .Where(i => i.Quantity <= threshold)
                    .Select(i => i.Product)
                    .Distinct() // Ensure unique products
                    .ToListAsync();

                // Track notified sellers
                var notifiedSellers = new HashSet<int>();

                foreach (var product in lowStockProducts)
                {
                    if (!notifiedSellers.Contains(product.SellerID))
                    {
                        notifiedSellers.Add(product.SellerID);

                        var sellerEmail = await _context.Users
                        .Where(u => u.UserID == product.SellerID)
                        .Select(u => u.Email)
                        .FirstOrDefaultAsync();
                        if (_emailServiceConfig.IsEmailServiceEnabled)
                        {
                            if (!string.IsNullOrEmpty(sellerEmail))
                            {
                                var subject = "Alert: Low Inventory Stock";
                                var body = $"Dear Seller,<br/><br/>" +
                                           $"The stock for your product '{product.ProductName}' (ID: {product.ProductID}) is below the threshold. Please restock as soon as possible.<br/><br/>" +
                                           $"Best regards,<br/>Shop Silo Team";
                                _emailService.SendEmail(sellerEmail, subject, body);
                            }
                        }
                    }
                }

                return lowStockProducts;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving low stock products.", ex);
            }
        }
    }
}