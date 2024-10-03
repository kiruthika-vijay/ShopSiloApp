using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ShopSiloAppFSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AauthController : ControllerBase
    {
        IConfiguration _config;
        private readonly IUserRepository _userRepository;

        private readonly ShopSiloDBContext _context;
        public AauthController(IConfiguration configuration, ShopSiloDBContext context, IUserRepository userRepository)
        {
            this._config = configuration;
            _context = context;
            _userRepository = userRepository;
        }

        [NonAction]
        public User Validate(string identifier, string password)
        {

            User s = _context.Users
                .FirstOrDefault(i => (i.Email == identifier || i.Username == identifier) && i.Password == password);

            return s;

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            IActionResult response = Unauthorized();

            var s = Validate(request.Email, request.Password);
            if (s != null)
            {

                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Audience"];
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var signingCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha512Signature);

                var subject = new ClaimsIdentity(new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, s.Username),
                    new Claim(JwtRegisteredClaimNames.Email,s.Email),
                    new Claim(ClaimTypes.Role, s.Role.ToString()) // Assign role to the token
                    });

                var expires = DateTime.UtcNow.AddMinutes(30);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = signingCredentials
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return Ok(jwtToken);

            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("customer-register")]
        public IActionResult CustomerRegister([FromBody] RegisterRequestDto request)
        {
            // Validate the incoming request
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Invalid user data.");
            }

            // Check if the user already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email || u.Username == request.Username);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            // Create a new user
            var newUser = new User
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password, // No hashing implemented here
                Role = UserRole.Customer // Default role
            };

            // Add the new user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Login), new { email = newUser.Email }, newUser);
        }

        [AllowAnonymous]
        [HttpPost("seller-register")]
        public IActionResult SellerRegister([FromBody] RegisterRequestDto request)
        {
            // Validate the incoming request
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Invalid user data.");
            }

            // Check if the user already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email || u.Username == request.Username);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            // Create a new user
            var newUser = new User
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password, // No hashing implemented here
                Role = UserRole.Seller // Default role
            };

            // Add the new user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Login), new { email = newUser.Email }, newUser);
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
                // For security reasons, respond with the same message even if the user is not found
                return Ok(new { message = "Reset password link sent to your email." });
            }
            catch (RepositoryException)
            {
                // Log the exception details (not shown here)
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Reset Password Endpoint
        /// </summary>
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
                Console.WriteLine($"Received reset password request for token: {token}");

                var user = await _userRepository.GetUserByResetTokenAsync(token);
                if (user == null || user.TokenExpiration < DateTime.Now)
                {
                    Console.WriteLine($"Invalid or expired token: {token}");
                    return BadRequest(new { message = "Invalid or expired token." });
                }

                if (request.Password == user.Password)
                {
                    return BadRequest(new { message = "The new password cannot be the same as the old password." });
                }

                await _userRepository.ChangePasswordAsync(user.UserID, request.Password);

                // Remove the reset token after successful password reset
                user.ResetToken = null;
                user.TokenExpiration = null;
                await _userRepository.UpdateUserAsync(user);

                Console.WriteLine($"Password reset successfully for user ID: {user.UserID}");

                return Ok(new { message = "Password reset successfully." });
            }
            catch (RepositoryException ex)
            {
                Console.WriteLine($"RepositoryException: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while resetting your password." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}