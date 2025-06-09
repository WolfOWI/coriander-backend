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
using Microsoft.AspNetCore.Http;

namespace CoriCore.Tests.Unit.Services
{
    public class AuthAdminTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IImageService> _mockImageService;
        private readonly AppDbContext _context;
        private readonly AuthServices _service;

        public AuthAdminTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;
            _context = new AppDbContext(options);
            _mockUserService = new Mock<IUserService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockImageService = new Mock<IImageService>();
            _service = new AuthServices(_context, _mockUserService.Object, _mockEmailService.Object, _mockImageService.Object);
        }

        [Fact]
        public async Task RegisterAdminVerifiedAsync_ReturnsError_WhenEmailNotFound()
        {
            // Arrange
            var dto = new RegisterVerifiedDTO
            {
                Email = "nonexistent@example.com",
                FullName = "Test Admin",
                Password = "Password123!",
                Code = "123456"
            };

            // Act
            var (code, message, isCreated, canSignIn) = await _service.RegisterAdminVerifiedAsync(dto);

            // Assert
            Assert.Equal(404, code);
            Assert.Contains("not found", message.ToLower());
            Assert.False(isCreated);
            Assert.False(canSignIn);
        }

        [Fact]
        public async Task RegisterAdminVerifiedAsync_ReturnsError_WhenAlreadyVerified()
        {
            // Arrange
            var user = new User
            {
                Email = "admin@example.com",
                IsVerified = true
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new RegisterVerifiedDTO
            {
                Email = "admin@example.com",
                FullName = "Test Admin",
                Password = "Password123!",
                Code = "123456"
            };

            // Act
            var (code, message, isCreated, canSignIn) = await _service.RegisterAdminVerifiedAsync(dto);

            // Assert
            Assert.Equal(409, code);
            Assert.Contains("already verified", message.ToLower());
            Assert.False(isCreated);
            Assert.True(canSignIn);
        }

        [Fact]
        public async Task RegisterAdminVerifiedAsync_ReturnsError_WhenCodeInvalid()
        {
            // Arrange
            var user = new User
            {
                Email = "admin@example.com",
                IsVerified = false,
                VerificationCode = "123456",
                CodeGeneratedAt = DateTime.UtcNow
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new RegisterVerifiedDTO
            {
                Email = "admin@example.com",
                FullName = "Test Admin",
                Password = "Password123!",
                Code = "654321" // Different code
            };

            // Act
            var (code, message, isCreated, canSignIn) = await _service.RegisterAdminVerifiedAsync(dto);

            // Assert
            Assert.Equal(401, code);
            Assert.Contains("invalid", message.ToLower());
            Assert.False(isCreated);
            Assert.False(canSignIn);
        }

        [Fact]
        public async Task RegisterAdminVerifiedAsync_ReturnsError_WhenCodeExpired()
        {
            // Arrange
            var user = new User
            {
                Email = "admin@example.com",
                IsVerified = false,
                VerificationCode = "123456",
                CodeGeneratedAt = DateTime.UtcNow.AddMinutes(-11) // Code expires after 10 minutes
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new RegisterVerifiedDTO
            {
                Email = "admin@example.com",
                FullName = "Test Admin",
                Password = "Password123!",
                Code = "123456"
            };

            // Act
            var (code, message, isCreated, canSignIn) = await _service.RegisterAdminVerifiedAsync(dto);

            // Assert
            Assert.Equal(410, code);
            Assert.Contains("expired", message.ToLower());
            Assert.False(isCreated);
            Assert.False(canSignIn);
        }

        [Fact]
        public async Task RegisterAdminVerifiedAsync_Succeeds_WhenAllValid()
        {
            // Arrange
            var user = new User
            {
                Email = "admin@example.com",
                IsVerified = false,
                VerificationCode = "123456",
                CodeGeneratedAt = DateTime.UtcNow
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new RegisterVerifiedDTO
            {
                Email = "admin@example.com",
                FullName = "Test Admin",
                Password = "Password123!",
                Code = "123456"
            };

            // Act
            var (code, message, isCreated, canSignIn) = await _service.RegisterAdminVerifiedAsync(dto);

            // Assert
            Assert.Equal(200, code);
            Assert.Contains("success", message.ToLower());
            Assert.True(isCreated);
            Assert.True(canSignIn);

            var updatedUser = await _context.Users.Include(u => u.Admin).FirstAsync(u => u.Email == dto.Email);
            Assert.Equal(dto.FullName, updatedUser.FullName);
            Assert.Equal(UserRole.Admin, updatedUser.Role);
            Assert.True(updatedUser.IsVerified);
            Assert.Null(updatedUser.VerificationCode);
            Assert.Null(updatedUser.CodeGeneratedAt);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, updatedUser.Password));
            Assert.NotNull(updatedUser.Admin);
        }

        [Fact]
        public async Task RegisterAdminVerifiedAsync_HandlesProfileImage_WhenProvided()
        {
            // Arrange
            var user = new User
            {
                Email = "admin@example.com",
                IsVerified = false,
                VerificationCode = "123456",
                CodeGeneratedAt = DateTime.UtcNow
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var mockImagePath = "uploads/profile123.jpg";
            _mockImageService
                .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(mockImagePath);

            var dto = new RegisterVerifiedDTO
            {
                Email = "admin@example.com",
                FullName = "Test Admin",
                Password = "Password123!",
                Code = "123456",
                ProfileImage = Mock.Of<IFormFile>()
            };

            // Act
            await _service.RegisterAdminVerifiedAsync(dto);

            // Assert
            var updatedUser = await _context.Users.FirstAsync(u => u.Email == dto.Email);
            Assert.Equal(mockImagePath, updatedUser.ProfilePicture);
            _mockImageService.Verify(s => s.UploadImageAsync(It.IsAny<IFormFile>()), Times.Once);
        }
    }
}