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
    public class ShippingAddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;

        public ShippingAddressController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        // GET: api/ShippingAddress/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddressById(int id)
        {
            try
            {
                var address = await _addressRepository.GetAddressByIdAsync(id);
                if (address == null)
                {
                    return NotFound(new { message = $"Address with ID {id} not found." });
                }
                return Ok(address);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/ShippingAddress/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByUserId(int userId)
        {
            try
            {
                var addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);
                return Ok(addresses);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("toggle/{addressId}")]
        public async Task<IActionResult> ToggleAddress(int addressId)
        {
            try
            {
                await _addressRepository.ToggleAddressStatusAsync(addressId);
                return NoContent(); // Return 204 No Content on success
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Error toggling address: " + ex.Message);
            }
        }

        [HttpPut("toggleBillingAddress/{addressId}")]
        public async Task<IActionResult> ToggleBillingAddress(int addressId)
        {
            try
            {
                await _addressRepository.ToggleBillingAddressStatusAsync(addressId);
                return NoContent(); // Return 204 No Content on success
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Error toggling billing address: " + ex.Message);
            }
        }

        [HttpPut("toggleShippingAddress/{addressId}")]
        public async Task<IActionResult> ToggleShippingAddress(int addressId)
        {
            try
            {
                await _addressRepository.ToggleShippingAddressStatusAsync(addressId);
                return NoContent(); // Return 204 No Content on success
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "Error toggling shipping address: " + ex.Message);
            }
        }

        // GET: api/ShippingAddress/user
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresByLoggedUserAsync()
        {
            try
            {
                var addresses = await _addressRepository.GetAddressesByLoggedUserAsync();
                return Ok(addresses);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/ShippingAddress/default/{userId}
        [HttpGet("default/{userId}")]
        public async Task<ActionResult<Address>> GetDefaultShippingAddress(int userId)
        {
            try
            {
                var address = await _addressRepository.GetDefaultShippingAddressAsync(userId);
                if (address == null)
                {
                    return NotFound(new { message = $"Default shipping address for user with ID {userId} not found." });
                }
                return Ok(address);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/ShippingAddress
        [HttpPost]
        public async Task<ActionResult<Address>> AddAddress([FromBody] AddressDto addressDto)
        {
            if (addressDto == null)
            {
                return BadRequest(new { message = "Address cannot be null." });
            }

            try
            {
                var address = new Address
                {
                    AddressLine1 = addressDto.AddressLine1,
                    AddressLine2 = addressDto.AddressLine2,
                    City = addressDto.City,
                    State = addressDto.State,
                    PostalCode = addressDto.PostalCode,
                    Country = addressDto.Country,
                    IsBillingAddress = addressDto.IsBillingAddress,
                    IsShippingAddress = addressDto.IsShippingAddress,
                    CustomerID = addressDto.CustomerID
                };

                var addedAddress = await _addressRepository.AddAddressAsync(address);

                var addedAddressDto = new AddressDto
                {
                    AddressLine1 = addedAddress.AddressLine1,
                    AddressLine2 = addedAddress.AddressLine2,
                    City = addedAddress.City,
                    State = addedAddress.State,
                    PostalCode = addedAddress.PostalCode,
                    Country = addedAddress.Country,
                    IsBillingAddress = addedAddress.IsBillingAddress,
                    IsShippingAddress = addedAddress.IsShippingAddress,
                    CustomerID = addedAddress.CustomerID
                };
                return CreatedAtAction(nameof(GetAddressById), new { id = addedAddressDto.AddressID }, addedAddressDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/ShippingAddress/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto addressDto)
        {
            if (id != addressDto.AddressID)
            {
                return BadRequest(new { message = "ID in URL does not match address ID." });
            }

            if (addressDto == null)
            {
                return BadRequest(new { message = "Address cannot be null." });
            }

            try
            {
                var address = new Address
                {
                    AddressID = addressDto.AddressID,
                    AddressLine1 = addressDto.AddressLine1,
                    AddressLine2 = addressDto.AddressLine2,
                    City = addressDto.City,
                    State = addressDto.State,
                    PostalCode = addressDto.PostalCode,
                    Country = addressDto.Country,
                    IsBillingAddress = addressDto.IsBillingAddress,
                    IsShippingAddress = addressDto.IsShippingAddress,
                    CustomerID = addressDto.CustomerID
                };

                await _addressRepository.UpdateAddressAsync(address);
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/ShippingAddress/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var deleted = await _addressRepository.DeleteAddressAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Address with ID {id} not found." });
                }
                return NoContent();
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
