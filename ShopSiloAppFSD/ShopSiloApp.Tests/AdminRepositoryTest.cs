using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Exceptions;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopSiloApp.Tests
{
    public class AdminRepositoryTests : IDisposable
    {
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAuditLogConfiguration> _mockAuditLogConfig;
        private AdminRepository _adminRepository;
        private ShopSiloDBContext _context;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShopSiloDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopSiloTestDB")
                .Options;

            _context = new ShopSiloDBContext(options);
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAuditLogConfig = new Mock<IAuditLogConfiguration>();

            // Setup the user in HttpContext
            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            var httpContext = new DefaultHttpContext { User = _user };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Seed initial data
            SeedDatabase();

            _adminRepository = new AdminRepository(_context, _mockHttpContextAccessor.Object, _mockAuditLogConfig.Object);
        }

        private void SeedDatabase()
        {
            _context.Users.Add(new User
            {
                UserID = 1, // Ensure this ID is unique
                Username = "admin",
                Role = UserRole.Admin,
                Email = "admin@example.com", // Required field
                Password = "AdminPassword123" // Required field
            });

            _context.Categories.Add(new Category
            {
                CategoryID = 1, // Ensure this ID is unique
                CategoryName = "Electronics",
                Status = ApprovalStatus.Pending
            });

            _context.Categories.Add(new Category
            {
                CategoryID = 2, // Add a different ID to avoid duplicates
                CategoryName = "Books",
                Status = ApprovalStatus.Pending
            });

            _context.Orders.Add(new Order
            {
                OrderID = 1, // Ensure this ID is unique
                OrderDate = DateTime.Now.AddDays(-2),
                TotalAmount = 100
            });

            _context.Orders.Add(new Order
            {
                OrderID = 2, // Ensure this ID is unique
                OrderDate = DateTime.Now.AddDays(-1),
                TotalAmount = 200
            });

            _context.OrderItems.Add(new OrderItem
            {
                OrderItemID = 1, // Ensure this ID is unique
                ProductID = 1,
                Quantity = 5
            });

            _context.Products.Add(new Product
            {
                ProductID = 1, // Ensure this ID is unique
                ProductName = "Laptop"
            });

            _context.Products.Add(new Product
            {
                ProductID = 2, // Add a different ID to avoid duplicates
                ProductName = "Smartphone"
            });

            _context.SaveChanges();
        }



        [Test]
        public async Task GenerateSalesReportAsync_ReturnsOrdersWithinDateRange()
        {
            // Act
            var result = await _adminRepository.GenerateSalesReportAsync(DateTime.Now.AddDays(-3), DateTime.Now);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

       

        [Test]
        public async Task UpdateCategoryStatusAsync_AdminUpdatesCategoryStatus_Success()
        {
            // Arrange
            _mockAuditLogConfig.Setup(x => x.IsAuditLogEnabled).Returns(true);

            // Act
            await _adminRepository.UpdateCategoryStatusAsync(1, ApprovalStatus.Approved);

            // Assert
            var category = await _context.Categories.FindAsync(1);
            Assert.AreEqual(ApprovalStatus.Approved, category.Status);

            var auditLog = _context.AuditLogs.FirstOrDefault();
            Assert.IsNotNull(auditLog);
            Assert.AreEqual("Category 'Electronics' approved by admin.", auditLog.Action);
        }
        


        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Clean up the database after each test
            _context.Dispose(); // Dispose the DbContext
        }
    }
}
