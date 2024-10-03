using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize(Roles = "Seller, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ISellerRepository _sellerRepository;

        public SellerController(ISellerRepository sellerRepository)
        {
            _sellerRepository = sellerRepository;
        }

        // GET: api/Seller/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Seller>> GetSellerById(int id)
        {
            try
            {
                var seller = await _sellerRepository.GetSellerByIdAsync(id);
                if (seller == null)
                {
                    return NotFound(new { message = $"Seller with ID {id} not found." });
                }
                return Ok(seller);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Seller
        [HttpGet]
        public async Task<ActionResult<Seller>> GetSellerByLogged()
        {
            try
            {
                var seller = await _sellerRepository.GetSellerByLoggedAsync();
                if (seller == null)
                {
                    return NotFound(new { message = $"Seller not authenticated." });
                }
                return Ok(seller);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // GET: api/Seller/top/{limit}
        [HttpGet("top/{limit}")]
        public async Task<ActionResult<IEnumerable<Seller>>> GetTopSellers(int limit)
        {
            try
            {
                var sellers = await _sellerRepository.GetTopSellersAsync(limit);
                return Ok(sellers);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Seller/sales/{sellerId}/{startDate}/{endDate}
        [HttpGet("sales/{sellerId}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<SalesData>>> GetSalesDataByDateRange(int sellerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var salesData = await _sellerRepository.GetSalesDataByDateRangeAsync(sellerId, startDate, endDate);
                return Ok(salesData);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<Seller>> GetAllSellersAsync()
        {
            try
            {
                var seller = await _sellerRepository.GetAllSellersAsync();

                return Ok(seller);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Seller/products/{sellerId}
        [HttpGet("products/{sellerId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetSellerProducts(int sellerId)
        {
            try
            {
                var products = await _sellerRepository.GetSellerProductsAsync(sellerId);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        // GET: api/Seller/products
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLoggedSellerProducts()
        {
            try
            {
                var products = await _sellerRepository.GetLoggedSellerProductsAsync();
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Seller/orders/{sellerId}
        [HttpGet("orders/{sellerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetSellerOrders()
        {
            try
            {
                var orders = await _sellerRepository.GetSellerOrdersAsync();
                return Ok(orders);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Seller
        [HttpPost]
        public async Task<ActionResult<Seller>> AddSeller([FromBody] SellerDto sellerDto)
        {
            if (sellerDto == null)
            {
                return BadRequest(new { message = "Seller cannot be null." });
            }

            try
            {
                // Map DTO to entity
                var seller = new Seller
                {
                    SellerID = sellerDto.SellerID,
                    CompanyName = sellerDto.CompanyName,
                    ContactPerson = sellerDto.ContactPerson,
                    ContactNumber = sellerDto.ContactNumber,
                    Address = sellerDto.Address,
                    StoreDescription = sellerDto.StoreDescription
                };

                await _sellerRepository.AddSellerAsync(seller);

                // Map entity to DTO for response
                var resultDto = new SellerDto
                {
                    CompanyName = seller.CompanyName,
                    ContactPerson = seller.ContactPerson,
                    ContactNumber = seller.ContactNumber,
                    Address = seller.Address,
                    StoreDescription = seller.StoreDescription
                };

                return CreatedAtAction(nameof(GetSellerById), new { id = resultDto.SellerID }, resultDto);
            }

            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Seller/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeller(int id, [FromBody] SellerDto sellerDto)
        {
            if (id != sellerDto.SellerID)
            {
                return BadRequest(new { message = "ID in URL does not match seller ID." });
            }

            try
            {
                // Map DTO to entity
                var seller = new Seller
                {
                    SellerID = sellerDto.SellerID,
                    CompanyName = sellerDto.CompanyName,
                    ContactPerson = sellerDto.ContactPerson,
                    ContactNumber = sellerDto.ContactNumber,
                    Address = sellerDto.Address,
                    StoreDescription = sellerDto.StoreDescription
                };

                await _sellerRepository.UpdateSellerAsync(seller);
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/Seller/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(int id)
        {
            try
            {
                await _sellerRepository.DeleteSellerAsync(id);
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
