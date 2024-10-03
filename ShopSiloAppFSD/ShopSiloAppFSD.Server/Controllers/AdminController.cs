using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ShopSiloDBContext _context;
        private readonly IAdminRepository _adminRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly User _user;

        public AdminController(IAdminRepository adminRepository, IHttpContextAccessor httpContextAccessor, ShopSiloDBContext context)
        {
            _adminRepository = adminRepository;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // Generate a sales report between two dates
        [Authorize(Roles = "Admin")]
        [HttpGet("sales-report")]
        public async Task<IActionResult> GenerateSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var report = await _adminRepository.GenerateSalesReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("salesReport/byMonth")]
        public async Task<IActionResult> GenerateSalesReportByMonth()
        {
            try
            {
                var report = await _adminRepository.GenerateMonthlySalesReportByMonthAsync();
                return Ok(report);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Get top-selling products based on order item count
        [AllowAnonymous]
        [HttpGet("top-selling-products/chart")]
        public async Task<IActionResult> GetTopSellingProductsChart([FromQuery] int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Limit must be greater than zero.");
            }

            try
            {
                var products = await _adminRepository.GetTopSellingProductsChartAsync(limit);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("top-selling-products")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int limit)
        {
            if (limit <= 0)
            {
                return BadRequest("Limit must be greater than zero.");
            }

            try
            {
                var products = await _adminRepository.GetTopSellingProductsAsync(limit);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update-status/{categoryId}")]
        public async Task<IActionResult> UpdateCategoryStatus(int categoryId, ApprovalStatus approvalStatus)
        {
            if (!Enum.IsDefined(typeof(ApprovalStatus), approvalStatus))
            {
                return BadRequest("Invalid approval status.");
            }

            try
            {
                await _adminRepository.UpdateCategoryStatusAsync(categoryId, approvalStatus);
                return Ok(new { message = $"Category status updated to {approvalStatus}." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Only Admin can update category status.");
            }
            catch (RepositoryException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
