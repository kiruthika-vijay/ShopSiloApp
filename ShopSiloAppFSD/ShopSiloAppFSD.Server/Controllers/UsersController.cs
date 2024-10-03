using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;  // Logger for logging

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users.");
                var users = await _userRepository.GetAllUsersAsync();
                return Ok(users);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error fetching users.");
                return StatusCode(500, "Internal server error occurred while retrieving users.");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID {UserId}", id);
                var user = await _userRepository.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found.", id);
                return NotFound(ex.Message);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error fetching user with ID {UserId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving the user.");
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] UserDto userDto)
        {
            User user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                Role = userDto.Role
            };

            try
            {
                _logger.LogInformation("Adding a new user.");
                await _userRepository.AddUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, user);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred while adding user.");
                return BadRequest(ex.Message);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error adding user.");
                return StatusCode(500, "Internal server error occurred while adding the user.");
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserDto userDto)
        {

            try
            {
                _logger.LogInformation("Updating user with ID {UserId}", id);
                var existingUser = await _userRepository.GetUserByIdAsync(id);

                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Manual mapping from UserDto to the existing User entity
                existingUser.Username = userDto.Username;
                existingUser.Password = userDto.Password;
                existingUser.Email = userDto.Email;
                existingUser.Role = userDto.Role;

                await _userRepository.UpdateUserAsync(existingUser);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found.", id);
                return NotFound(ex.Message);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", id);
                return StatusCode(500, "Internal server error occurred while updating the user.");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID {UserId}", id);
                await _userRepository.DeleteUserAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found.", id);
                return NotFound(ex.Message);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
                return StatusCode(500, "Internal server error occurred while deleting the user.");
            }
        }

        // POST: api/Users/authenticate
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateUser([FromQuery] string identifier, [FromQuery] string password)
        {
            try
            {
                _logger.LogInformation("Authenticating user.");
                var user = await _userRepository.AuthenticateUserAsync(identifier, password);
                if (user == null)
                {
                    throw new AuthenticationException("Invalid credentials.");
                }

                return Ok(user);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, "Authentication failed.");
                return Unauthorized(ex.Message);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error during user authentication.");
                return StatusCode(500, "Internal server error occurred during authentication.");
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Invalid email address." });
            }

            try
            {
                await _userRepository.ResetPasswordAsync(request.Email);
                return Ok(new { message = "Reset password link sent to your email." });
            }
            catch (NotFoundException)
            {
                // For security reasons, it's common not to reveal whether the email exists
                return Ok(new { message = "Reset password link sent to your email." });
            }
            catch (RepositoryException ex)
            {
                // Log the exception (not shown here)
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password/{token}")]
        public async Task<IActionResult> ResetPassword(string token, [FromBody] ResetPasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Invalid password." });
            }

            try
            {
                var user = await _userRepository.GetUserByResetTokenAsync(token);
                if (user == null || user.TokenExpiration < DateTime.Now)
                {
                    return BadRequest(new { message = "Invalid or expired token." });
                }

                if(request.Password == user.Password)
                {
                    return BadRequest(new { message = "The new password cannot be the same as the old password." });
                }

                await _userRepository.ChangePasswordAsync(user.UserID, request.Password);

                // Optionally, remove the reset token after successful password reset
                user.ResetToken = null;
                user.TokenExpiration = null;
                await _userRepository.UpdateUserAsync(user);

                return Ok(new { message = "Password reset successfully." });
            }
            catch (RepositoryException ex)
            {
                // Log the exception (not shown here)
                return StatusCode(500, new { message = "An error occurred while resetting your password." });
            }
        }

        // GET: api/Users/role/{role}
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(UserRole role)
        {
            try
            {
                _logger.LogInformation("Fetching users with role {Role}", role);
                var users = await _userRepository.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error fetching users by role.");
                return StatusCode(500, "Internal server error occurred while retrieving users by role.");
            }
        }

        // GET: api/Users/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation("Fetching users with email {Email}", email);
                var users = await _userRepository.GetUsersByEmailAsync(email);
                return Ok(users);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error fetching users by email.");
                return StatusCode(500, "Internal server error occurred while retrieving users by email.");
            }
        }

        [AllowAnonymous]
        // GET: api/Users/token/{token}
        [HttpGet("token/{token}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserByToken(string token)
        {
            try
            {
                _logger.LogInformation("Fetching users with token {token}", token);
                var users = await _userRepository.GetUserByResetTokenAsync(token);
                return Ok(users);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error fetching users by token.");
                return StatusCode(500, "Internal server error occurred while retrieving users by token.");
            }
        }

        [AllowAnonymous]
        // GET: api/Users/subscribe
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var isSubscribed = await _userRepository.SubscribeUserAsync(request.Email);

            if (!isSubscribed)
            {
                return StatusCode(500, "Failed to subscribe.");
            }

            return Ok("Subscription successful!");
        }

        [HttpPost("profile/change-password")]
        public async Task<IActionResult> ChangePasswordOfLoggedUserAsync(string newPassword)
        {
            try
            {
                await _userRepository.ChangePasswordOfLoggedUserAsync(newPassword);
                return Ok(new { message = "Password changed successfully." });
            }
            catch
            {
                return BadRequest("Unable to change the password");
            }
        }
    }
}