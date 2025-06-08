using System;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _mockUserService;
    private readonly AppDbContext _context;

    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_context, _mockUserService.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, FullName = "John Doe", Email = "john@example.com" },
            new User { UserId = 2, FullName = "Jane Smith", Email = "jane@example.com" }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count());
    }

    [Fact]
    public async Task GetUser_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var user = new User { UserId = 1, FullName = "John Doe", Email = "john@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUser(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(user.UserId, returnedUser.UserId);
        Assert.Equal(user.FullName, returnedUser.FullName);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange & Act
        var result = await _controller.GetUser(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PutUser_ReturnsNoContent_WhenUpdateSuccessful()
    {
        // Arrange
        var user = new User { UserId = 1, FullName = "John Doe", Email = "john@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.FullName = "John Updated";

        // Act
        var result = await _controller.PutUser(1, user);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var updatedUser = await _context.Users.FindAsync(1);
        Assert.Equal("John Updated", updatedUser.FullName);
    }

    [Fact]
    public async Task PutUser_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var user = new User { UserId = 1, FullName = "John Doe", Email = "john@example.com" };

        // Act
        var result = await _controller.PutUser(2, user);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
    {
        // Arrange
        var user = new User { UserId = 1, FullName = "John Doe", Email = "john@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var deletedUser = await _context.Users.FindAsync(1);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange & Act
        var result = await _controller.DeleteUser(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUnlinkedUsers_ReturnsOkWithUsers()
    {
        // Arrange
        var unlinkedUsers = new List<User>
        {
            new User { UserId = 1, FullName = "Unlinked User", Email = "unlinked@example.com" }
        };

        _mockUserService.Setup(x => x.GetUnlinkedUsersAsync())
            .ReturnsAsync(unlinkedUsers);

        // Act
        var result = await _controller.GetUnlinkedUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Single(returnedUsers);
        _mockUserService.Verify(x => x.GetUnlinkedUsersAsync(), Times.Once);
    }
}