using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
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
    [TestFixture]
    public class CartItemRepositoryTests : IDisposable
    {
        private ShopSiloDBContext _context;
        private CartItemRepository _cartItemRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAuditLogConfiguration> _mockAuditLogConfig;
        private string _userId = "testuser";
        private User _user;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<ShopSiloDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopSiloTestDB")
                .Options;
            _context = new ShopSiloDBContext(options);

            // Set up mock objects
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAuditLogConfig = new Mock<IAuditLogConfiguration>();

            // Set up mock HttpContextAccessor
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, _userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var userPrincipal = new ClaimsPrincipal(identity);
            var mockHttpContext = new DefaultHttpContext { User = userPrincipal };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            // Seed a user in the database
            _user = new User { UserID = 1, Username = _userId, Email = "test@example.com", Password = "testpassword" };
            _context.Users.Add(_user);
            _context.SaveChanges();

            // Initialize the repository with mocks and in-memory context
            _cartItemRepository = new CartItemRepository(_context, _mockHttpContextAccessor.Object, _mockAuditLogConfig.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Ensure the in-memory database is cleared between tests
            _context.Database.EnsureDeleted();
        }

        

        [Test]
        public async Task AddCartItemAsync_ValidCartItem_AddsToDatabase()
        {
            // Arrange
            var cartItem = new CartItem
            {
                CartID = 1,
                ProductID = 1,
                Quantity = 2,
                Price = 100
            };
            _mockAuditLogConfig.Setup(x => x.IsAuditLogEnabled).Returns(true);

            // Act
            await _cartItemRepository.AddCartItemAsync(cartItem);

            // Assert
            var cartItems = await _context.CartItems.ToListAsync();
            Assert.AreEqual(1, cartItems.Count);
            Assert.AreEqual(1, cartItems[0].CartID);
            Assert.AreEqual(100, cartItems[0].Price);
        }

       

        [Test]
        public async Task UpdateCartItemAsync_CartItemNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var cartItem = new CartItem
            {
                CartItemID = 999,
                CartID = 1,
                ProductID = 1,
                Quantity = 2,
                Price = 100
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
                await _cartItemRepository.UpdateCartItemAsync(cartItem));
            Assert.AreEqual("Cart item not found.", ex.Message);
        }

        [Test]
        public async Task DeleteCartItemAsync_CartItemNotFound_ReturnsFalse()
        {
            // Act
            var result = await _cartItemRepository.DeleteCartItemAsync(999);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteCartItemAsync_ValidCartItem_ReturnsTrueAndRemovesItem()
        {
            // Arrange
            var cartItem = new CartItem
            {
                CartID = 1,
                ProductID = 1,
                Quantity = 2,
                Price = 100
            };
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartItemRepository.DeleteCartItemAsync(cartItem.CartItemID);

            // Assert
            Assert.IsTrue(result);
            Assert.IsEmpty(await _context.CartItems.ToListAsync());
        }

        [Test]
        public async Task GetCartItemByIdAsync_CartItemNotFound_ReturnsNull()
        {
            // Act
            var result = await _cartItemRepository.GetCartItemByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

       

        [Test]
        public async Task UpdateCartItemQuantityAsync_InvalidQuantity_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _cartItemRepository.UpdateCartItemQuantityAsync(1, 0));
            Assert.AreEqual("Quantity must be greater than zero.", ex.Message);
        }

        [Test]
        public async Task UpdateCartItemQuantityAsync_ValidQuantity_UpdatesCartItem()
        {
            // Arrange
            var cartItem = new CartItem
            {
                CartID = 1,
                ProductID = 1,
                Quantity = 2,
                Price = 100
            };
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            // Act
            var updatedCartItem = await _cartItemRepository.UpdateCartItemQuantityAsync(cartItem.CartItemID, 5);

            // Assert
            Assert.AreEqual(5, updatedCartItem.Quantity);
        }

        // Implement the Dispose method to clean up resources
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

