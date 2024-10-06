  using Moq;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using ShopSiloAppFSD.Repository;
    using ShopSiloAppFSD.Models;
    using ShopSiloAppFSD.Interfaces;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Collections.Generic;
    using ShopSiloAppFSD.Exceptions;
namespace ShopSiloApp.Tests
{


    [TestFixture]
    public class CustomerDetailsRepositoryTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IAuditLogConfiguration> _auditLogConfigMock;
        private ShopSiloDBContext _context;
        private CustomerDetailsRepository _repository;
        private string _userId = "test-user-id";

        [SetUp]
        public void SetUp()
        {
            // InMemory database setup
            var options = new DbContextOptionsBuilder<ShopSiloDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ShopSiloDBContext(options);

            // Seed User and CustomerDetails
            SeedDatabase();

            // Mock IHttpContextAccessor
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, _userId)
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            // Mock IAuditLogConfiguration
            _auditLogConfigMock = new Mock<IAuditLogConfiguration>();
            _auditLogConfigMock.Setup(x => x.IsAuditLogEnabled).Returns(true); // Mock audit log as enabled

            // Create repository instance
            _repository = new CustomerDetailsRepository(_context, _httpContextAccessorMock.Object, _auditLogConfigMock.Object);
        }
        [TearDown]
        public void Dispose()
        {
            _context.Dispose();
        }

        private void SeedDatabase()
        {
            // Seed User
            var user = new User
            {
                UserID = 1,
                Username = _userId,
                Email = "test@example.com",
                Password = "hashedpassword", // Include all required fields
                IsActive = true
            };
            _context.Users.Add(user);

            // Seed CustomerDetails
            var customerDetail = new CustomerDetail
            {
                CustomerID = 1,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890",
               
                IsActive = true
            };
            _context.CustomerDetails.Add(customerDetail);

            // Save the changes
            _context.SaveChanges();
        }

        [Test]
        public async Task AddCustomerDetailsAsync_Should_Add_CustomerDetails_And_CreateAuditLog()
        {
            // Arrange
            var customerDetail = new CustomerDetail
            {
                CustomerID = 2, // Ensure unique ID
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "9876543210",
                IsActive = true
            };

            // Act
            await _repository.AddCustomerDetailsAsync(customerDetail);
            var result = await _context.CustomerDetails.FindAsync(2); // Check if the customer was added

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Jane", result.FirstName);
            Assert.AreEqual(1, _context.AuditLogs.Count()); // Ensure audit log is created
        }


        [Test]
        public async Task GetCustomerDetailsByIdAsync_Should_Return_CustomerDetail_If_Found()
        {
            // Act
            var result = await _repository.GetCustomerDetailsByIdAsync(1); // Assuming ID 1 exists from seeding

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public async Task GetCustomerDetailsByIdAsync_Should_Throw_NotFoundException_If_Customer_Not_Found()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _repository.GetCustomerDetailsByIdAsync(999)); // Non-existent ID
            Assert.AreEqual("Customer with ID 999 not found.", ex.Message);
        }

        [Test]
        public async Task UpdateCustomerDetailsAsync_Should_Update_CustomerDetail_Successfully()
        {
            // Arrange
            var updatedCustomerDetail = new CustomerDetail
            {
                CustomerID = 1, // Assuming ID 1 exists from seeding
                FirstName = "UpdatedName",
                LastName = "UpdatedLastName",
                PhoneNumber = "9876543210"
            };

            // Act
            await _repository.UpdateCustomerDetailsAsync(updatedCustomerDetail);
            var result = await _context.CustomerDetails.FindAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UpdatedName", result.FirstName);
            Assert.AreEqual(1, _context.AuditLogs.Count()); // Ensure audit log is created
        }

        [Test]
        public async Task DeleteCustomerDetailAsync_Should_Set_CustomerDetail_Inactive_And_CreateAuditLog()
        {
            // Arrange
            var customerId = 1; // Assuming ID 1 exists from seeding

            // Act
            await _repository.DeleteCustomerDetailAsync(customerId);
            var customer = await _context.CustomerDetails.FindAsync(customerId);

            // Assert
            Assert.IsNotNull(customer);
            Assert.IsFalse(customer.IsActive); // Ensure the customer is marked as inactive
            Assert.AreEqual(1, _context.AuditLogs.Count()); // Ensure audit log is created
        }

        [Test]
        public async Task DeleteCustomerDetailAsync_Should_Throw_NotFoundException_If_Customer_Not_Found()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _repository.DeleteCustomerDetailAsync(999)); // Non-existent ID
            Assert.AreEqual("Customer with ID 999 not found.", ex.Message);
        }


       


    }



}
