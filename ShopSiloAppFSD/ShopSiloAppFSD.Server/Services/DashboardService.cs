using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using System.Security.Claims;

public class DashboardService
{
    private readonly ShopSiloDBContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogConfiguration _auditLogConfig;
    private readonly string _userId;
    private readonly User _user;
    public DashboardService(ShopSiloDBContext context, IHttpContextAccessor httpContextAccessor, IAuditLogConfiguration auditLogConfig)
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

    public async Task UpdateDashboardAsync(int dashboardId)
    {
        try
        {
            var existingDashboard = await _context.SellerDashboards
                .SingleOrDefaultAsync(sd => sd.DashboardID == dashboardId);

            if (existingDashboard == null)
            {
                throw new NotFoundException($"SellerDashboard with ID {dashboardId} not found.");
            }

            // Calculate updated values
            existingDashboard.TotalSales = await CalculateTotalSalesAsync();
            existingDashboard.TotalOrders = await CalculateTotalOrdersAsync();
            existingDashboard.TotalProducts = await CalculateTotalProductsAsync();
            existingDashboard.TotalRevenue = await CalculateTotalRevenueAsync();
            existingDashboard.LastLogin = DateTime.Now; // Or some other logic for LastLogin

            // Mark the entity as modified
            _context.SellerDashboards.Update(existingDashboard);

            if (_auditLogConfig.IsAuditLogEnabled) // Check if audit log is enabled
            {
                // Log the update
                AuditLog auditLog = new AuditLog
                {
                    Action = $"Seller Dashboard has been updated with new statistics.",
                    Timestamp = DateTime.Now,
                    UserId = _user.UserID // Assuming UserID is available from the context
                };
                await _context.AuditLogs.AddAsync(auditLog);
            }
            // Save changes to the database
            await _context.SaveChangesAsync();
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException("Concurrency error occurred while updating SellerDashboard.", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Error updating SellerDashboard.", ex);
        }
    }

    private async Task<decimal> CalculateTotalSalesAsync()
    {
        return await _context.Orders
            .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30)) // Example: last 30 days
            .SumAsync(o => o.TotalAmount);
    }

    private async Task<int> CalculateTotalOrdersAsync()
    {
        return await _context.Orders
            .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30)) // Example: last 30 days
            .CountAsync();
    }

    private async Task<int> CalculateTotalProductsAsync()
    {
        return await _context.OrderItems
            .Where(oi => oi.Order.OrderDate >= DateTime.Now.AddDays(-30)) // Example: last 30 days
            .Select(oi => oi.ProductID)
            .Distinct()
            .CountAsync();
    }

    private async Task<decimal> CalculateTotalRevenueAsync()
    {
        return await _context.Orders
            .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30)) // Example: last 30 days
            .SumAsync(o => o.TotalAmount);
    }
}
