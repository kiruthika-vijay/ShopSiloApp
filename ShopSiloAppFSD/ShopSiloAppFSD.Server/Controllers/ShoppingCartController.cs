using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Repository;
using ShopSiloAppFSD.Server.DTO;
using System.Security.Claims;
using NuGet.Protocol.Core.Types;

namespace ShopSiloAppFSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        // GET: api/ShoppingCart/{cartId}
        [Authorize(Roles = "Customer")]
        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetCartByIdAsync(cartId);
                if (cart == null)
                {
                    return NotFound("Shopping cart not found.");
                }
                return Ok(cart);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/ShoppingCart/user/{userId}
        [Authorize(Roles = "Customer")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return NotFound("Shopping cart for user not found.");
                }
                return Ok(cart);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/ShoppingCart
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] ShoppingCartDto cartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to entity
            var cart = new ShoppingCart
            {
                UserID = cartDto.UserID
            };

            try
            {
                await _shoppingCartRepository.AddCartAsync(cart);
                return CreatedAtAction(nameof(GetCartById), new { cartId = cart.CartID }, cart);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/shoppingcart/add
        [HttpPost("add")]
        [Authorize(Roles = "Customer")] // Ensures that only authenticated users can add items to the cart
        public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (addToCartDto == null || addToCartDto.ProductId <= 0 || addToCartDto.Quantity <= 0)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                await _shoppingCartRepository.AddItemToCartAsync(addToCartDto.ProductId, addToCartDto.Quantity);
                return Ok(new { Message = "Item added to cart successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Generic 500 error for other unhandled exceptions
            }
        }


        // PUT: api/ShoppingCart/{cartId}
        [Authorize(Roles = "Customer")]
        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, [FromBody] ShoppingCartDto cartDto)
        {
            if (cartDto.CartID != cartId)
            {
                return BadRequest("Cart ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to entity
            var cart = new ShoppingCart
            {
                CartID = cartDto.CartID,
                UserID = cartDto.UserID
            };

            try
            {
                await _shoppingCartRepository.UpdateCartAsync(cart);
                return NoContent(); // Return 204 No Content on success
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/ShoppingCart/{cartId}
        [Authorize(Roles = "Customer")]
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            try
            {
                await _shoppingCartRepository.DeleteCartAsync(cartId);
                return NoContent(); // Return 204 No Content on success
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems()
        {
            var cartItems = await _shoppingCartRepository.GetCartItemsByLoggedUserAsync();
            return Ok(cartItems);
        }

        // DELETE: api/ShoppingCart/{cartId}/clear
        [Authorize(Roles = "Customer")]
        [HttpDelete("clear-cart")]
        public async Task<IActionResult> ClearCartOfLoggedUser()
        {
            try
            {
                await _shoppingCartRepository.ClearCartOfLoggedUserAsync();
                return NoContent(); // Return 204 No Content on success
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("cart/count")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<CountDto>> GetCartCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var count = await _shoppingCartRepository.GetCartCountAsync(userId);

            return Ok(new CountDto { Count = count });
        }

        [HttpGet("wishlist/count")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<CountDto>> GetWishlistCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var count = await _shoppingCartRepository.GetWishlistCountAsync(userId);

            return Ok(new CountDto { Count = count });
        }

        [HttpGet]
        [Route("GetCart")]
        public async Task<ActionResult<List<CartItemDTO>>> GetAllCartItems()
        {
            // Call the method to get all cart items for the logged-in user
            var cartItems = await _shoppingCartRepository.GetAllCartItemsAsync();

            if (cartItems == null || cartItems.Count == 0)
            {
                return NotFound("No items found in the shopping cart.");
            }

            return Ok(cartItems); // Return the cart items with a 200 status
        }
    }
}