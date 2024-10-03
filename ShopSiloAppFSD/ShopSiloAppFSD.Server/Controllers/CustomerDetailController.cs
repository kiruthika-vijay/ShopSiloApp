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
    public class CustomerDetailController : ControllerBase
    {
        private readonly ICustomerDetailsRepository _customerDetailsRepository;

        public CustomerDetailController(ICustomerDetailsRepository customerDetailsRepository)
        {
            _customerDetailsRepository = customerDetailsRepository;
        }

        // POST: api/CustomerDetails
        [HttpPost]
        public async Task<IActionResult> AddCustomerDetails([FromBody] CustomerDetailDto customerDetailDto)
        {
            if (customerDetailDto == null)
            {
                return BadRequest(new { message = "Customer details cannot be null." });
            }

            try
            {
                // Manually map CustomerDetailDto to CustomerDetail entity
                var customerDetail = new CustomerDetail
                {
                    CustomerID = customerDetailDto.CustomerID,
                    FirstName = customerDetailDto.FirstName,
                    LastName = customerDetailDto.LastName,
                    PhoneNumber = customerDetailDto.PhoneNumber
                };

                await _customerDetailsRepository.AddCustomerDetailsAsync(customerDetail);
                return CreatedAtAction(nameof(GetCustomerDetailsById), new { id = customerDetail.CustomerID }, customerDetailDto);
            }
            catch (RepositoryException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/CustomerDetails
        [HttpPut]
        public async Task<IActionResult> UpdateCustomerDetails([FromBody] CustomerDetailDto customerDetailDto)
        {
             if (customerDetailDto == null)
            {
                return BadRequest(new { message = "Customer details cannot be null." });
            }

            try
            {
                // Manually map CustomerDetailDto to CustomerDetail entity
                var customerDetail = new CustomerDetail
                {
                    CustomerID = customerDetailDto.CustomerID,
                    FirstName = customerDetailDto.FirstName,
                    LastName = customerDetailDto.LastName,
                    PhoneNumber = customerDetailDto.PhoneNumber
                };

                await _customerDetailsRepository.UpdateCustomerDetailsAsync(customerDetail);
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

        // GET: api/CustomerDetails/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDetail>> GetCustomerDetailsById(int id)
        {
            try
            {
                var customerDetail = await _customerDetailsRepository.GetCustomerDetailsByIdAsync(id);
                return Ok(customerDetail);
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
        
        // GET: api/CustomerDetails/
        [HttpGet("details")]
        public async Task<ActionResult<CustomerDetail>> GetLoggedCustomerDetails()
        {
            try
            {
                var customerDetail = await _customerDetailsRepository.GetCustomerDetailsOfLoggedUser();
                return Ok(customerDetail);
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

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CustomerDetail>>> GetCustomerDetails()
        {
            try
            {
                var customerDetail = await _customerDetailsRepository.GetCustomerDetails();
                return Ok(customerDetail);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerDetails(int id)
        {
            try
            {
                // Call the repository method to delete the customer detail by ID
                await _customerDetailsRepository.DeleteCustomerDetailAsync(id);

                // Return NoContent if the operation was successful
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                // Return NotFound if the customer was not found
                return NotFound(new { message = ex.Message });
            }
            catch (RepositoryException ex)
            {
                // Return a server error if there's an issue with the repository
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
