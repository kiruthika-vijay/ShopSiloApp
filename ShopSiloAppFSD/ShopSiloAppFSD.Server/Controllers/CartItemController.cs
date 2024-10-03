using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        // Add a new cart item
        [HttpPost]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemDTO cartItemDto)
        {
            if (cartItemDto == null)
            {
                return BadRequest("Cart item cannot be null.");
            }

            try
            {
                // Map CartItemDto to CartItem entity
                var cartItem = new CartItem
                {
                    Quantity = cartItemDto.Quantity,
                    Price = cartItemDto.Price,
                    ProductID = cartItemDto.ProductID
                };

                await _cartItemRepository.AddCartItemAsync(cartItem);
                return CreatedAtAction(nameof(GetCartItemById), new { id = cartItem.CartItemID }, cartItemDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Update an existing cart item
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] CartItemDTO cartItemDto)
        {
            if (id != cartItemDto.CartItemID)
            {
                return BadRequest("Cart item ID mismatch.");
            }

            try
            {
                // Map CartItemDto to CartItem entity
                var cartItem = new CartItem
                {
                    CartItemID = cartItemDto.CartItemID,
                    Quantity = cartItemDto.Quantity,
                    Price = cartItemDto.Price,
                    ProductID = cartItemDto.ProductID
                };

                await _cartItemRepository.UpdateCartItemAsync(cartItem);
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("updateQuantity/{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] CartQuantityUpdateDto updateCartItemDto)
        {
            if (updateCartItemDto == null || updateCartItemDto.quantity <= 0)
            {
                return BadRequest("Invalid quantity value.");
            }

            var updatedItem = await _cartItemRepository.UpdateCartItemQuantityAsync(cartItemId, updateCartItemDto.quantity);

            if (updatedItem == null)
            {
                return NotFound("Cart item not found.");
            }

            return Ok(updatedItem);
        }

        // Delete a cart item by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            try
            {
                bool result = await _cartItemRepository.DeleteCartItemAsync(id);
                if (result)
                {
                    return NoContent(); // Successfully deleted
                }
                return NotFound(new { message = "Cart item not found." });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // Get cart items by Cart ID
        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetCartItemsByCartId(int cartId)
        {
            try
            {
                var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);
                return Ok(cartItems);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Get a specific cart item by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartItemById(int id)
        {
            try
            {
                var cartItem = await _cartItemRepository.GetCartItemByIdAsync(id);
                if (cartItem != null)
                {
                    return Ok(cartItem);
                }
                return NotFound(new { message = "Cart item not found." });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
