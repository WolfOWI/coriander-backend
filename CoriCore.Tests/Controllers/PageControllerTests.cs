using System;
using CoriCore.Controllers;
using CoriCore.DTOs.Page_Specific;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class PageControllerTests
{
    private readonly PageController _controller;
    private readonly Mock<IPageService> _mockPageService;

    public PageControllerTests()
    {
        _mockPageService = new Mock<IPageService>();
        _controller = new PageController(_mockPageService.Object);
    }

    [Fact]
    public async Task GetAdminEmpDetailsPage_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var employeeId = 1;
        var pageInfo = new AdminEmpDetailsPageDTO();

        _mockPageService.Setup(x => x.GetAdminEmpDetailsPageInfo(employeeId))
            .ReturnsAsync(pageInfo);

        // Act
        var result = await _controller.GetAdminEmpDetailsPage(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(pageInfo, okResult.Value);
        _mockPageService.Verify(x => x.GetAdminEmpDetailsPageInfo(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetAdminEmpDetailsPage_ReturnsNotFound_WhenExceptionThrown()
    {
        // Arrange
        var employeeId = 1;
        var exceptionMessage = "Employee not found";

        _mockPageService.Setup(x => x.GetAdminEmpDetailsPageInfo(employeeId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetAdminEmpDetailsPage(employeeId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains(exceptionMessage, notFoundResult.Value.ToString());
        _mockPageService.Verify(x => x.GetAdminEmpDetailsPageInfo(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetAdminEmpManagementPage_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var pageInfo = new List<AdminEmpManagePageListItemDTO>
        {
            new AdminEmpManagePageListItemDTO(),
            new AdminEmpManagePageListItemDTO()
        };

        _mockPageService.Setup(x => x.GetAdminEmpManagementPageInfo())
            .ReturnsAsync(pageInfo);

        // Act
        var result = await _controller.GetAdminEmpManagementPage();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPageInfo = Assert.IsAssignableFrom<List<AdminEmpManagePageListItemDTO>>(okResult.Value);
        Assert.Equal(2, returnedPageInfo.Count);
        _mockPageService.Verify(x => x.GetAdminEmpManagementPageInfo(), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeProfilePage_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var employeeId = 1;
        var pageInfo = new EmployeeProfilePageDTO();

        _mockPageService.Setup(x => x.GetEmployeeProfilePageInfo(employeeId))
            .ReturnsAsync(pageInfo);

        // Act
        var result = await _controller.GetEmployeeProfilePage(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(pageInfo, okResult.Value);
        _mockPageService.Verify(x => x.GetEmployeeProfilePageInfo(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetAdminDashboardPage_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var adminId = 1;
        var pageInfo = new AdminDashboardPageDTO();

        _mockPageService.Setup(x => x.GetAdminDashboardPageInfo(adminId))
            .ReturnsAsync(pageInfo);

        // Act
        var result = await _controller.GetAdminDashboardPage(adminId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(pageInfo, okResult.Value);
        _mockPageService.Verify(x => x.GetAdminDashboardPageInfo(adminId), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeLeaveOverviewPage_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var employeeId = 1;
        var pageInfo = new EmployeeLeaveOverviewPageDTO();

        _mockPageService.Setup(x => x.GetEmployeeLeaveOverviewPageInfo(employeeId))
            .ReturnsAsync(pageInfo);

        // Act
        var result = await _controller.GetEmployeeLeaveOverviewPage(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(pageInfo, okResult.Value);
        _mockPageService.Verify(x => x.GetEmployeeLeaveOverviewPageInfo(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeLeaveOverviewPage_ReturnsNotFound_WhenExceptionThrown()
    {
        // Arrange
        var employeeId = 1;
        var exceptionMessage = "Error retrieving data";

        _mockPageService.Setup(x => x.GetEmployeeLeaveOverviewPageInfo(employeeId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetEmployeeLeaveOverviewPage(employeeId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains(exceptionMessage, notFoundResult.Value.ToString());
        _mockPageService.Verify(x => x.GetEmployeeLeaveOverviewPageInfo(employeeId), Times.Once);
    }
}