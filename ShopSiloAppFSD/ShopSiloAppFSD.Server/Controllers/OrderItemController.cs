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
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        // POST: api/OrderItem
        [HttpPost]
        public async Task<IActionResult> AddOrderItem([FromBody] OrderItemDto orderItemDto)
        {
            if (orderItemDto == null)
            {
                return BadRequest(new { message = "OrderItem cannot be null." });
            }

            try
            {
                // Manually map OrderItemDto to OrderItem
                var orderItem = new OrderItem
                {
                    Quantity = orderItemDto.Quantity,
                    Price = orderItemDto.Price,
                    OrderID = orderItemDto.OrderID,
                    ProductID = orderItemDto.ProductID
                };

                await _orderItemRepository.AddOrderItemAsync(orderItem);
                return CreatedAtAction(nameof(GetOrderItemById), new { id = orderItem.OrderItemID }, orderItemDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/OrderItem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] OrderItemDto orderItemDto)
        {
            if (id != orderItemDto.OrderItemID)
            {
                return BadRequest(new { message = "ID in URL does not match the OrderItem ID in the body." });
            }

            try
            {
                // Manually map OrderItemDto to OrderItem
                var orderItem = new OrderItem
                {
                    OrderItemID = orderItemDto.OrderItemID,
                    Quantity = orderItemDto.Quantity,
                    Price = orderItemDto.Price,
                    OrderID = orderItemDto.OrderID,
                    ProductID = orderItemDto.ProductID
                };

                await _orderItemRepository.UpdateOrderItemAsync(orderItem);
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

        // DELETE: api/OrderItem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            try
            {
                await _orderItemRepository.DeleteOrderItemAsync(id);
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

        // GET: api/OrderItem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItemById(int id)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetOrderItemByIdAsync(id);
                if (orderItem == null)
                {
                    return NotFound(new { message = "OrderItem not found." });
                }

                return Ok(orderItem);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/OrderItem/Order/{orderId}
        [HttpGet("Order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItemsByOrderId(int orderId)
        {
            try
            {
                var orderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
                return Ok(orderItems);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
