using System;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Tests.Unit.Controllers;

public class LeaveTypeControllerTests
{
    private readonly LeaveTypeController _controller;
    private readonly AppDbContext _context;

    public LeaveTypeControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _controller = new LeaveTypeController(_context);
    }

    [Fact]
    public async Task GetLeaveTypes_ReturnsOkWithLeaveTypes()
    {
        // Arrange
        var leaveTypes = new List<LeaveType>
        {
            new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Annual Leave", DefaultDays = 21 },
            new LeaveType { LeaveTypeId = 2, LeaveTypeName = "Sick Leave", DefaultDays = 30 },
            new LeaveType { LeaveTypeId = 3, LeaveTypeName = "Maternity Leave", DefaultDays = 120 }
        };

        _context.LeaveTypes.AddRange(leaveTypes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetLeaveTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLeaveTypes = Assert.IsAssignableFrom<IEnumerable<LeaveType>>(okResult.Value);
        Assert.Equal(3, returnedLeaveTypes.Count());

        var firstLeaveType = returnedLeaveTypes.First();
        Assert.Equal("Annual Leave", firstLeaveType.LeaveTypeName);
        Assert.Equal(21, firstLeaveType.DefaultDays);
    }

    [Fact]
    public async Task GetLeaveType_ReturnsOk_WhenLeaveTypeExists()
    {
        // Arrange
        var leaveType = new LeaveType
        {
            LeaveTypeId = 1,
            LeaveTypeName = "Study Leave",
            DefaultDays = 10
        };
        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetLeaveType(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLeaveType = Assert.IsType<LeaveType>(okResult.Value);
        Assert.Equal(leaveType.LeaveTypeId, returnedLeaveType.LeaveTypeId);
        Assert.Equal(leaveType.LeaveTypeName, returnedLeaveType.LeaveTypeName);
        Assert.Equal(leaveType.DefaultDays, returnedLeaveType.DefaultDays);
    }

    [Fact]
    public async Task GetLeaveType_ReturnsNotFound_WhenLeaveTypeDoesNotExist()
    {
        // Arrange & Act
        var result = await _controller.GetLeaveType(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetLeaveTypes_ReturnsEmptyList_WhenNoLeaveTypesExist()
    {
        // Arrange - No leave types added

        // Act
        var result = await _controller.GetLeaveTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLeaveTypes = Assert.IsAssignableFrom<IEnumerable<LeaveType>>(okResult.Value);
        Assert.Empty(returnedLeaveTypes);
    }

    [Fact]
    public async Task GetLeaveTypes_ReturnsCorrectData_ForSouthAfricanLeaveTypes()
    {
        // Arrange - Create some South African leave types
        var leaveTypes = new List<LeaveType>
        {
            new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Annual Leave", DefaultDays = 21 },
            new LeaveType { LeaveTypeId = 2, LeaveTypeName = "Family Responsibility Leave", DefaultDays = 3 },
            new LeaveType { LeaveTypeId = 3, LeaveTypeName = "Compassionate Leave", DefaultDays = 3 }
        };

        _context.LeaveTypes.AddRange(leaveTypes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetLeaveTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLeaveTypes = Assert.IsAssignableFrom<IEnumerable<LeaveType>>(okResult.Value);
        Assert.Equal(3, returnedLeaveTypes.Count());

        var familyLeave = returnedLeaveTypes.FirstOrDefault(lt => lt.LeaveTypeName == "Family Responsibility Leave");
        Assert.NotNull(familyLeave);
        Assert.Equal(3, familyLeave.DefaultDays);
    }
}