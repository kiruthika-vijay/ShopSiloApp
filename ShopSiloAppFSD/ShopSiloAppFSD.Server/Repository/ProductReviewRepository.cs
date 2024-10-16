using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
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
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public ProductReviewRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        public async Task AddReviewAsync(ProductReview review)
        {
            try
            {
                var user = _user.UserID;
                review.UserID = user;

                await _context.ProductReviews.AddAsync(review);
                // Save changes to populate the ReviewID
                await _context.SaveChangesAsync();

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == review.ProductID);
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"New Review added for product {product.ProductName}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding review.", ex);
            }
        }

        public async Task UpdateReviewAsync(ProductReview review)
        {
            try
            {
                var existingReview = await _context.ProductReviews.FindAsync(review.ReviewID);
                if (existingReview == null)
                {
                    throw new NotFoundException($"Review with ID {review.ReviewID} not found.");
                }

                existingReview.Rating = review.Rating;
                existingReview.ReviewText = review.ReviewText;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == review.ProductID);
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Review updated for product {product.ProductName}.",
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
                throw new RepositoryException("Concurrency error occurred while updating review.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating review.", ex);
            }
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            try
            {
                var review = await _context.ProductReviews.FindAsync(reviewId);
                if (review == null)
                {
                    throw new NotFoundException($"Review with ID {reviewId} not found.");
                }
                
                _context.ProductReviews.Remove(review);

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == review.ProductID);
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Review deleted for product {product.ProductName}. Review ID : {reviewId}",
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
                throw new RepositoryException("Error deleting review.", ex);
            }
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductAsync(int productId)
        {
            try
            {
                var reviews = await _context.ProductReviews
                    .Where(r => r.ProductID == productId)
                    .Select(r => new ProductReview
                    {
                        ReviewID = r.ReviewID,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        ReviewDate = r.ReviewDate,
                        UserID = r.UserID
                    })
                    .ToListAsync();

                return reviews;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving reviews by product.", ex);
            }
        }

        public async Task<IEnumerable<ProductReview>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _context.ProductReviews.ToListAsync();
                return reviews;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all reviews.", ex);
            }
        }

        public async Task<ProductReview> GetReviewByIdAsync(int reviewId)
        {
            try
            {
                var review = await _context.ProductReviews
                    .FirstOrDefaultAsync(r => r.ReviewID == reviewId);

                if (review == null)
                {
                    throw new NotFoundException($"Review with ID {reviewId} not found.");
                }

                return review;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving review by ID.", ex);
            }
        }
    }
}
