using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Exceptions;
using System.Security.Claims; // Assuming exceptions are in this namespace

namespace ShopSiloAppFSD.Repository
{
    public class CustomerDetailsRepository : ICustomerDetailsRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public CustomerDetailsRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditLogConfig = auditLogConfig;
            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(_userId))
            {
                _user = _context.Users.FirstOrDefault(u => u.Username == _userId); // Or await FindAsync for async
            }
        }
        public async Task AddCustomerDetailsAsync(CustomerDetail customerDetail)
        {
            try
            {
                await _context.CustomerDetails.AddAsync(customerDetail);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "Customer detail added to the database.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Error adding customer details to the database.", ex);
            }
        }

        public async Task UpdateCustomerDetailsAsync(CustomerDetail customerDetail)
        {
            try
            {
                var existingCustomer = await _context.CustomerDetails.FindAsync(customerDetail.CustomerID);

                if (existingCustomer == null)
                {
                    Console.WriteLine($"Customer with ID {customerDetail.CustomerID} not found.");  // Debugging
                    throw new NotFoundException($"Customer with ID {customerDetail.CustomerID} not found.");
                }

                existingCustomer.FirstName = customerDetail.FirstName;
                existingCustomer.LastName = customerDetail.LastName;
                existingCustomer.PhoneNumber = customerDetail.PhoneNumber;
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = "Customer detail updated in the database.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency error occurred while updating customer details.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating customer details.", ex);
            }
        }

        public async Task<CustomerDetail> GetCustomerDetailsByIdAsync(int customerId)
        {
            try
            {
                var customerDetail = await _context.CustomerDetails.FindAsync(customerId);
                if (customerDetail == null)
                {
                    throw new NotFoundException($"Customer with ID {customerId} not found.");
                }

                return customerDetail;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving customer details by ID.", ex);
            }
        }

        public async Task<IEnumerable<CustomerDetail>> GetCustomerDetailsOfLoggedUser()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email ==  _userId));
                var customerId = user.UserID;

                var customerDetail = await _context.CustomerDetails.Where(c => c.CustomerID == customerId).Include(c => c.Addresses).ToListAsync();
                if (customerDetail == null)
                {
                    throw new NotFoundException($"Customer with ID {customerId} not found.");
                }

                return customerDetail;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving customer details by ID.", ex);
            }
        }

        public async Task DeleteCustomerDetailAsync(int customerId)
        {
            try
            {
                var customer = await _context.CustomerDetails.FirstOrDefaultAsync(s => s.CustomerID == customerId);
                if (customer == null)
                {
                    throw new NotFoundException($"Customer with ID {customerId} not found.");
                }

                // Now remove the seller
                customer.IsActive = false;
                if (customer.IsActive == false)
                {
                    _user.IsActive = false;
                }
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Customer detail set in-active for {customerId}.",
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
                throw new RepositoryException("Error deleting customer from the database.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Cannot delete customer data, because the customer {customerId} has pending orders to complete", ex);
            }
        }

        public async Task<IEnumerable<CustomerDetail>> GetCustomerDetails()
        {
            try
            {
                var customerDetail = await _context.CustomerDetails.ToListAsync();


                return customerDetail;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving customer details by ID.", ex);
            }
        }
    }
}
