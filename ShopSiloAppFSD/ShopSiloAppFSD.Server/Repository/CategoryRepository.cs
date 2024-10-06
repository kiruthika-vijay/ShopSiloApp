using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public CategoryRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        public async Task AddCategoryAsync(Category category)
        {
            try
            {
                // Check if the category already exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName && c.IsActive);

                if (existingCategory != null)
                {
                    throw new InvalidOperationException($"Category '{category.CategoryName}' already exists.");
                }

                // Get the role of the current user
                UserRole role = _user.Role; // Assuming _user object has the Role property

                // Admin adds category directly
                if (role == UserRole.Admin)
                {
                    category.Status = ApprovalStatus.Approved; // Directly approve if admin
                }
                // Seller requires approval from admin
                else if (role == UserRole.Seller)
                {
                    category.Status = ApprovalStatus.Pending; // Mark the category as pending
                }
                else
                {
                    throw new UnauthorizedAccessException("Only Admin or Seller can add categories.");
                }

                await _context.Categories.AddAsync(category);

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    // Create audit log entry
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"New Category '{category.CategoryName}' added by {_user.Role}. Status: {category.Status}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };

                    await _context.AuditLogs.AddAsync(auditLog);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Category already exists.", ex);
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(category.CategoryID);
                if (existingCategory == null)
                {
                    throw new NotFoundException($"Category with ID {category.CategoryID} not found.");
                }
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.ParentCategoryId = category.ParentCategoryId;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Category Updated from {existingCategory.CategoryName} to {category.CategoryName}.",
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
                throw new RepositoryException("Concurrency error occurred while updating category.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating category.", ex);
            }
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException($"Category with ID {categoryId} not found.");
                }

                category.IsActive = false;

                // Recursively set IsActive to false for all subcategories
                await SetSubcategoriesInactive(categoryId);

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Category {category.CategoryName} and their sub-categories set in-active.",
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
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting category.", ex);
            }
        }


        private async Task SetSubcategoriesInactive(int parentCategoryId)
        {
            // Find all subcategories of the given category
            var subcategories = await _context.Categories
                                              .Where(c => c.ParentCategoryId == parentCategoryId && c.IsActive)
                                              .ToListAsync();

            foreach (var subcategory in subcategories)
            {
                // Set the subcategory IsActive flag to false
                subcategory.IsActive = false;

                // Recursively call the function to deactivate subcategories of this subcategory
                await SetSubcategoriesInactive(subcategory.CategoryID);
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    throw new NotFoundException($"Category with ID {categoryId} not found.");
                }

                return category;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving category by ID.", ex);
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all categories.", ex);
            }
        }

        public async Task<IEnumerable<Category>> GetSubcategoriesAsync(int parentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .ToListAsync();
        }
    }
}
