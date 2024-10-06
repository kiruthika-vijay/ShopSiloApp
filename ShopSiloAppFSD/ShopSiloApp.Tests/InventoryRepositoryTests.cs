using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ShopSiloAppFSD.Interfaces;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using ShopSiloAppFSD.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ShopSiloApp.Tests
{
    public class InventoryRepositoryTests
    {
        private InventoryRepository _inventoryRepository;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IEmailNotificationService> _emailServiceMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IAuditLogConfiguration> _auditLogConfigMock;
        private Mock<IEmailServiceConfiguration> _emailServiceConfigMock;
        private ShopSiloDBContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShopSiloDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopSiloDB")
                .Options;

            _context = new ShopSiloDBContext(options);

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _emailServiceMock = new Mock<IEmailNotificationService>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _auditLogConfigMock = new Mock<IAuditLogConfiguration>();
            _emailServiceConfigMock = new Mock<IEmailServiceConfiguration>();

            // Setup HttpContext mock to simulate a user with a valid ID
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

            _inventoryRepository = new InventoryRepository(
                _context,
                _httpContextAccessorMock.Object,
                _emailServiceMock.Object,
                _auditLogConfigMock.Object,
                _emailServiceConfigMock.Object,
                _productRepositoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        

        [Test]
        public async Task DeleteInventoryAsync_ShouldThrowNotFoundException_WhenInventoryNotFound()
        {
            // Arrange
            int nonExistentInventoryId = 999;

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _inventoryRepository.DeleteInventoryAsync(nonExistentInventoryId));
        }

        [Test]
        public async Task DeleteInventoryAsync_ShouldSetInventoryToInactive_WhenInventoryExists()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "TestProduct" };
            var inventory = new Inventory { InventoryID = 1, Product = product, IsActive = true };
            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();

            // Act
            await _inventoryRepository.DeleteInventoryAsync(1);

            // Assert
            var updatedInventory = await _context.Inventories.FindAsync(1);
            Assert.IsFalse(updatedInventory.IsActive);
        }

       
        [Test]
        public async Task GetLowStockProductsAsync_ShouldReturnLowStockProducts()
        {
            // Arrange
            var product1 = new Product { ProductID = 1, ProductName = "Product1", SellerID = 1 };
            var product2 = new Product { ProductID = 2, ProductName = "Product2", SellerID = 1 };
            var inventory1 = new Inventory { InventoryID = 1, ProductID = 1, Product = product1, Quantity = 10 };
            var inventory2 = new Inventory { InventoryID = 2, ProductID = 2, Product = product2, Quantity = 30 };

            await _context.Products.AddRangeAsync(product1, product2);
            await _context.Inventories.AddRangeAsync(inventory1, inventory2);
            await _context.SaveChangesAsync();

            // Act
            var lowStockProducts = await _inventoryRepository.GetLowStockProductsAsync(20);

            // Assert
            Assert.AreEqual(1, lowStockProducts.Count());
            Assert.AreEqual("Product1", lowStockProducts.First().ProductName);
        }

        [Test]
        public async Task RestockProductAsync_ShouldIncreaseInventoryQuantity_WhenProductExists()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "TestProduct" };
            var inventory = new Inventory { InventoryID = 1, ProductID = 1, Product = product, Quantity = 10 };

            await _context.Products.AddAsync(product);
            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();

            // Act
            await _inventoryRepository.RestockProductAsync(1, 20);

            // Assert
            var updatedInventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == 1);
            Assert.AreEqual(30, updatedInventory.Quantity);
        }
    }
}
