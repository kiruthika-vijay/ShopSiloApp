using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Repository;
using System.Security.Claims;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize(Roles = "Seller, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SellerDashboardController : ControllerBase
    {
        private readonly ISellerDashboardRepository _dashboardRepository;
        private readonly DashboardService _dashboardService;
        private readonly ShopSiloDBContext _context;

        public SellerDashboardController(ISellerDashboardRepository dashboardRepository, DashboardService dashboardService, ShopSiloDBContext context)
        {
            _dashboardRepository = dashboardRepository;
            _dashboardService = dashboardService;
            _context = context;
        }

        // GET: api/SellerDashboard/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SellerDashboard>> GetDashboardById(int id)
        {
            try
            {
                var dashboard = await _dashboardRepository.GetSellerDashboardByIdAsync(id);
                return Ok(dashboard);
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

        // GET: api/SellerDashboard
        [HttpGet]
        public async Task<ActionResult<SellerDashboard>> GetDashboardByLoggedUser()
        {
            try
            {
                var dashboard = await _dashboardRepository.GetSellerDashboardByLoggedUserAsync();
                return Ok(dashboard);
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

        [HttpPut("update/{dashboardId}")]
        public async Task<IActionResult> UpdateDashboard(int dashboardId)
        {
            try
            {
                // Optionally, you can fetch the new dashboard data from the request body or other source
                // For this example, we assume you want to update metrics dynamically

                await _dashboardService.UpdateDashboardAsync(dashboardId);
                return Ok(new { Message = "Dashboard updated successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        // GET: api/salesdata
        [HttpGet("salesdata")]
        public async Task<ActionResult<IEnumerable<SalesData>>> GetSalesData()
        {
            return await _context.SalesData.Include(sd => sd.Order).ToListAsync();
        }

        // You can also create a method to fetch aggregated data for graph plotting
        // Example: Get total sales amount by date
        [HttpGet("aggregate")]
        public async Task<ActionResult<IEnumerable<object>>> GetSalesDataAggregate()
        {
            var salesData = await _context.SalesData
                .GroupBy(sd => sd.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(sd => sd.TotalAmount)
                })
                .ToListAsync();

            return Ok(salesData);
        }

        // Get all products for the logged-in seller
        // GET: api/seller/products
        [HttpGet("products")]
        public async Task<IActionResult> GetAllSellingProducts()
        {
            try
            {
                // Call the service to get all products sold by the logged-in seller
                var products = await _dashboardRepository.GetAllSellingProductsAsync();

                if (products == null || !products.Any())
                {
                    return NotFound("No products found for this seller.");
                }

                return Ok(products); // Return 200 OK with the products
            }
            catch (RepositoryException ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Return 500 Internal Server Error
            }
        }

        // Get a single product for the logged-in seller
        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {

            var product = await _dashboardRepository.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                AverageRating = product.ProductReviews.Any() ? (decimal)product.ProductReviews.Average(r => r.Rating) : 0,
            };

            return Ok(productDto);
        }

        // Get filtered products for the logged-in seller
        [HttpGet("products/filter")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFilteredProducts([FromQuery] string? productName, [FromQuery] string? categoryName, [FromQuery] int? rating)
        {
            var products = await _dashboardRepository.GetFilteredProductsAsync(productName, categoryName, rating);

            var productDtos = products.Select(p => new ProductDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                CategoryID = p.CategoryID,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                AverageRating = p.ProductReviews.Any() ? (decimal)p.ProductReviews.Average(r => r.Rating) : 0,
            });

            return Ok(productDtos);
        }

        // Method to get total orders for a specific product
        [HttpGet("products/{id}/totalorders")]
        public async Task<ActionResult<int>> GetTotalOrdersPerProduct(int id)
        {
            try
            {
                var totalOrders = await _dashboardRepository.GetTotalOrdersPerProductAsync(id);

                if (totalOrders == 0)
                {
                    return NotFound(new { message = "No orders found for this product." });
                }

                return Ok(totalOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("products/names")]
        public async Task<IActionResult> GetProductNames()
        {
            var productNames = await _dashboardRepository.GetProductNamesBySellerAsync();

            if (productNames == null || !productNames.Any())
            {
                return NotFound("No products found for this seller.");
            }

            return Ok(productNames);
        }
    }
}
