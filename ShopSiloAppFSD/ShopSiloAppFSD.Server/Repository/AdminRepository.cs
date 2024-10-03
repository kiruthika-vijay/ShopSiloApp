using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using ShopSiloAppFSD.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public AdminRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        // Generate a sales report between two dates
        public async Task<IEnumerable<Order>> GenerateSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error generating sales report.", ex);
            }
        }

        public async Task<IEnumerable<MonthlySalesReport>> GenerateMonthlySalesReportByMonthAsync()
        {
            try
            {
                var monthlySalesReport = await _context.Orders
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new MonthlySalesReport
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalSales = g.Sum(o => o.TotalAmount),

                    })
                    .ToListAsync();

                return monthlySalesReport;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error generating monthly sales report.", ex);
            }
        }

        // Get top-selling products based on order item count
        public async Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsChartAsync(int limit)
        {
            try
            {
                return await _context.OrderItems
                    .GroupBy(oi => oi.ProductID)
                    .OrderByDescending(g => g.Sum(oi => oi.Quantity)) // Order by total quantity sold
                    .Take(limit)
                    .Select(g => new TopSellingProductDto
                    {

                        ProductName = _context.Products
                                       .Where(p => p.ProductID == g.Key)
                                       .Select(p => p.ProductName)
                                       .FirstOrDefault(), // Fetch the product name
                        Quantity = g.Sum(oi => oi.Quantity), // Total quantity sold

                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving top-selling products.", ex);
            }
        }

        public async Task<IEnumerable<ProductWithReviewsDto>> GetTopSellingProductsAsync(int limit)
        {
            try
            {
                return await _context.OrderItems
                    .GroupBy(oi => oi.ProductID)
                    .OrderByDescending(g => g.Count())
                    .Take(limit)
                    .Select(g => new ProductWithReviewsDto
                    {
                        Product = _context.Products
                            .Include(p => p.ProductReviews) // Include reviews here
                            .FirstOrDefault(p => p.ProductID == g.Key),
                        AverageRating = _context.ProductReviews
                            .Where(r => r.ProductID == g.Key)
                            .Average(r => (double?)r.Rating) ?? 0,
                        ReviewCount = _context.ProductReviews
                            .Count(r => r.ProductID == g.Key)
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving top-selling products.", ex);
            }
        }

        public async Task UpdateCategoryStatusAsync(int categoryId, ApprovalStatus newStatus)
        {
            try
            {
                // Get the role of the current user
                UserRole role = _user.Role;

                if (role != UserRole.Admin)
                {
                    throw new UnauthorizedAccessException("Only Admin can update category status.");
                }

                // Find the category by its ID
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    throw new RepositoryException("Category not found.");
                }

                // Check if it's in a Pending state
                if (category.Status != ApprovalStatus.Pending)
                {
                    throw new InvalidOperationException("Category is not pending approval.");
                }

                // Update the category status
                category.Status = newStatus;

                // Create audit log entry
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    string action = newStatus == ApprovalStatus.Approved
                        ? $"Category '{category.CategoryName}' approved by admin."
                        : $"Category '{category.CategoryName}' rejected by admin.";

                    AuditLog auditLog = new AuditLog
                    {
                        Action = action,
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating category status.", ex);
            }
        }
    }
}
