using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;
using System.Linq;
using ShopSiloAppFSD.Server.DTO;
using iText.Commons.Actions.Data;
using ShopSiloAppFSD.Server.Services;

namespace ShopSiloAppFSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductController(IProductRepository productRepository, ICloudinaryService cloudinaryService)
        {
            _productRepository = productRepository;
            _cloudinaryService = cloudinaryService;
        }

        // Existing Endpoints...

        // POST: api/Product
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] UploadProductDto productDto)
        {
            if(productDto.Image == null)
                return BadRequest("Image is required.");

            if (productDto == null)
            {
                return BadRequest(new { message = "Product cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (productDto.Image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(productDto.Image);
                    productDto.ImageURL = imageUrl; // Ensure the URL is set
                }

                var product = new Product
                {
                    ProductName = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    StockQuantity = productDto.StockQuantity,
                    ImageURL = productDto.ImageURL,
                    SellerID = productDto.SellerID,
                    CategoryID = productDto.CategoryID
                };

                await _productRepository.AddProductAsync(product);
                return CreatedAtAction(nameof(GetProductById), new { id = product.ProductID }, product);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework)
                return StatusCode(500, new { message = "An error occurred while adding the product." });
            }
        }

        [HttpPut("toggle/{productId}")]
        public async Task<IActionResult> ToggleProduct(int productId)
        {
            try
            {
                await _productRepository.ToggleProductStatusAsync(productId);
                return NoContent(); // Return 204 No Content on success
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Error toggling address: " + ex.Message);
            }
        }

        // PUT: api/Product/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto productDto)
        {
            try
            {
                if (id != productDto.ProductID)
                {
                    return BadRequest("Product ID mismatch.");
                }

                // Fetch the existing product from the database
                var existingProduct = await _productRepository.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound("Product not found.");
                }

                // **Handle Image Update**
                if (productDto.Image != null)
                {
                    // If an existing image is present and it is hosted on Cloudinary, remove it
                    if (!string.IsNullOrEmpty(existingProduct.ImageURL) && existingProduct.ImageURL.Contains("cloudinary"))
                    {
                        var publicId = _cloudinaryService.GetPublicIdFromUrl(existingProduct.ImageURL); // Extract the public ID
                        await _cloudinaryService.DeleteImage(publicId); // Delete the existing image from Cloudinary
                    }

                    // Upload the new image to Cloudinary
                    var uploadResult = await _cloudinaryService.UploadImageAsync(productDto.Image);
                    if (uploadResult == null)
                    {
                        return BadRequest("Image upload failed.");
                    }

                    // Set the new image URL in the database
                    existingProduct.ImageURL = uploadResult;
                }
                else if (productDto.RemoveImage)
                {
                    // **Handle Image Removal**: If user opts to remove the image
                    if (!string.IsNullOrEmpty(existingProduct.ImageURL) && existingProduct.ImageURL.Contains("cloudinary"))
                    {
                        var publicId = _cloudinaryService.GetPublicIdFromUrl(existingProduct.ImageURL); // Extract the public ID
                        await _cloudinaryService.DeleteImage(publicId); // Delete the image from Cloudinary
                    }
                    existingProduct.ImageURL = null; // Set the image URL to null
                }

                // **No New Image Upload**: If no image is uploaded and the image is not removed, retain the existing image URL
                // (This is implicit: you don't change the `ImageURL` if neither of the above conditions are true)

                // **Update Other Product Details**
                existingProduct.ProductName = productDto.ProductName;
                existingProduct.Description = productDto.Description;
                existingProduct.Price = productDto.Price;
                // Add any other fields you want to update here

                // **Save changes to the repository**
                await _productRepository.UpdateProductAsync(existingProduct);

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the product: {ex.Message}");
            }
        }


        // DELETE: api/Product/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productRepository.DeleteProductAsync(id);
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

        // GET: api/Product/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                return Ok(product);
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

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDisplayDto>>> GetAllProducts()
        {
            try
            {
                // Fetch all products from the repository, including category information
                var products = await _productRepository.GetAllProductsAsync();

                // Manually map the products to ProductDTO, including category information
                var productDtos = products.Select(product => new ProductDisplayDto
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountedPrice = product.DiscountedPrice, // Optional
                    ImageURL = product.ImageURL,
                    StockQuantity = product.StockQuantity,
                    CategoryID = product.Category.CategoryID, // Assuming Product has a Category property
                    CategoryName = product.Category.CategoryName // Fetch CategoryName
                }).ToList();

                return Ok(productDtos);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // GET: api/Product/Search/{searchTerm}
        [AllowAnonymous]
        [HttpGet("Search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string searchTerm)
        {
            try
            {
                var products = await _productRepository.SearchProductsAsync(searchTerm);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Product/Category/{categoryId?}/{categoryName?}
        [Authorize]
        [HttpGet("Category/{categoryId?}/{categoryName?}")]
        public async Task<ActionResult<IEnumerable<ProductDisplayDto>>> GetProductsByCategory(int? categoryId, string categoryName)
        {
            try
            {
                var products = await _productRepository.GetProductsByCategoryAsync(categoryId, categoryName);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Category/{parentCategoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int parentCategoryId)
        {
            var products = await _productRepository.GetProductsByParentCategoryIdAsync(parentCategoryId);
            if (products == null || !products.Any())
            {
                return NotFound("No products found for this category.");
            }
            return Ok(products);
        }

        // GET: api/Product/TopRated/{limit}
        [AllowAnonymous]
        [HttpGet("TopRated/{limit}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTopRatedProducts(int limit)
        {
            try
            {
                var products = await _productRepository.GetTopRatedProductsAsync(limit);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Product/UpdateStock/{id}
        [Authorize]
        [HttpPut("UpdateStock/{id}")]
        public async Task<IActionResult> UpdateStockQuantity(int id)
        {
            try
            {
                await _productRepository.UpdateStockQuantityAsync(id);
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

        // GET: api/Product/Seller/{sellerId}
        [Authorize]
        [HttpGet("Seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsBySeller(int sellerId)
        {
            try
            {
                var products = await _productRepository.GetProductsBySellerAsync(sellerId);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Product/deals
        [AllowAnonymous]
        [HttpGet("deals")]
        public async Task<IActionResult> GetDeals([FromQuery] int limit = 4)
        {
            try
            {
                var deals = await _productRepository.GetTopDealsAsync(limit);
                return Ok(deals);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Product/new-arrivals
        [AllowAnonymous]
        [HttpGet("new-arrivals")]
        public async Task<IActionResult> GetNewArrivals([FromQuery] int limit = 4)
        {
            try
            {
                var newArrivals = await _productRepository.GetNewArrivalsAsync(limit);
                return Ok(newArrivals);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // *** New Flash Sale Endpoints ***

        // GET: api/Product/flashsale
        [AllowAnonymous]
        [HttpGet("flashsales")]
        public async Task<ActionResult<IEnumerable<ProductDealDto>>> GetFlashSaleProducts()
        {
            var products = await _productRepository.GetFlashSaleProductsAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No flash sale products available.");
            }

            return Ok(products);
        }

        // POST: api/Product/{id}/flashsale
        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("{id}/flashsale")]
        public async Task<IActionResult> AddFlashSale(int id, [FromBody] FlashSaleDto flashSaleDto)
        {
            if (flashSaleDto == null)
            {
                return BadRequest(new { message = "Flash sale data cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate that FlashSaleStart is before FlashSaleEnd
            if (flashSaleDto.FlashSaleStart >= flashSaleDto.FlashSaleEnd)
            {
                return BadRequest(new { message = "FlashSaleStart must be earlier than FlashSaleEnd." });
            }

            try
            {
                await _productRepository.AddFlashSaleAsync(
                    id,
                    flashSaleDto.DiscountedPrice,
                    flashSaleDto.FlashSaleStart,
                    flashSaleDto.FlashSaleEnd
                );

                return Ok(new { message = "Flash sale added successfully." });
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

        // PUT: api/Product/{id}/flashsale
        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("{id}/flashsale")]
        public async Task<IActionResult> UpdateFlashSale(int id, [FromBody] FlashSaleUpdateDto flashSaleUpdateDto)
        {
            if (flashSaleUpdateDto == null)
            {
                return BadRequest(new { message = "Flash sale update data cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // If both FlashSaleStart and FlashSaleEnd are provided, validate the order
            if (flashSaleUpdateDto.FlashSaleStart.HasValue && flashSaleUpdateDto.FlashSaleEnd.HasValue)
            {
                if (flashSaleUpdateDto.FlashSaleStart.Value >= flashSaleUpdateDto.FlashSaleEnd.Value)
                {
                    return BadRequest(new { message = "FlashSaleStart must be earlier than FlashSaleEnd." });
                }
            }

            try
            {
                await _productRepository.UpdateFlashSaleAsync(
                    id,
                    flashSaleUpdateDto.DiscountedPrice,
                    flashSaleUpdateDto.FlashSaleStart,
                    flashSaleUpdateDto.FlashSaleEnd
                );

                return Ok(new { message = "Flash sale updated successfully." });
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

        // DELETE: api/Product/{id}/flashsale
        [Authorize(Roles = "Admin,Seller")]
        [HttpDelete("{id}/flashsale")]
        public async Task<IActionResult> RemoveFlashSale(int id)
        {
            try
            {
                await _productRepository.RemoveFlashSaleAsync(id);
                return Ok(new { message = "Flash sale removed successfully." });
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

        [AllowAnonymous]
        [HttpGet("explore")]
        public async Task<ActionResult<IEnumerable<Product>>> GetExploreProducts()
        {
            var products = await _productRepository.GetExploreProductsAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }

        [HttpGet("suggestions")]
        public async Task<ActionResult<List<Product>>> GetSuggestions()
        {
            var suggestedProducts = await _productRepository.GetSuggestedProducts();
            return Ok(suggestedProducts);
        }
    }
}
