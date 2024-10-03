using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> AuthenticateUserAsync(string identifier, string password);
        Task ChangePasswordAsync(int userId, string newPassword);
        Task ChangePasswordOfLoggedUserAsync(string newPassword);
        Task ResetPasswordAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetUsersByEmailAsync(string email);
        Task<User> GetUserByResetTokenAsync(string token);
        Task<bool> SubscribeUserAsync(string email);
    }
}
