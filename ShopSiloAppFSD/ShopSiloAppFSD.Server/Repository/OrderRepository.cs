using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Razorpay.Api;
using ShopSiloAppFSD.Services;
using Order = ShopSiloAppFSD.Models.Order;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Server.DTO;

namespace ShopSiloAppFSD.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopSiloDBContext _context;
        private readonly IEmailNotificationService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogConfiguration _auditLogConfig;
        private readonly IEmailServiceConfiguration _emailSeviceConfig;
        private readonly string _userId;
        public OrderRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig, IEmailServiceConfiguration emailServiceConfig, IEmailNotificationService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditLogConfig = auditLogConfig;
            _emailSeviceConfig = emailServiceConfig;
            _emailService = emailService;
            _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task PlaceOrderAsync(Order order)
        {
            try
            {
                // Update inventory and process order items
                foreach (var item in order.OrderItems)
                {
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == item.ProductID);
                    if (inventory == null || inventory.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException("Insufficient stock.");
                    }

                    // Deduct the stock quantity
                    inventory.Quantity -= item.Quantity;
                    _context.Inventories.Update(inventory);
                }

                // Add the order to the database
                _context.Orders.Add(order);

                // Add the order items to the database
                foreach (var item in order.OrderItems)
                {
                    item.OrderID = order.OrderID;  // Associate the order items with the order
                    _context.OrderItems.Add(item);  // Assuming you have an OrderItems DbSet
                }

                await _context.SaveChangesAsync();

                // Send confirmation email and log audit
                await SendOrderConfirmationEmail(order); // Extract email logic to a separate method for clarity
                await LogAuditForOrder(order);

                // Final save to ensure audit logs are also persisted
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle any exceptions and log them appropriately
                throw new RepositoryException("Error placing an order.", ex);
            }
        }

        private async Task SendOrderConfirmationEmail(Order order)
        {
            if (_emailSeviceConfig.IsEmailServiceEnabled)
            {
                var paymentTransaction = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == order.OrderID);
                var customerDetail = await _context.CustomerDetails.FirstOrDefaultAsync(c => c.CustomerID == order.UserID);
                var fullName = $"{customerDetail?.FirstName} {customerDetail?.LastName}".Trim();
                string userName = string.IsNullOrEmpty(fullName) ? "User" : fullName;

                string emailBody = $@"
            <h1>Order Confirmation</h1>
            <p>Dear {userName},</p>
            <p>Thank you for placing your order with us!</p>
            <p>Your Order ID is: <strong>{order.OrderID}</strong></p>
            <p>Payment ID: <strong>{paymentTransaction?.RazorpayPaymentId}</strong></p>
            <p>We will notify you once your order is shipped.</p>
            <p>Thank you for shopping with us!</p>
            <p>Sincerely, <br/> The ShopSilo Team</p>";

                // Send order confirmation email
                _emailService.SendEmail(order.User.Email, "Order Confirmation", emailBody);
            }
        }

        private async Task LogAuditForOrder(Order order)
        {
            if (_auditLogConfig.IsAuditLogEnabled)
            {
                AuditLog auditLog = new AuditLog()
                {
                    Action = $"Order {order.OrderID} placed by {order.User.CustomerDetail?.FirstName} {order.User.CustomerDetail?.LastName}",
                    Timestamp = DateTime.Now,
                    UserId = order.UserID
                };

                await _context.AuditLogs.AddAsync(auditLog);
            }
        }


        // Update an existing order
        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
                var existingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderID == order.OrderID);

                if (existingOrder != null)
                {
                    existingOrder.ShippingAddressID = order.ShippingAddressID;
                    existingOrder.BillingAddressID = order.BillingAddressID;
                    existingOrder.TotalAmount = order.TotalAmount;
                    existingOrder.OrderStatus = order.OrderStatus;
                    existingOrder.UserID = order.UserID;
                    existingOrder.SellerID = order.SellerID;

                    _context.Orders.Update(existingOrder);
                    if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                    {
                        AuditLog auditLog = new AuditLog()
                        {
                            Action = $"Order has been updated for Order {order.OrderID} placed by customer {(order.User.CustomerDetail.FirstName) + " " + (order.User.CustomerDetail.LastName)}",
                            Timestamp = DateTime.Now,
                            UserId = user.UserID
                        };
                        await _context.AuditLogs.AddAsync(auditLog);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new NotFoundException("Order not found.");
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency error occurred while updating order.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating order.", ex);
            }
        }

        // Get an order by its ID
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                return await _context.Orders
                    .Include(oi => oi.OrderItems)
                    .ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(o => o.OrderID == orderId);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving order by ID.", ex);
            }
        }

        public async Task<OrderDetailDto?> GetOrderDetailByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(oi => oi.OrderItems)
                        .ThenInclude(p => p.Product)
                    .Include(o => o.Seller) // Include seller details
                    .FirstOrDefaultAsync(o => o.OrderID == orderId);

                if (order == null)
                    return null;

                // Map to DTO if necessary
                var orderDto = new OrderDetailDto
                {
                    OrderID = order.OrderID,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    OrderStatus = order.OrderStatus,
                    UserID = order.UserID,
                    SellerID = order.SellerID,
                    TrackingNumber = order.TrackingNumber,
                    SellerName = order.Seller?.ContactPerson, // Assuming Seller has a Name property
                    ShippingAddressID = order.ShippingAddressID,
                    BillingAddressID = order.BillingAddressID,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductID = oi.ProductID,
                        Quantity = oi.Quantity,
                        // Map other properties as needed
                    }).ToList()
                };

                return orderDto;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving order by ID.", ex);
            }
        }
        public async Task<IEnumerable<OrderDto>> GetSellerOrdersAsync(int sellerId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.SellerID == sellerId)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                // Map Order entity to OrderDto
                return orders.Select(o => new OrderDto
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    TrackingNumber = o.TrackingNumber,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductID = oi.ProductID,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving orders for seller.", ex);
            }
        }


        // Get orders by user ID
        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                var order =  await _context.Orders
                    .Where(o => o.UserID == userId)
                    .Include(oi => oi.OrderItems)
                    .ThenInclude(p => p.Product)
                    .ToListAsync();

                return order.Select(o => new OrderDto
                {
                    OrderID = o.OrderID,
                    TotalAmount = o.TotalAmount,
                    UserID = o.UserID,
                    SellerID = o.SellerID,
                    ShippingAddressID = o.ShippingAddressID,
                    BillingAddressID = o.BillingAddressID,
                    TrackingNumber = o.TrackingNumber,
                    DiscountID = o.DiscountID,
                    OrderStatus = o.OrderStatus
                });
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving orders by user ID.", ex);
            }
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByLoggedUserAsync()
        {
            try
            {
                // Assuming _userId contains the email or username from the authenticated user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == _userId || u.Email == _userId);

                // Check if the user exists
                if (user == null)
                {
                    throw new RepositoryException("User not found.");
                }

                var order = await _context.Orders
                   .Where(o => o.UserID == user.UserID)
                   .Include(oi => oi.OrderItems)
                   .ThenInclude(p => p.Product)
                   .ToListAsync();

                return order.Select(o => new OrderDto
                {
                    OrderID = o.OrderID,
                    TotalAmount = o.TotalAmount,
                    UserID = o.UserID,
                    SellerID = o.SellerID,
                    ShippingAddressID = o.ShippingAddressID,
                    BillingAddressID = o.BillingAddressID,
                    TrackingNumber = o.TrackingNumber,
                    DiscountID = o.DiscountID,
                    OrderStatus = o.OrderStatus
                });
            }
            catch (Exception ex)
            {
                // Log the exact exception message for debugging
                throw new RepositoryException("Error retrieving orders by user ID.", ex);
            }
        }

        // Get orders by status
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                {
                    throw new ArgumentException("Invalid order status.");
                }

                return await _context.Orders
                    .Include(oi => oi.OrderItems)
                    .ThenInclude(p => p.Product)
                    .Where(o => o.OrderStatus == orderStatus)
                    .ToListAsync();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving orders by status.", ex);
            }
        }

        // Cancel an order by setting its status to 'Cancelled'
        public async Task CancelOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                                          .Include(o => o.OrderItems)
                                          .FirstOrDefaultAsync(o => o.OrderID == orderId);
                if (order == null)
                {
                    throw new NotFoundException("Order not found.");
                }

                // Revert stock for each order item
                foreach (var item in order.OrderItems)
                {
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == item.ProductID);
                    if (inventory != null)
                    {
                        inventory.Quantity += item.Quantity;
                        _context.Inventories.Update(inventory);
                    }
                }

                // Update order status to canceled
                order.OrderStatus = OrderStatus.Cancelled;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                var user = await _context.Users.FirstOrDefaultAsync(u => (u.UserID == order.UserID));
                if (_emailSeviceConfig.IsEmailServiceEnabled) // Check if audit log is enabled
                {
                    // Check if FullName is null, fallback to "User" if it is
                    var customerDetail = await _context.CustomerDetails.FirstOrDefaultAsync(c => c.CustomerID == order.UserID);
                    var fullName = (customerDetail.FirstName) + " " + (customerDetail.LastName);
                    string userName = string.IsNullOrEmpty(fullName) ? "User" : fullName;

                    // Create email body with cancellation details
                    string emailBody = $@"
                        <h1>Order Cancellation</h1>
                        <p>Dear {fullName},</p>
                        <p>Your order with Order ID: <strong>{order.OrderID}</strong> has been cancelled.</p>
                        <p>If you have any questions, feel free to reach out to us.</p>
                        <p>Sincerely, <br/> The ShopSilo Team</p>";
                
                    // Send order cancellation email
                    _emailService.SendEmail(user.Email, "Order Cancellation Confirmation", emailBody);
                }
                if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
                {
                    // Audit log entry
                    AuditLog auditLog = new AuditLog()
                    {
                        Action = $"Order {orderId} cancelled by {order.User.CustomerDetail.FirstName} {order.User.CustomerDetail.LastName}",
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
                throw new RepositoryException("Error occurred while canceling the order.", ex);
            }
        }

        // Update tracking information for an order
        public async Task UpdateTrackingNumberAsync(int orderId, string trackingNumber)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    throw new NotFoundException("Order not found.");
                }

                order.TrackingNumber = trackingNumber;
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating tracking number.", ex);
            }
        }

        // Get tracking information for an order
        public async Task<string?> GetTrackingNumberAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                return order?.TrackingNumber;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving tracking number.", ex);
            }
        }

        // Add Tracking number
        public async Task AddTrackingNumberAsync(int orderId, string trackingNumber)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            order.TrackingNumber = trackingNumber;
            await _context.SaveChangesAsync();
        }
    }
}
