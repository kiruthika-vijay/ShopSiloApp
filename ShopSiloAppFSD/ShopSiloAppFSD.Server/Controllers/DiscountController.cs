using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountController(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        [Authorize(Roles = "Seller, Admin")]
        // POST: api/Discount
        [HttpPost]
        public async Task<IActionResult> AddDiscount([FromBody] DiscountDto discountDto)
        {
            if (discountDto == null)
            {
                return BadRequest(new { message = "Discount cannot be null." });
            }

            try
            {
                // Manually map DiscountDto to Discount entity
                var discount = new Discount
                {
                    DiscountCode = discountDto.DiscountCode,
                    Description = discountDto.Description,
                    DiscountPercentage = discountDto.DiscountPercentage,
                    ExpiryDate = discountDto.ExpiryDate,
                    MinimumPurchaseAmount = discountDto.MinimumPurchaseAmount
                };

                await _discountRepository.AddDiscountAsync(discount);
                return CreatedAtAction(nameof(GetDiscountById), new { id = discount.DiscountID }, discountDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Seller, Admin")]
        // PUT: api/Discount
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount([FromBody] DiscountDto discountDto)
        {
            if (discountDto == null)
            {
                return BadRequest(new { message = "Discount cannot be null." });
            }

            try
            {
                // Manually map DiscountDto to Discount entity
                var discount = new Discount
                {
                    DiscountID = discountDto.DiscountID,
                    DiscountCode = discountDto.DiscountCode,
                    Description = discountDto.Description,
                    DiscountPercentage = discountDto.DiscountPercentage,
                    ExpiryDate = discountDto.ExpiryDate,
                    MinimumPurchaseAmount = discountDto.MinimumPurchaseAmount
                };

                await _discountRepository.UpdateDiscountAsync(discount);
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Seller, Admin")]
        // DELETE: api/Discount/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            try
            {
                await _discountRepository.DeleteDiscountAsync(id);
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

        // GET: api/Discount/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Discount>> GetDiscountById(int id)
        {
            try
            {
                var discount = await _discountRepository.GetDiscountByIdAsync(id);
                if (discount == null)
                {
                    return NotFound(new { message = "Discount not found." });
                }

                return Ok(discount);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("Apply")]
        public async Task<IActionResult> ApplyDiscount([FromQuery] string discountCode)
        {
            try
            {
                decimal discountPercentage = await _discountRepository.ApplyDiscountAsync(discountCode);
                return Ok(new
                {
                    DiscountPercentage = discountPercentage,
                    Message = "Coupon applied successfully!"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // GET: api/Discount/Validate
        [HttpGet("Validate")]
        public async Task<IActionResult> ValidateDiscount([FromQuery] string discountCode)
        {
            try
            {
                var isValid = await _discountRepository.ValidateDiscountAsync(discountCode);
                return Ok(new { IsValid = isValid });
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        
        [HttpGet("CartTotal")]
        public async Task<IActionResult> GetUserCartTotal([FromQuery] int userId)
        {
            try
            {
                var cartTotal = await _discountRepository.GetUserCartTotalAsync(userId);
                return Ok(cartTotal);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}