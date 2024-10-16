using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.DTO;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Server.Interfaces;
using ShopSiloAppFSD.Server.Services;
using System.Security.Claims;

public class PaymentRepository : IPaymentRepository
{
    private readonly ShopSiloDBContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogConfiguration _auditLogConfig;
    private readonly IOrderService _orderService;
    private readonly IOrderRepository _orderRepository;
    private readonly string? _userId;
    private readonly User? _user;

    public PaymentRepository(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig, IOrderService orderService, IOrderRepository orderRepository)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _auditLogConfig = auditLogConfig;
        _orderService = orderService;
        _userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(_userId))
        {
            _user = _context.Users.FirstOrDefault(u => u.Username == _userId);
        }
        _orderRepository = orderRepository;
    }

    // Fetch payment transactions by user ID
    public async Task<IEnumerable<PaymentTransaction>> GetPaymentTransactionsByUserIdAsync(int userId)
    {
        return await _context.Payments
                             .Where(pt => pt.UserId == userId)
                             .Include(pt => pt.Order)
                             .ToListAsync();
    }

    // Fetch payment transactions for the logged-in user
    public async Task<IEnumerable<PaymentTransactionDto>> GetPaymentTransactionsByLoggedUserAsync()
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));

        if (user == null) return Enumerable.Empty<PaymentTransactionDto>();

        var payments = await _context.Payments
            .Where(pt => pt.UserId == user.UserID)
            .Include(pt => pt.Order) // You can include Order if necessary
            .ToListAsync();

        // Map to DTO
        return payments.Select(pt => new PaymentTransactionDto
        {
            TransactionId = pt.TransactionId,
            UserId = pt.UserId,
            RazorpayPaymentId = pt.RazorpayPaymentId,
            Amount = pt.Amount,
            PaymentStatus = pt.PaymentStatus,
            Timestamp = pt.Timestamp,
            OrderID = pt.OrderID
        });
    }


    // Save payment transaction and create order
    public async Task<(bool, int)> SavePaymentAndCreateOrderAsync(PaymentTransaction transaction, Order order, List<OrderItemDto> orderItems)
    {
        // Ensure there's no active transaction
        if (_context.Database.CurrentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == _userId || u.Email == _userId));
        // Assign the logged-in user's ID to the order
        order.UserID = user.UserID; // Use the correct logged-in user ID
        order.TrackingNumber = _orderService.GenerateOrderTrackingNumber();
        // Begin a new transaction
        using var transactionScope = await _context.Database.BeginTransactionAsync();

        try
        {
            // Save the order first
            var orders = _context.Orders.AddAsync(order);
            Console.Write(orders);
            await _context.SaveChangesAsync(); // Save to get the generated OrderID

            // Assign OrderID to the payment transaction
            transaction.OrderID = order.OrderID;

            // Save the payment transaction
            await _context.Payments.AddAsync(transaction);
            await _context.SaveChangesAsync();

            // Now save the order items
            foreach (var item in orderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    // Add any additional properties if required
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync(); // Save order items


            // Commit the transaction
            await transactionScope.CommitAsync();

            // Log the success in the audit
            if (_auditLogConfig.IsAuditLogEnabled && _user != null)
            {
                var auditLog = new AuditLog()
                {
                    Action = $"Payment and order created successfully for UserID: {transaction.UserId}, OrderID: {transaction.OrderID}",
                    Timestamp = DateTime.Now,
                    UserId = _user.UserID
                };
                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }

            return (true, order.OrderID);
        }
        catch (Exception ex)
        {
            await transactionScope.RollbackAsync();

            if (_auditLogConfig.IsAuditLogEnabled && _user != null)
            {
                var auditLog = new AuditLog()
                {
                    Action = $"Failed to create payment and order: {ex.Message}, StackTrace: {ex.StackTrace}",
                    Timestamp = DateTime.Now,
                    UserId = _user.UserID
                };
                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }

            return (false, 0);
        }
    }
}
