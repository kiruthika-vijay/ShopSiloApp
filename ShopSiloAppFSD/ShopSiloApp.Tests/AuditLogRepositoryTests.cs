using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ShopSiloAppFSD.Models;
using ShopSiloAppFSD.Repository;
using ShopSiloAppFSD.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ShopSiloAppFSD.Interfaces;

namespace ShopSiloApp.Tests
{
    [TestFixture]
    public class AuditLogRepositoryTests
    {
        private ShopSiloDBContext _context;
        private AuditLogRepository _auditLogRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAuditLogConfiguration> _mockAuditLogConfig;

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

            // Initialize repository with mocks and in-memory context
            _auditLogRepository = new AuditLogRepository(_context, _mockHttpContextAccessor.Object, _mockAuditLogConfig.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Ensure clean slate for each test
            _context.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Test]
        public async Task LogUserActionAsync_AuditLogDisabled_NoLogCreated()
        {
            // Arrange
            _mockAuditLogConfig.Setup(x => x.IsAuditLogEnabled).Returns(false);

            // Act
            await _auditLogRepository.LogUserActionAsync("Test Action");

            // Assert
            var logs = await _context.AuditLogs.ToListAsync();
            Assert.IsEmpty(logs);
        }

        [Test]
        public void LogUserActionAsync_NullOrEmptyAction_ThrowsArgumentException()
        {
            // Arrange
            _mockAuditLogConfig.Setup(x => x.IsAuditLogEnabled).Returns(true);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _auditLogRepository.LogUserActionAsync(null));
            Assert.AreEqual("Action cannot be null or empty. (Parameter 'action')", ex.Message);
        }

        [Test]
        public async Task LogUserActionAsync_ValidUser_CreatesAuditLog()
        {
            // Arrange
            _mockAuditLogConfig.Setup(x => x.IsAuditLogEnabled).Returns(true);

            // Mock user with ClaimsPrincipal
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testuser")
            }));
            var httpContext = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Seed a user in the context
            var seededUser = new User { UserID = 1, Username = "testuser", Email = "test@example.com", Password = "testpassword" };
            _context.Users.Add(seededUser);
            await _context.SaveChangesAsync();

            // Act
            await _auditLogRepository.LogUserActionAsync("Test Action");

            // Assert
            var logs = await _context.AuditLogs.ToListAsync();
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("Test Action", logs.First().Action);
            Assert.AreEqual(1, logs.First().UserId);
        }

       

        [Test]
        public async Task GetUserActivityLogsAsync_ValidUserId_ReturnsLogs()
        {
            // Arrange
            // Seed a user and audit log
            var user = new User { UserID = 1, Username = "testuser", Email = "test@example.com", Password = "testpassword" };
            var auditLog = new AuditLog { UserId = 1, Action = "Test Action", Timestamp = DateTime.UtcNow };
            _context.Users.Add(user);
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            // Act
            var logs = await _auditLogRepository.GetUserActivityLogsAsync(1);

            // Assert
            Assert.AreEqual(1, logs.Count());
            Assert.AreEqual("Test Action", logs.First().Action);
        }

        [Test]
        public void GetUserActivityLogsAsync_ThrowsRepositoryExceptionOnFailure()
        {
            // Arrange
            // Simulate exception by passing invalid DbContext (e.g., setting context to null)
            var invalidRepo = new AuditLogRepository(null, _mockHttpContextAccessor.Object, _mockAuditLogConfig.Object);

            // Act & Assert
            Assert.ThrowsAsync<RepositoryException>(async () =>
                await invalidRepo.GetUserActivityLogsAsync(1));
        }

    }
}
