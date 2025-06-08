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

        [Fact]
        public async Task LoginWithEmail_ReturnsJwt_WhenCredentialsAreValid()
        {
            // Arrange
            var context = GetDbContext();
            var mockUserService = new Mock<IUserService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockImageService = new Mock<IImageService>();

            var password = "TestPassword123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Email = "login@test.com",
                Password = hashedPassword,
                Role = UserRole.Employee
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new AuthServices(context, mockUserService.Object, mockEmailService.Object, mockImageService.Object);

            // Act
            var jwt = await service.LoginWithEmail(user.Email, password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(jwt));
        }

        [Fact]
        public async Task LoginWithEmail_ThrowsException_WhenUserNotFound()
        {
            var context = GetDbContext();
            var service = new AuthServices(context, new Mock<IUserService>().Object, new Mock<IEmailService>().Object, new Mock<IImageService>().Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.LoginWithEmail("nouser@test.com", "anyPassword"));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task LoginWithEmail_ThrowsException_WhenPasswordInvalid()
        {
            var context = GetDbContext();
            var user = new User
            {
                Email = "wrongpass@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
                Role = UserRole.Employee
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new AuthServices(context, new Mock<IUserService>().Object, new Mock<IEmailService>().Object, new Mock<IImageService>().Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.LoginWithEmail(user.Email, "WrongPassword"));
            Assert.Contains("Invalid password", ex.Message);
        }

        [Fact]
        public async Task VerifyPassword_ReturnsTrue_WhenPasswordMatches()
        {
            var context = GetDbContext();
            var service = new AuthServices(context, new Mock<IUserService>().Object, new Mock<IEmailService>().Object, new Mock<IImageService>().Object);
            var password = "Test123!";
            var hashed = await service.HashPassword(password);

            var user = new User { Password = hashed };

            var result = await service.VerifyPassword(user, password);
            Assert.True(result);
        }

        [Fact]
        public async Task VerifyPassword_ReturnsFalse_WhenPasswordDoesNotMatch()
        {
            var context = GetDbContext();
            var service = new AuthServices(context, new Mock<IUserService>().Object, new Mock<IEmailService>().Object, new Mock<IImageService>().Object);
            var hashed = await service.HashPassword("Correct123!");

            var user = new User { Password = hashed };

            var result = await service.VerifyPassword(user, "Wrong123!");
            Assert.False(result);
        }
    }
}