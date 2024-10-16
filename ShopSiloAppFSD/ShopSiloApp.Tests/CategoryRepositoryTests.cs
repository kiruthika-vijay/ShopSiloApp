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
    [TestFixture]
    public class CategoryRepositoryTests
    {
        private CategoryRepository _categoryRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAuditLogConfiguration> _mockAuditLogConfig;
        private ShopSiloDBContext _context;
        private string _userId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShopSiloDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopSiloDB")
                .Options;

            _context = new ShopSiloDBContext(options);

            // Seed a valid User in the context
            var user = new User
            {
                UserID = 1,
                Username = "testuser",
                Email = "testuser@example.com", // Required
                Password = "testpassword", // Required
                Role = UserRole.Admin // Assume the role is required
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Setup HttpContextAccessor Mock
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            }));

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Setup AuditLogConfiguration Mock
            _mockAuditLogConfig = new Mock<IAuditLogConfiguration>();
            _mockAuditLogConfig.Setup(x => x.IsAuditLogEnabled).Returns(true);

            _categoryRepository = new CategoryRepository(_context, _mockHttpContextAccessor.Object, _mockAuditLogConfig.Object);
        }
        [TearDown]
        public void TearDown()
        {
            // Ensure the in-memory database is cleared between tests
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddCategoryAsync_ShouldAddCategory_WhenValid()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "New Category",
                IsActive = true
            };

            // Act
            await _categoryRepository.AddCategoryAsync(category);

            // Assert
            var addedCategory = _context.Categories.FirstOrDefault(c => c.CategoryName == "New Category");
            Assert.NotNull(addedCategory);
            Assert.AreEqual(ApprovalStatus.Approved, addedCategory.Status); // Since user role is Admin
        }

       

        [Test]
        public async Task UpdateCategoryAsync_ShouldUpdateCategory_WhenValid()
        {
            // Arrange
            var existingCategory = new Category
            {
                CategoryName = "Category to Update",
                IsActive = true
            };

            _context.Categories.Add(existingCategory);
            await _context.SaveChangesAsync();

            var updatedCategory = new Category
            {
                CategoryID = existingCategory.CategoryID,
                CategoryName = "Updated Category Name",
                ParentCategoryId = null,
                IsActive = true
            };

            // Act
            await _categoryRepository.UpdateCategoryAsync(updatedCategory);

            // Assert
            var updated = await _context.Categories.FindAsync(existingCategory.CategoryID);
            Assert.NotNull(updated);
            Assert.AreEqual("Updated Category Name", updated.CategoryName);
        }

        [Test]
        public void UpdateCategoryAsync_ShouldThrowException_WhenCategoryNotFound()
        {
            // Arrange
            var categoryToUpdate = new Category
            {
                CategoryID = 999, // Non-existent ID
                CategoryName = "Non-existent Category",
                IsActive = true
            };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _categoryRepository.UpdateCategoryAsync(categoryToUpdate));
        }

        [Test]
        public async Task DeleteCategoryAsync_ShouldSetCategoryInactive_WhenValid()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "Category to Delete",
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            await _categoryRepository.DeleteCategoryAsync(category.CategoryID);

            // Assert
            var deletedCategory = await _context.Categories.FindAsync(category.CategoryID);
            Assert.NotNull(deletedCategory);
            Assert.IsFalse(deletedCategory.IsActive); // Check if IsActive is set to false
        }

        [Test]
        public void DeleteCategoryAsync_ShouldThrowException_WhenCategoryNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _categoryRepository.DeleteCategoryAsync(999)); // Non-existent ID
        }

        [Test]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenExists()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "Existing Category",
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var retrievedCategory = await _categoryRepository.GetCategoryByIdAsync(category.CategoryID);

            // Assert
            Assert.NotNull(retrievedCategory);
            Assert.AreEqual(category.CategoryName, retrievedCategory.CategoryName);
        }

        [Test]
        public void GetCategoryByIdAsync_ShouldThrowException_WhenCategoryNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _categoryRepository.GetCategoryByIdAsync(999)); // Non-existent ID
        }

        [Test]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var category1 = new Category { CategoryName = "Category 1", IsActive = true };
            var category2 = new Category { CategoryName = "Category 2", IsActive = true };

            _context.Categories.AddRange(category1, category2);
            await _context.SaveChangesAsync();

            // Act
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            // Assert
            Assert.AreEqual(2, categories.Count());
        }

        // Additional tests can be added to cover more edge cases as needed.
    }
}
