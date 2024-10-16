using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IProductReviewRepository
    {
        Task AddReviewAsync(ProductReview review);
        Task UpdateReviewAsync(ProductReview review);
        Task DeleteReviewAsync(int reviewId);
        Task<IEnumerable<ProductReview>> GetReviewsByProductAsync(int productId);
        Task<IEnumerable<ProductReview>> GetAllReviewsAsync();
        Task<ProductReview> GetReviewByIdAsync(int reviewId);
    }
}
