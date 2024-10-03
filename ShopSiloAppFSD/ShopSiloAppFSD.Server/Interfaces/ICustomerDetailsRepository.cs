using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface ICustomerDetailsRepository
    {
        Task AddCustomerDetailsAsync(CustomerDetail customerDetail);
        Task UpdateCustomerDetailsAsync(CustomerDetail customerDetail);
        Task<CustomerDetail> GetCustomerDetailsByIdAsync(int customerId);
        Task<IEnumerable<CustomerDetail>> GetCustomerDetailsOfLoggedUser();
        Task<IEnumerable<CustomerDetail>> GetCustomerDetails();
        Task DeleteCustomerDetailAsync(int customerId);
    }
}
