using System;
using CoriCore.Controllers;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class GatheringControllerTests
{
    private readonly GatheringController _controller;
    private readonly Mock<IGatheringService> _mockGatheringService;

    public GatheringControllerTests()
    {
        _mockGatheringService = new Mock<IGatheringService>();
        _controller = new GatheringController(_mockGatheringService.Object);
    }

    [Fact]
    public async Task GetAllGatheringsByEmpId_ReturnsOkWithGatherings()
    {
        // Arrange
        var employeeId = 1;
        var gatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 1, Purpose = "Team Meeting", Type = GatheringType.Meeting },
            new GatheringDTO { Id = 2, Purpose = "Performance Review", Type = GatheringType.PerformanceReview }
        };

        _mockGatheringService.Setup(x => x.GetAllGatheringsByEmployeeId(employeeId))
            .ReturnsAsync(gatherings);

        // Act
        var result = await _controller.GetAllGatheringsByEmpId(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Equal(2, returnedGatherings.Count());
        _mockGatheringService.Verify(x => x.GetAllGatheringsByEmployeeId(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetAllUpcomingGatheringsByEmpId_ReturnsOkWithUpcomingGatherings()
    {
        // Arrange
        var employeeId = 1;
        var upcomingGatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 1, Purpose = "Upcoming Meeting", Type = GatheringType.Meeting }
        };

        _mockGatheringService.Setup(x => x.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Upcoming"))
            .ReturnsAsync(upcomingGatherings);

        // Act
        var result = await _controller.GetAllUpcomingGatheringsByEmpId(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Single(returnedGatherings);
        _mockGatheringService.Verify(x => x.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Upcoming"), Times.Once);
    }

    [Fact]
    public async Task GetAllCompletedGatheringsByEmpId_ReturnsOkWithCompletedGatherings()
    {
        // Arrange
        var employeeId = 1;
        var completedGatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 1, Purpose = "Completed Meeting", Type = GatheringType.Meeting },
            new GatheringDTO { Id = 2, Purpose = "Completed Review", Type = GatheringType.PerformanceReview }
        };

        _mockGatheringService.Setup(x => x.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Completed"))
            .ReturnsAsync(completedGatherings);

        // Act
        var result = await _controller.GetAllCompletedGatheringsByEmpId(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Equal(2, returnedGatherings.Count());
        _mockGatheringService.Verify(x => x.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Completed"), Times.Once);
    }

    [Fact]
    public async Task GetAllUpcomingAndCompletedGatheringsByEmpIdDescending_ReturnsOk()
    {
        // Arrange
        var employeeId = 1;
        var gatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 3, Purpose = "Recent Meeting", Type = GatheringType.Meeting },
            new GatheringDTO { Id = 1, Purpose = "Older Meeting", Type = GatheringType.Meeting }
        };

        _mockGatheringService.Setup(x => x.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId))
            .ReturnsAsync(gatherings);

        // Act
        var result = await _controller.GetAllUpcomingAndCompletedGatheringsByEmpIdDescending(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Equal(2, returnedGatherings.Count());
        _mockGatheringService.Verify(x => x.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetAllUpcomingGatheringsByAdminId_ReturnsOkWithUpcomingGatherings()
    {
        // Arrange
        var adminId = 1;
        var upcomingGatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 1, Purpose = "Admin Meeting", Type = GatheringType.Meeting }
        };

        _mockGatheringService.Setup(x => x.GetAllGatheringsByAdminIdAndStatus(adminId, "Upcoming"))
            .ReturnsAsync(upcomingGatherings);

        // Act
        var result = await _controller.GetAllUpcomingGatheringsByAdminId(adminId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Single(returnedGatherings);
        _mockGatheringService.Verify(x => x.GetAllGatheringsByAdminIdAndStatus(adminId, "Upcoming"), Times.Once);
    }

    [Fact]
    public async Task GetAllCompletedGatheringsByAdminId_ReturnsOkWithCompletedGatherings()
    {
        // Arrange
        var adminId = 1;
        var completedGatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 1, Purpose = "Completed Admin Meeting", Type = GatheringType.Meeting }
        };

        _mockGatheringService.Setup(x => x.GetAllGatheringsByAdminIdAndStatus(adminId, "Completed"))
            .ReturnsAsync(completedGatherings);

        // Act
        var result = await _controller.GetAllCompletedGatheringsByAdminId(adminId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Single(returnedGatherings);
        _mockGatheringService.Verify(x => x.GetAllGatheringsByAdminIdAndStatus(adminId, "Completed"), Times.Once);
    }

    [Fact]
    public async Task GetGatheringsByAdminIdAndMonth_ReturnsOk_WhenValidMonth()
    {
        // Arrange
        var adminId = 1;
        var month = "03"; // March
        var gatherings = new List<GatheringDTO>
        {
            new GatheringDTO { Id = 1, Purpose = "March Meeting", Type = GatheringType.Meeting }
        };

        _mockGatheringService.Setup(x => x.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, month))
            .ReturnsAsync(gatherings);

        // Act
        var result = await _controller.GetGatheringsByAdminIdAndMonth(adminId, month);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGatherings = Assert.IsAssignableFrom<IEnumerable<GatheringDTO>>(okResult.Value);
        Assert.Single(returnedGatherings);
        _mockGatheringService.Verify(x => x.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, month), Times.Once);
    }

    [Fact]
    public async Task GetGatheringsByAdminIdAndMonth_ReturnsBadRequest_WhenInvalidMonth()
    {
        // Arrange
        var adminId = 1;
        var invalidMonth = "invalid";
        var errorMessage = "Invalid month format";

        _mockGatheringService.Setup(x => x.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, invalidMonth))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.GetGatheringsByAdminIdAndMonth(adminId, invalidMonth);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(errorMessage, badRequestResult.Value);
        _mockGatheringService.Verify(x => x.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, invalidMonth), Times.Once);
    }
}