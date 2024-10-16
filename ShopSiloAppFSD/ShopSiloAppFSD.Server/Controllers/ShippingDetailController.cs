using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopSiloAppFSD.DTO;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingDetailController : ControllerBase
    {
        private readonly IShippingDetailRepository _shippingDetailRepository;

        public ShippingDetailController(IShippingDetailRepository shippingDetailRepository)
        {
            _shippingDetailRepository = shippingDetailRepository;
        }

        // GET: api/ShippingDetails/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingDetail>> GetShippingDetailsById(int id)
        {
            try
            {
                var shippingDetail = await _shippingDetailRepository.GetShippingDetailsByIdAsync(id);
                if (shippingDetail == null)
                {
                    return NotFound(new { message = $"Shipping detail with ID {id} not found." });
                }
                return Ok(shippingDetail);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/ShippingDetails
        [HttpPost]
        public async Task<ActionResult> AddShippingDetails([FromBody] ShippingDetailDto shippingDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shippingDetail = new ShippingDetail
            {
                ShippingMethod = shippingDetailDto.ShippingMethod,
                ShippingStatus = shippingDetailDto.ShippingStatus,
                TrackingNumber = shippingDetailDto.TrackingNumber,
                OrderID = shippingDetailDto.OrderID
            };

            try
            {
                await _shippingDetailRepository.AddShippingDetailsAsync(shippingDetail);
                return CreatedAtAction(nameof(GetShippingDetailsById), new { id = shippingDetail.ShippingID }, shippingDetail);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/ShippingDetails/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShippingDetails(int id, [FromBody] ShippingDetailDto shippingDetailDto)
        {
            if (id != shippingDetailDto.ShippingID)
            {
                return BadRequest(new { message = "ID in URL does not match shipping detail ID." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shippingDetail = new ShippingDetail
            {
                ShippingMethod = shippingDetailDto.ShippingMethod,
                ShippingStatus = shippingDetailDto.ShippingStatus,
                TrackingNumber = shippingDetailDto.TrackingNumber,
                OrderID = shippingDetailDto.OrderID
            };

            try
            {
                await _shippingDetailRepository.UpdateShippingDetailsAsync(shippingDetail);
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

        // DELETE: api/ShippingDetails/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingDetails(int id)
        {
            try
            {
                await _shippingDetailRepository.DeleteShippingDetailsAsync(id);
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

        // PATCH: api/ShippingDetails/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateShippingStatus(int id, [FromBody] ShippingStatus status)
        {
            try
            {
                await _shippingDetailRepository.UpdateShippingStatusAsync(id, status);
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

        // GET: api/ShippingDetails/track/{trackingNumber}
        [HttpGet("track/{trackingNumber}")]
        public async Task<ActionResult<ShippingDetail>> TrackShipment(string trackingNumber)
        {
            try
            {
                var shippingDetail = await _shippingDetailRepository.TrackShipmentAsync(trackingNumber);
                if (shippingDetail == null)
                {
                    return NotFound(new { message = $"No shipment found for tracking number {trackingNumber}." });
                }
                return Ok(shippingDetail);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
