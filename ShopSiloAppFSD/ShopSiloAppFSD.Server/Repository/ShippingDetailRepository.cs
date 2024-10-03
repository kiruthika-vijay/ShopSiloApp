using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSiloAppFSD.Repository
{
    public class ShippingDetailRepository : IShippingDetailRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public ShippingDetailRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        // Method to add new shipping details
        public async Task AddShippingDetailsAsync(ShippingDetail shippingDetails)
        {
            try
            {
                await _context.ShippingDetails.AddAsync(shippingDetails);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shipment created for Order {shippingDetails.OrderID} with Shipping ID {shippingDetails.ShippingID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding shipping details.", ex);
            }
        }

        // Method to update existing shipping details
        public async Task UpdateShippingDetailsAsync(ShippingDetail shippingDetails)
        {
            try
            {
                _context.ShippingDetails.Update(shippingDetails);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Shipment updated for Order {shippingDetails.OrderID} with Shipping ID {shippingDetails.ShippingID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating shipping details.", ex);
            }
        }

        // Method to delete shipping details by ID
        public async Task DeleteShippingDetailsAsync(int shippingId)
        {
            try
            {
                var shippingDetails = await _context.ShippingDetails.FindAsync(shippingId);
                if (shippingDetails != null)
                {
                    shippingDetails.IsActive = false;
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Shipment details set in-active for {shippingDetails.ShippingID}.",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Shipping details not found.");
                }
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException("Shipping details not found.",ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting shipping details.", ex);
            }
        }

        // Method to get shipping details by ID
        public async Task<ShippingDetail?> GetShippingDetailsByIdAsync(int shippingId)
        {
            try
            {
                return await _context.ShippingDetails
                    .Include(sd => sd.Order)
                    .FirstOrDefaultAsync(sd => sd.ShippingID == shippingId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving shipping details by ID.", ex);
            }
        }

        // Method to get shipping details by order ID
        public async Task<ShippingDetail?> GetShippingDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                return await _context.ShippingDetails
                    .Include(sd => sd.Order)
                    .FirstOrDefaultAsync(sd => sd.OrderID == orderId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving shipping details by order ID.", ex);
            }
        }

        // Method to update the shipping status
        public async Task UpdateShippingStatusAsync(int shippingId, ShippingStatus status)
        {
            try
            {
                var shippingDetails = await _context.ShippingDetails.FindAsync(shippingId);
                if (shippingDetails != null)
                {
                    shippingDetails.ShippingStatus = status;
                    _context.ShippingDetails.Update(shippingDetails);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Shipment status updated for Order {shippingDetails.OrderID} with Shipping ID {shippingDetails.ShippingID}.",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Shipping details not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating shipping status.", ex);
            }
        }

        // Method to perform shipment tracking
        public async Task<ShippingDetail?> TrackShipmentAsync(string trackingNumber)
        {
            try
            {
                return await _context.ShippingDetails
                    .FirstOrDefaultAsync(sd => sd.TrackingNumber == trackingNumber);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error tracking shipment.", ex);
            }
        }
    }
}
