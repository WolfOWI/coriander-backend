using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoriCore.Tests.Unit.Services;

public class AdminServiceTests
{
    private readonly AppDbContext _context;
    private readonly AdminService _service;
    private readonly Mock<IUserService> _mockUserService;

    public AdminServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _mockUserService = new Mock<IUserService>();

        _service = new AdminService(_context, _mockUserService.Object);
    }

    [Fact]
    public async Task CreateAdmin_ReturnsErrorMessage_WhenUserNotFound()
    {
        // Arrange
        var adminDto = new AdminDTO
        {
            UserId = 999
        };

        // Act
        var result = await _service.CreateAdmin(adminDto);

        // Assert
        Assert.Equal("A user with that id could not be found", result);
    }

    [Fact]
    public async Task CreateAdmin_ReturnsErrorMessage_WhenUserAlreadyAdmin()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Admin
        };

        var existingAdmin = new Admin
        {
            AdminId = 1,
            UserId = 1,
            User = user
        };

        _context.Users.Add(user);
        _context.Admins.Add(existingAdmin);
        await _context.SaveChangesAsync();

        var adminDto = new AdminDTO
        {
            UserId = 1
        };

        // Act
        var result = await _service.CreateAdmin(adminDto);

        // Assert
        Assert.Equal("The user is already an admin", result);
    }

    [Fact]
    public async Task CreateAdmin_CreatesSuccessfully_WhenValidUser()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Employee
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var adminDto = new AdminDTO
        {
            UserId = 1
        };

        // Act
        var result = await _service.CreateAdmin(adminDto);

        // Assert
        Assert.Equal("Admin created successfully", result);

        var adminInDb = await _context.Admins.FirstAsync();
        Assert.Equal(adminDto.UserId, adminInDb.UserId);
    }

    [Fact]
    public async Task GetAllAdmins_ReturnsEmptyList_WhenNoAdminsExist()
    {
        // Arrange & Act
        var result = await _service.GetAllAdmins();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAdmins_ReturnsAdminList_WhenAdminsExist()
    {
        // Arrange
        var user1 = new User
        {
            UserId = 1,
            FullName = "Admin One",
            Email = "admin1@example.com",
            Role = UserRole.Admin
        };

        var user2 = new User
        {
            UserId = 2,
            FullName = "Admin Two",
            Email = "admin2@example.com",
            Role = UserRole.Admin
        };

        var admin1 = new Admin
        {
            AdminId = 1,
            UserId = 1,
            User = user1
        };

        var admin2 = new Admin
        {
            AdminId = 2,
            UserId = 2,
            User = user2
        };

        _context.Users.AddRange(user1, user2);
        _context.Admins.AddRange(admin1, admin2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAdmins();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var firstAdmin = result.First(a => a.AdminId == 1);
        Assert.Equal(user1.FullName, firstAdmin.FullName);
        Assert.Equal(user1.Email, firstAdmin.Email);

        var secondAdmin = result.First(a => a.AdminId == 2);
        Assert.Equal(user2.FullName, secondAdmin.FullName);
        Assert.Equal(user2.Email, secondAdmin.Email);
    }

    [Fact]
    public async Task GetAdminUserByAdminId_ReturnsNull_WhenAdminNotFound()
    {
        // Arrange
        var adminId = 999;

        // Act
        var result = await _service.GetAdminUserByAdminId(adminId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminUserByAdminId_ReturnsNull_WhenAdminHasNoUser()
    {
        // Arrange
        var admin = new Admin
        {
            AdminId = 1,
            UserId = 1
            // No User entity linked
        };

        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAdminUserByAdminId(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminUserByAdminId_ReturnsAdminUser_WhenAdminExists()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            FullName = "Admin User",
            Email = "admin@example.com",
            Role = UserRole.Admin
        };

        var admin = new Admin
        {
            AdminId = 1,
            UserId = 1,
            User = user
        };

        _context.Users.Add(user);
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAdminUserByAdminId(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(admin.AdminId, result.AdminId);
        Assert.Equal(user.UserId, result.UserId);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.Email, result.Email);
    }
}