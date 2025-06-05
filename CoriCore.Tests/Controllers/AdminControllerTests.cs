using System;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class AdminControllerTests
{
    private readonly AdminController _controller;
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly AppDbContext _context;

    public AdminControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _mockAdminService = new Mock<IAdminService>();
        _mockUserService = new Mock<IUserService>();
        _controller = new AdminController(_context, _mockAdminService.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task PromoteExistingUserToAdmin_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var adminDto = new AdminDTO { UserId = 1 };
        var successMessage = "Admin created successfully";

        _mockUserService.Setup(x => x.SetUserRoleAsync(adminDto.UserId, (int)UserRole.Admin))
            .ReturnsAsync(201);
        _mockAdminService.Setup(x => x.CreateAdmin(adminDto))
            .ReturnsAsync(successMessage);

        // Act
        var result = await _controller.PromoteExistingUserToAdmin(adminDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(successMessage, okResult.Value);
        _mockUserService.Verify(x => x.SetUserRoleAsync(adminDto.UserId, (int)UserRole.Admin), Times.Once);
        _mockAdminService.Verify(x => x.CreateAdmin(adminDto), Times.Once);
    }

    [Fact]
    public async Task PromoteExistingUserToAdmin_ReturnsBadRequest_WhenRoleSetFails()
    {
        // Arrange
        var adminDto = new AdminDTO { UserId = 1 };

        _mockUserService.Setup(x => x.SetUserRoleAsync(adminDto.UserId, (int)UserRole.Admin))
            .ReturnsAsync(400);

        // Act
        var result = await _controller.PromoteExistingUserToAdmin(adminDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Failed to set user role to admin", badRequestResult.Value);
        _mockUserService.Verify(x => x.SetUserRoleAsync(adminDto.UserId, (int)UserRole.Admin), Times.Once);
        _mockAdminService.Verify(x => x.CreateAdmin(It.IsAny<AdminDTO>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAdmins_ReturnsOkWithAdmins()
    {
        // Arrange
        var admins = new List<AdminUserDTO>
        {
            new AdminUserDTO { AdminId = 1, FullName = "Admin One" },
            new AdminUserDTO { AdminId = 2, FullName = "Admin Two" }
        };

        _mockAdminService.Setup(x => x.GetAllAdmins())
            .ReturnsAsync(admins);

        // Act
        var result = await _controller.GetAllAdmins();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAdmins = Assert.IsAssignableFrom<List<AdminUserDTO>>(okResult.Value);
        Assert.Equal(2, returnedAdmins.Count);
        _mockAdminService.Verify(x => x.GetAllAdmins(), Times.Once);
    }

    [Fact]
    public async Task GetAdminUserByAdminId_ReturnsOk_WhenAdminExists()
    {
        // Arrange
        var adminId = 1;
        var adminUser = new AdminUserDTO
        {
            AdminId = adminId,
            FullName = "Test Admin",
            Email = "admin@example.com"
        };

        _mockAdminService.Setup(x => x.GetAdminUserByAdminId(adminId))
            .ReturnsAsync(adminUser);

        // Act
        var result = await _controller.GetAdminUserByAdminId(adminId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAdmin = Assert.IsType<AdminUserDTO>(okResult.Value);
        Assert.Equal(adminId, returnedAdmin.AdminId);
        _mockAdminService.Verify(x => x.GetAdminUserByAdminId(adminId), Times.Once);
    }

    [Fact]
    public async Task GetAdminUserByAdminId_ReturnsNotFound_WhenAdminDoesNotExist()
    {
        // Arrange
        var adminId = 999;

        _mockAdminService.Setup(x => x.GetAdminUserByAdminId(adminId))
            .ReturnsAsync((AdminUserDTO)null);

        // Act
        var result = await _controller.GetAdminUserByAdminId(adminId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockAdminService.Verify(x => x.GetAdminUserByAdminId(adminId), Times.Once);
    }
}