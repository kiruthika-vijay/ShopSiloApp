using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address> AddAddressAsync(Address address);
        Task<Address> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(int addressId);
        Task<Address?> GetAddressByIdAsync(int addressId);
        Task ToggleAddressStatusAsync(int addressId);
        Task ToggleBillingAddressStatusAsync(int addressId);
        Task ToggleShippingAddressStatusAsync(int addressId);
        Task<IEnumerable<Address>> GetAddressesByUserIdAsync(int userId);
        Task<IEnumerable<Address>> GetAddressesByLoggedUserAsync();
        Task<Address?> GetDefaultShippingAddressAsync(int userId);
    }
}
