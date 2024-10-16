using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Razorpay.Api;
using ShopSiloAppFSD.Services;
using Order = ShopSiloAppFSD.Models.Order;
using ShopSiloAppFSD.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest(new { message = "Order cannot be null." });
            }

            try
            {
                // Manually map OrderDto to Order entity
                Order order = new Order
                {
                    TotalAmount = orderDto.TotalAmount,
                    UserID = orderDto.UserID,
                    SellerID = orderDto.SellerID,
                    ShippingAddressID = orderDto.ShippingAddressID,
                    BillingAddressID = orderDto.BillingAddressID,
                    TrackingNumber = orderDto.TrackingNumber,
                    DiscountID = orderDto.DiscountID
                };

                await _orderRepository.PlaceOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, orderDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            if (id != orderDto.OrderID)
            {
                return BadRequest(new { message = "ID in URL does not match the Order ID in the body." });
            }

            try
            {
                // Manually map OrderDto to Order entity
                var order = new Order
                {
                    OrderID = orderDto.OrderID,
                    TotalAmount = orderDto.TotalAmount,
                    UserID = orderDto.UserID,
                    SellerID = orderDto.SellerID,
                    ShippingAddressID = orderDto.ShippingAddressID,
                    BillingAddressID = orderDto.BillingAddressID,
                    TrackingNumber = orderDto.TrackingNumber,
                    DiscountID = orderDto.DiscountID
                };

                await _orderRepository.UpdateOrderAsync(order);
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

        // DELETE: api/Order/{id}

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                return Ok(order);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Order/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(int userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("SellerOrder/{sellerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetSellerOrders(int sellerId)
        {
            try
            {
                var orders = await _orderRepository.GetSellerOrdersAsync(sellerId);
                return Ok(orders);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Order/User/orders
        [HttpGet("User/orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByLoggedUser()
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByLoggedUserAsync();
                return Ok(orders);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Order/Status/{status}
        [HttpGet("Status/{status}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByStatus(string status)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByStatusAsync(status);
                return Ok(orders);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/Order/Cancel/{id}
        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                await _orderRepository.CancelOrderAsync(id);
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
        // Update tracking number
        [HttpPost("{id}/tracking")]
        public async Task<IActionResult> UpdateTrackingNumber(int id, [FromBody] string trackingNumber)
        {
            try
            {
                await _orderRepository.UpdateTrackingNumberAsync(id, trackingNumber);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, ex.Message);
            }
        }

        // Get tracking number
        [HttpGet("{id}/tracking")]
        public async Task<IActionResult> GetTrackingNumber(int id)
        {
            try
            {
                var trackingNumber = await _orderRepository.GetTrackingNumberAsync(id);
                if (trackingNumber == null)
                {
                    return NotFound();
                }
                return Ok(new { TrackingNumber = trackingNumber });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, ex.Message);
            }
        }
    }
}
