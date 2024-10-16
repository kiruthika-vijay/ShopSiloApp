using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Razorpay.Api;
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
    public class ProductReviewRepositoryTests
    {
        private ProductReviewRepository _productReviewRepository;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IAuditLogConfiguration> _auditLogConfigMock;
        private ShopSiloDBContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShopSiloDBContext>()
                .UseInMemoryDatabase(databaseName: "ShopSiloDB")
                .Options;

            _context = new ShopSiloDBContext(options);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _auditLogConfigMock = new Mock<IAuditLogConfiguration>();

            // Setup HttpContext mock to simulate a user with a valid ID
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = claimsPrincipal };
            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

            _productReviewRepository = new ProductReviewRepository(
                _context,
                _httpContextAccessorMock.Object,
                _auditLogConfigMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddReviewAsync_ShouldThrowRepositoryException_WhenReviewIsNull()
        {
            // Arrange
            ProductReview review = null;

            // Act & Assert
            Assert.ThrowsAsync<RepositoryException>(async () =>
                await _productReviewRepository.AddReviewAsync(review));
        }

        [Test]
        public async Task UpdateReviewAsync_ShouldThrowNotFoundException_WhenReviewDoesNotExist()
        {
            // Arrange
            var review = new ProductReview { ReviewID = 1, Rating = 5, ReviewText = "Great product!" };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _productReviewRepository.UpdateReviewAsync(review));
        }

        [Test]
        public async Task DeleteReviewAsync_ShouldThrowNotFoundException_WhenReviewDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _productReviewRepository.DeleteReviewAsync(999));
        }

        [Test]
        public async Task GetReviewByIdAsync_ShouldThrowNotFoundException_WhenReviewDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _productReviewRepository.GetReviewByIdAsync(999));
        }

        [Test]
        public async Task GetReviewsByProductAsync_ShouldReturnEmpty_WhenNoReviewsExist()
        {
            // Act
            var reviews = await _productReviewRepository.GetReviewsByProductAsync(1);

            // Assert
            Assert.IsEmpty(reviews);
        }

        

        [Test]
        public async Task UpdateReviewAsync_ShouldUpdateReview_WhenReviewExists()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "TestProduct" };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var review = new ProductReview
            {
                ProductID = 1,
                Rating = 5,
                ReviewText = "Great product!"
            };
            await _context.ProductReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            review.Rating = 4;
            review.ReviewText = "Updated review text";

            // Act
            await _productReviewRepository.UpdateReviewAsync(review);

            // Assert
            var updatedReview = await _context.ProductReviews.FindAsync(review.ReviewID);
            Assert.AreEqual(4, updatedReview.Rating);
            Assert.AreEqual("Updated review text", updatedReview.ReviewText);
        }

        [Test]
        public async Task DeleteReviewAsync_ShouldRemoveReview_WhenReviewExists()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "TestProduct" };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var review = new ProductReview
            {
                ProductID = 1,
                Rating = 5,
                ReviewText = "Great product!"
            };
            await _context.ProductReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // Act
            await _productReviewRepository.DeleteReviewAsync(review.ReviewID);

            // Assert
            var deletedReview = await _context.ProductReviews.FindAsync(review.ReviewID);
            Assert.IsNull(deletedReview);
        }
    }
}
