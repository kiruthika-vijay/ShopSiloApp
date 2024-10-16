using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using ShopSiloAppFSD.Server.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistsController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public WishlistsController(IWishlistRepository wishlistRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _wishlistRepository = wishlistRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WishlistItemDTO>>> GetAllWishlists()
        {
            var wishlists = await _wishlistRepository.GetAllWishlistsAsync();
            return Ok(wishlists);
        }

        // POST: api/Wishlists
        [HttpPost]
        public async Task<IActionResult> AddWishlist(WishlistDto wishlistDto)
        {

            try
            {
                var wishlist = new Wishlist
                {
                    UserID = wishlistDto.userID
                };
                await _wishlistRepository.CreateWishlistAsync(wishlist);
                return Ok();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Wishlist>> GetWishlistById(int id)
        {
            var wishlist = await _wishlistRepository.GetWishlistByIdAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }
            return Ok(wishlist);
        }

        [HttpDelete("{wishlistId}/items/{productId}")]
        public async Task<IActionResult> RemoveItemFromWishlist(int wishlistId, int productId)
        {
            await _wishlistRepository.RemoveItemFromWishlistAsync(wishlistId, productId);
            return NoContent();
        }

        [HttpDelete("{wishlistId}/clear")]
        public async Task<IActionResult> ClearWishlist(int wishlistId)
        {
            await _wishlistRepository.ClearWishlistAsync(wishlistId);
            return NoContent();
        }

        [HttpPost("{wishlistId}/move-to-bag")]
        public async Task<IActionResult> MoveAllItemsToBag(int wishlistId)
        {
            await _wishlistRepository.MoveAllItemsToBagAsync(wishlistId, _shoppingCartRepository);
            return NoContent();
        }

        // POST api/shoppingcart/add
        [HttpPost("add")]
        [Authorize(Roles = "Customer")] // Ensures that only authenticated users can add items to the cart
        public async Task<IActionResult> AddItemToWishlist([FromBody] WishlistItemAddDTO addToWishlistDto)
        {
            if (addToWishlistDto == null || addToWishlistDto.ProductID <= 0)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                await _wishlistRepository.AddItemToWishlistAsync(addToWishlistDto.ProductID);
                return Ok(new { Message = "Item added to wishlist successfully" });
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


    }
}
