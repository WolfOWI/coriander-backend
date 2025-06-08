using System;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly AppDbContext _context;
    private readonly UserService _service;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _service = new UserService(_context);
    }

    [Fact]
    public async Task EmployeeAdminExistsAsync_ReturnsCode400_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;

        // Act
        var result = await _service.EmployeeAdminExistsAsync(userId);

        // Assert
        Assert.Equal(400, result);
    }

    [Fact]
    public async Task EmployeeAdminExistsAsync_ReturnsCode201_WhenUserExistsAndNotLinked()
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

        // Act
        var result = await _service.EmployeeAdminExistsAsync(user.UserId);

        // Assert
        Assert.Equal(201, result);
    }

    [Fact]
    public async Task EmployeeAdminExistsAsync_ReturnsCode400_WhenUserAlreadyLinkedAsEmployee()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Employee
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = user.UserId,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        _context.Users.Add(user);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.EmployeeAdminExistsAsync(user.UserId);

        // Assert
        Assert.Equal(400, result);
    }

    [Fact]
    public async Task SetUserRoleAsync_ReturnsCode400_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 999;
        var userRole = (int)UserRole.Admin;

        // Act
        var result = await _service.SetUserRoleAsync(userId, userRole);

        // Assert
        Assert.Equal(400, result);
    }

    [Fact]
    public async Task SetUserRoleAsync_ReturnsCode201_WhenRoleSetSuccessfully()
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

        var newRole = (int)UserRole.Admin;

        // Act
        var result = await _service.SetUserRoleAsync(user.UserId, newRole);

        // Assert
        Assert.Equal(201, result);

        var updatedUser = await _context.Users.FindAsync(user.UserId);
        Assert.Equal(UserRole.Admin, updatedUser.Role);
    }

    [Fact]
    public async Task GetUserRoleAsync_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetUserRoleAsync(userId));
    }

    [Fact]
    public async Task GetUserRoleAsync_ReturnsCorrectRole()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Admin
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserRoleAsync(user.UserId);

        // Assert
        Assert.Equal(UserRole.Admin, result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var email = "notfound@example.com";

        // Act
        var result = await _service.GetUserByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenUserExists()
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

        // Act
        var result = await _service.GetUserByEmailAsync(user.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FullName, result.FullName);
    }

    [Fact]
    public async Task GetUnlinkedUsersAsync_ReturnsOnlyUnlinkedUsers()
    {
        // Arrange
        var linkedUser = new User
        {
            UserId = 1,
            FullName = "Linked User",
            Email = "linked@example.com",
            Role = UserRole.Employee
        };

        var unlinkedUser = new User
        {
            UserId = 2,
            FullName = "Unlinked User",
            Email = "unlinked@example.com",
            Role = UserRole.Employee
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = linkedUser.UserId,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        _context.Users.AddRange(linkedUser, unlinkedUser);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUnlinkedUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(unlinkedUser.UserId, result.First().UserId);
        Assert.Equal(unlinkedUser.Email, result.First().Email);
    }
}