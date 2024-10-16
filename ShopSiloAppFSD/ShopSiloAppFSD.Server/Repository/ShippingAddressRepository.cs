using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Repository
{
    public class ShippingAddressRepository : IAddressRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public ShippingAddressRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        public async Task<Address> AddAddressAsync(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            try
            {
                await _context.Address.AddAsync(address);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"New shipping address added for User {address.CustomerID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
                return address;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error adding address.", ex);
            }
        }

        public async Task ToggleAddressStatusAsync(int addressId)
        {
            var address = await _context.Address.FindAsync(addressId);
            if (address != null)
            {
                address.IsActive = !address.IsActive; // Toggle the status
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleBillingAddressStatusAsync(int addressId)
        {
            var address = await _context.Address.FindAsync(addressId);
            if (address != null)
            {
                address.IsBillingAddress = !address.IsBillingAddress; // Toggle the status
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleShippingAddressStatusAsync(int addressId)
        {
            var address = await _context.Address.FindAsync(addressId);
            if (address != null)
            {
                address.IsShippingAddress = !address.IsShippingAddress; // Toggle the status
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            try
            {
                _context.Address.Update(address);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shipping address updated for User {address.CustomerID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
                return address;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error updating address.", ex);
            }
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            try
            {
                var address = await _context.Address.FindAsync(addressId);
                if (address == null) return false;

                address.IsActive = false;
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shipping address set in-active for User {address.CustomerID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error deleting address.", ex);
            }
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId)
        {
            try
            {
                return await _context.Address
                                     .FirstOrDefaultAsync(a => a.AddressID == addressId);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error retrieving address by ID.", ex);
            }
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Address
                                     .Where(a => a.CustomerID == userId)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error retrieving addresses by user ID.", ex);
            }
        }

        public async Task<IEnumerable<Address>> GetAddressesByLoggedUserAsync()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
                var userId = user.UserID;
                return await _context.Address
                                     .Where(a => a.CustomerID == userId)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error retrieving addresses by user ID.", ex);
            }
        }

        public async Task<Address?> GetDefaultShippingAddressAsync(int userId)
        {
            try
            {
                return await _context.Address
                                     .FirstOrDefaultAsync(a => a.CustomerID == userId && a.IsShippingAddress);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new RepositoryException("Error retrieving default shipping address.", ex);
            }
        }
    }
}
