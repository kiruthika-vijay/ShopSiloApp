using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions; // Assuming exceptions are in this namespace
using System.Net;
using System.Net.Mail;
using ShopSiloAppFSD.Enums;
using System.Security.Claims;
using ShopSiloAppFSD.Services;
using ShopSiloAppFSD.Server.Models;

namespace ShopSiloAppFSD.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IEmailNotificationService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly IEmailServiceConfiguration _emailServiceConfig;
        private readonly string _userId;
        private readonly User _user;
        public UserRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IEmailNotificationService emailService, IAuditLogConfiguration auditLogConfig, IEmailServiceConfiguration emailServiceConfig)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _auditLogConfig = auditLogConfig;
            _emailServiceConfig = emailServiceConfig;
            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(_userId))
            {
                _user = _context.Users.FirstOrDefault(u => u.Username == _userId); // Or await FindAsync for async
            }
        }


        public async Task AddUserAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "User details added to database",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Error adding user to the database.", ex);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.UserID);
                if (existingUser == null)
                {
                    throw new NotFoundException($"User with ID {user.UserID} not found.");
                }

                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "User details updated in database",
                        Timestamp = DateTime.Now,
                        UserId = user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency error occurred while updating user.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating user.", ex);
            }
        }
        public async Task UpdateUserRoleAsync(int userId, UserRole role)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null)
                {
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                existingUser.Role = role;
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"User role updated for {userId}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency error occurred while updating user.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating user.", ex);
            }
        }
        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                user.IsActive = false;
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"User details set in-active for {userId}",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Error deleting user from the database.", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                return user;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving user by ID.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all users.", ex);
            }
        }

        public async Task<User> AuthenticateUserAsync(string identifier, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => (u.Email == identifier || u.Username == identifier) && u.Password == password);

                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"User with Username/Email {identifier} authenticated successfully.",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error authenticating user.", ex);
            }
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException($"User with ID {userId} not found.");
                }

                user.Password = newPassword;
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "User changed their password.",
                        Timestamp = DateTime.Now,
                        UserId = user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error changing user password.", ex);
            }
        }
        
        public async Task ChangePasswordOfLoggedUserAsync(string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
                var userId = user.UserID;
                if (user == null)
                {
                    throw new NotFoundException($"User with ID {userId} not found.");
                }
                var loggedUser = await _context.Users.FindAsync(userId);

                loggedUser.Password = newPassword;

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "User changed their password.",
                        Timestamp = DateTime.Now,
                        UserId = user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error changing user password.", ex);
            }
        }

        public async Task ResetPasswordAsync(string email)
        {
            try
            {
                // Step 1: Generate a secure random token for password reset
                string token = GenerateResetToken();

                // Step 2: Find the user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    throw new NotFoundException($"User with email {email} not found.");
                }

                // Step 3: Store the token in the user record
                user.ResetToken = token;
                user.TokenExpiration = DateTime.Now.AddHours(1); // Token expires in 1 hour

                // Step 4: Save changes to the database
                await _context.SaveChangesAsync();
                // Step 7: Check if email service is enabled before sending the email
                if (_emailServiceConfig.IsEmailServiceEnabled)
                {
                    // Step 5: Construct the reset password link
                    string resetLink = $"https://localhost:5173/reset-password/{token}"; ;

                    // Check if FullName is null, fallback to "User" if it is
                    var customerDetail = await _context.CustomerDetails.FirstOrDefaultAsync(c => c.CustomerID == user.UserID);
                    var fullName = (customerDetail.FirstName) + " " + (customerDetail.LastName);
                    string userName = string.IsNullOrEmpty(fullName) ? "User" : fullName;

                    // Step 6: Create a more professional email body
                    string emailBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; color: #333;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                                <h2 style='color: #0056b3;'>Password Reset Request</h2>
                                <p>Dear {userName},</p>

                                <p>We received a request to reset your password for your account associated with this email address. Please click the button below to reset your password:</p>

                                <div style='text-align: center; margin: 20px 0;'>
                                    <a href='{resetLink}' style='background-color: #0056b3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                                        Reset Your Password
                                    </a>
                                </div>

                                <p>If you did not request a password reset, please ignore this email. Your password will remain unchanged.</p>

                                <p style='margin-top: 30px;'>Thank you,<br/>
                                <strong>ShopSilo Ecom</strong><br/>
                                Customer Support Team<br/>
                                <a href='mailto:support@shopsilo.com'>support@shopsilo.com</a></p>
                            </div>
                        </body>
                    </html>";

                    _emailService.SendEmail(email, "Password Reset Request", emailBody);
                }
                // Log the action for security purposes
                Console.WriteLine($"Password reset link sent to {user.Email}");

                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    // Log the action in the audit log
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "User attempted to reset their password.",
                        Timestamp = DateTime.Now,
                        UserId = user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw; // Let the controller handle this as a NotFound response
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error resetting user password.", ex);
            }
        }

        private string GenerateResetToken()
        {
            // Step 4: Create a secure random token using cryptography
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);

                // Convert to a URL-safe base64 string
                return Convert.ToBase64String(tokenData).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                return await _context.Users.Where(u => u.Role == role).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving users by role.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetUsersByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.Where(u => u.Email == email).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving users by email.", ex);
            }
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking if email is registered.", ex);
            }
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                if (await IsEmailRegisteredAsync(user.Email))
                {
                    // Email is already registered
                    return false;
                }

                // Proceed with adding the user
                await AddUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error registering new user.", ex);
            }
        }

        public async Task<User> GetUserByResetTokenAsync(string token)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == token && u.TokenExpiration > DateTime.Now);
                if (user == null)
                {
                    throw new NotFoundException("Invalid or expired reset token.");
                }

                return user;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving user by reset token.", ex);
            }
        }

        public async Task<bool> SubscribeUserAsync(string email)
        {
            // Check if the email is already subscribed
            if (await _context.SubscribedUsers.AnyAsync(u => u.Email == email))
            {
                return false; // Email already subscribed
            }

            // Create a new SubscribedUser object
            var newUser = new SubscribedUsers { Email = email };

            // Send the email as part of the subscription process
            try
            {
                _emailService.SendEmail(email, "Subscription Confirmation", "Thank you for subscribing to our newsletter!");
                // Save to the database
                await _context.SubscribedUsers.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return true; // Successfully subscribed
            }
            catch
            {
                return false; // Handle failure to send email
            }
        }
    }
}