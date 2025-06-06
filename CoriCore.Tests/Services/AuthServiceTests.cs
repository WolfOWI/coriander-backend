using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using CoriCore.Services;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Interfaces;

namespace CoriCore.Tests.Unit.Services
{
    public class AuthServicesTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task RegisterWithEmail_CreatesUser_WhenEmailNotExists()
        {
            // Arrange
            var context = GetDbContext();
            var mockUserService = new Mock<IUserService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockImageService = new Mock<IImageService>();

            mockUserService.Setup(s => s.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var service = new AuthServices(context, mockUserService.Object, mockEmailService.Object, mockImageService.Object);

            var dto = new UserEmailRegisterDTO
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                ProfilePicture = null,
                Role = UserRole.Employee
            };

            // Act
            var result = await service.RegisterWithEmail(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.FullName, result.FullName);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.Role, result.Role);
            Assert.NotNull(result.Password);
            Assert.NotEqual(dto.Password, result.Password); // Should be hashed
        }

        [Fact]
        public async Task RegisterWithEmail_ThrowsException_WhenEmailExists()
        {
            // Arrange
            var context = GetDbContext();
            var mockUserService = new Mock<IUserService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockImageService = new Mock<IImageService>();

            mockUserService.Setup(s => s.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User { Email = "test@example.com" });

            var service = new AuthServices(context, mockUserService.Object, mockEmailService.Object, mockImageService.Object);

            var dto = new UserEmailRegisterDTO
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                ProfilePicture = null,
                Role = UserRole.Employee
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.RegisterWithEmail(dto));
            Assert.Contains("already exists", ex.Message);
        }
    }
}