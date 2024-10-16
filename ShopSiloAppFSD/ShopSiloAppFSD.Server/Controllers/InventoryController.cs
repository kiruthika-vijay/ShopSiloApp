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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpGet("seller")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoriesBySeller()
        {
            var inventories = await _inventoryRepository.GetInventoriesBySellerAsync();
            return Ok(inventories);
        }

        // POST: api/Inventory
        [HttpPost]
        public async Task<IActionResult> AddInventory([FromBody] InventoryDto inventoryDto)
        {
            if (inventoryDto == null)
            {
                return BadRequest(new { message = "Inventory cannot be null." });
            }

            try
            {
                // Manually map InventoryDto to Inventory entity
                var inventory = new Inventory
                {
                    Quantity = inventoryDto.Quantity,
                    ProductID = inventoryDto.ProductID
                };

                await _inventoryRepository.AddInventoryAsync(inventory);
                return CreatedAtAction(nameof(GetInventoryByProductId), new { productId = inventory.ProductID }, inventoryDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Inventory
        [HttpPut]
        public async Task<IActionResult> UpdateInventory([FromBody] InventoryDto inventoryDto)
        {
            if (inventoryDto == null)
            {
                return BadRequest(new { message = "Inventory cannot be null." });
            }

            try
            {
                // Manually map InventoryDto to Inventory entity
                var inventory = new Inventory
                {
                    InventoryID = inventoryDto.InventoryID,
                    Quantity = inventoryDto.Quantity,
                    ProductID = inventoryDto.ProductID,
                };

                await _inventoryRepository.UpdateInventoryAsync(inventory);
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/Inventory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                await _inventoryRepository.DeleteInventoryAsync(id);
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

        // GET: api/Inventory/Product/{productId}
        [HttpGet("Product/{productId}")]
        public async Task<ActionResult<Inventory>> GetInventoryByProductId(int productId)
        {
            try
            {
                var inventory = await _inventoryRepository.GetInventoryByProductIdAsync(productId);
                if (inventory == null)
                {
                    return NotFound(new { message = "Inventory not found for the specified product." });
                }

                return Ok(inventory);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Inventory/LowStock/{threshold}
        [HttpGet("LowStock/{threshold}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLowStockProducts(int threshold)
        {
            try
            {
                var products = await _inventoryRepository.GetLowStockProductsAsync(threshold);
                return Ok(products);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Inventory/Restock
        [HttpPost("Restock")]
        public async Task<IActionResult> RestockProduct([FromQuery] int productId, [FromQuery] int quantity)
        {
            try
            {
                await _inventoryRepository.RestockProductAsync(productId, quantity);
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
    }
}