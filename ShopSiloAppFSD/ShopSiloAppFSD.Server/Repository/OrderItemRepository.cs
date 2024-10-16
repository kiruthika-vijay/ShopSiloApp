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
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly string _userId;
        private readonly User _user;
        public OrderItemRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

        // Add a new OrderItem
        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            try
            {
                await _context.OrderItems.AddAsync(orderItem);
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Order item added for Order {orderItem.OrderID}.",
                        Timestamp = DateTime.Now,
                        UserId = _user.UserID
                    };
                    await _context.AuditLogs.AddAsync(auditLog);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding order item.", ex);
            }
        }

        // Update an existing OrderItem
        public async Task UpdateOrderItemAsync(OrderItem orderItem)
        {
            try
            {
                var existingOrderItem = await _context.OrderItems
                    .Include(oi => oi.Order)   // Ensure related entities are loaded
                    .Include(oi => oi.Product)
                    .FirstOrDefaultAsync(oi => oi.OrderItemID == orderItem.OrderItemID);

                if (existingOrderItem != null)
                {
                    existingOrderItem.Quantity = orderItem.Quantity;
                    existingOrderItem.Price = orderItem.Price;
                    existingOrderItem.OrderID = orderItem.OrderID;
                    existingOrderItem.ProductID = orderItem.ProductID;

                    _context.OrderItems.Update(existingOrderItem);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Order item updated for Order {orderItem.OrderID}.",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("OrderItem not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency error occurred while updating order item.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating order item.", ex);
            }
        }

        // Delete an OrderItem by ID
        public async Task DeleteOrderItemAsync(int orderItemId)
        {
            try
            {
                var orderItem = await _context.OrderItems.FindAsync(orderItemId);
                if (orderItem != null)
                {
                    _context.OrderItems.Remove(orderItem);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Order item removed for Order {orderItem.OrderID}.",
                            Timestamp = DateTime.Now,
                            UserId = _user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("OrderItem not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting order item.", ex);
            }
        }

        // Get an OrderItem by ID
        public async Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId)
        {
            try
            {
                return await _context.OrderItems
                    .Include(oi => oi.Order)   // Eager load related Order
                    .Include(oi => oi.Product) // Eager load related Product
                    .FirstOrDefaultAsync(oi => oi.OrderItemID == orderItemId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving order item by ID.", ex);
            }
        }

        // Get all OrderItems for a specific OrderID
        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            try
            {
                return await _context.OrderItems
                    .Where(oi => oi.OrderID == orderId)
                    .Include(oi => oi.Product)  // Eager load related Product
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving order items by order ID.", ex);
            }
        }
    }
}
