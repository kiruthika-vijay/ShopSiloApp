using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewRepository _reviewRepository;

        public ProductReviewController(IProductReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // POST: api/ProductReview
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ProductReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest(new { message = "Review cannot be null." });
            }

            try
            {
                // Manually map from ProductReviewDto to ProductReview
                var review = new ProductReview
                {
                    Rating = reviewDto.Rating,
                    ReviewText = reviewDto.ReviewText,
                    ReviewDate = reviewDto.ReviewDate,
                    ProductID = reviewDto.ProductID,
                    UserID = reviewDto.UserID
                };

                await _reviewRepository.AddReviewAsync(review);

                // Optionally, map back to DTO to return
                var createdReviewDto = new ProductReviewDto
                {
                    ReviewID = review.ReviewID,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReviewDate = review.ReviewDate,
                    ProductID = review.ProductID,
                    UserID = review.UserID
                };

                return CreatedAtAction(nameof(GetReviewById), new { id = review.ReviewID }, createdReviewDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/ProductReview/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ProductReviewDto reviewDto)
        {
            if (id != reviewDto.ReviewID)
            {
                return BadRequest(new { message = "ID in URL does not match the Review ID in the body." });
            }

            try
            {
                // Manually map from ProductReviewDto to ProductReview
                var review = new ProductReview
                {
                    ReviewID = reviewDto.ReviewID,
                    Rating = reviewDto.Rating,
                    ReviewText = reviewDto.ReviewText,
                    ReviewDate = reviewDto.ReviewDate,
                    ProductID = reviewDto.ProductID,
                    UserID = reviewDto.UserID
                };

                await _reviewRepository.UpdateReviewAsync(review);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/ProductReview/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewRepository.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/ProductReview/Product/{productId}
        [HttpGet("Product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductReviewDto>>> GetReviewsByProduct(int productId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByProductAsync(productId);
                if (reviews == null || !reviews.Any())
                {
                    return NotFound(new { message = "No reviews found for this product." });
                }

                return Ok(new { reviews }); // Ensure your response structure matches what your frontend expects
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/ProductReview/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReviewDto>> GetReviewById(int id)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByIdAsync(id);
                return Ok(review);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
