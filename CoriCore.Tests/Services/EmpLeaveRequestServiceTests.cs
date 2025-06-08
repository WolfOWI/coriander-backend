// CoriCore.Tests/Services/EmpLeaveRequestServiceTests.cs
using System;
using System.Threading.Tasks;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoriCore.Tests.Services;

/// <summary>
/// Tests for the EmpLeaveRequestService class.
/// </summary>
public class EmpLeaveRequestServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ILeaveBalanceService> _mockBalanceService;
    private readonly EmpLeaveRequestService _service;

    public EmpLeaveRequestServiceTests()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(opts);
        _mockBalanceService = new Mock<ILeaveBalanceService>();
        _service = new EmpLeaveRequestService(_context, _mockBalanceService.Object);
    }

    [Fact]
    public async Task ApproveLeaveRequestById_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.ApproveLeaveRequestById(999);
        Assert.False(result);
    }

    [Fact]
    public async Task ApproveLeaveRequestById_ReturnsFalse_WhenSubtractFails()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lr = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 50,
            LeaveTypeId = 60,
            StartDate = today,
            EndDate = today.AddDays(2),
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _context.LeaveRequests.AddAsync(lr);
        await _context.SaveChangesAsync();

        // duration = 3 days
        _mockBalanceService
            .Setup(s => s.SubtractLeaveRequestDays(50, 60, 3))
            .ReturnsAsync(false);

        // Act
        var ok = await _service.ApproveLeaveRequestById(1);

        // Assert
        Assert.False(ok);
        var updated = await _context.LeaveRequests.FindAsync(1);
        Assert.Equal(LeaveStatus.Pending, updated.Status);
    }

    [Fact]
    public async Task ApproveLeaveRequestById_Succeeds_WhenSubtractSucceeds()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lr = new LeaveRequest
        {
            LeaveRequestId = 2,
            EmployeeId = 100,
            LeaveTypeId = 200,
            StartDate = today,
            EndDate = today.AddDays(4),
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _context.LeaveRequests.AddAsync(lr);
        await _context.SaveChangesAsync();

        // duration = 5 days
        _mockBalanceService
            .Setup(s => s.SubtractLeaveRequestDays(100, 200, 5))
            .ReturnsAsync(true);

        // Act
        var ok = await _service.ApproveLeaveRequestById(2);

        // Assert
        Assert.True(ok);
        _mockBalanceService.Verify(s => s.SubtractLeaveRequestDays(100, 200, 5), Times.Once);
        var updated = await _context.LeaveRequests.FindAsync(2);
        Assert.Equal(LeaveStatus.Approved, updated.Status);
    }

    [Fact]
    public async Task RejectLeaveRequestById_ReturnsFalse_WhenNotFound()
    {
        var ok = await _service.RejectLeaveRequestById(999);
        Assert.False(ok);
    }

    [Fact]
    public async Task RejectLeaveRequestById_Succeeds_WhenFound()
    {
        var lr = new LeaveRequest
        {
            LeaveRequestId = 3,
            Status = LeaveStatus.Pending
        };
        await _context.LeaveRequests.AddAsync(lr);
        await _context.SaveChangesAsync();

        var ok = await _service.RejectLeaveRequestById(3);

        Assert.True(ok);
        var updated = await _context.LeaveRequests.FindAsync(3);
        Assert.Equal(LeaveStatus.Rejected, updated.Status);
    }

    [Fact]
    public async Task SetLeaveRequestToPendingById_ReturnsFalse_WhenNotFound()
    {
        var ok = await _service.SetLeaveRequestToPendingById(999);
        Assert.False(ok);
    }

    [Fact]
    public async Task SetLeaveRequestToPendingById_Succeeds_WhenAlreadyPending()
    {
        var lr = new LeaveRequest
        {
            LeaveRequestId = 4,
            Status = LeaveStatus.Pending
        };
        await _context.LeaveRequests.AddAsync(lr);
        await _context.SaveChangesAsync();

        var ok = await _service.SetLeaveRequestToPendingById(4);

        Assert.True(ok);
        var updated = await _context.LeaveRequests.FindAsync(4);
        Assert.Equal(LeaveStatus.Pending, updated.Status);
        _mockBalanceService.Verify(
          s => s.AddLeaveRequestDays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
          Times.Never
        );
    }

    [Fact]
    public async Task SetLeaveRequestToPendingById_AddsDaysBack_WhenPreviouslyApproved_AndSucceeds()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lr = new LeaveRequest
        {
            LeaveRequestId = 5,
            EmployeeId = 20,
            LeaveTypeId = 30,
            StartDate = today.AddDays(-2),
            EndDate = today,
            Status = LeaveStatus.Approved
        };
        await _context.LeaveRequests.AddAsync(lr);
        await _context.SaveChangesAsync();

        // duration = 3 days
        _mockBalanceService
            .Setup(s => s.AddLeaveRequestDays(20, 30, 3))
            .ReturnsAsync(true);

        var ok = await _service.SetLeaveRequestToPendingById(5);

        Assert.True(ok);
        _mockBalanceService.Verify(s => s.AddLeaveRequestDays(20, 30, 3), Times.Once);
        var updated = await _context.LeaveRequests.FindAsync(5);
        Assert.Equal(LeaveStatus.Pending, updated.Status);
    }

    [Fact]
    public async Task SetLeaveRequestToPendingById_ReturnsFalse_WhenAddDaysFails()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lr = new LeaveRequest
        {
            LeaveRequestId = 6,
            EmployeeId = 40,
            LeaveTypeId = 50,
            StartDate = today.AddDays(-1),
            EndDate = today,
            Status = LeaveStatus.Approved
        };
        await _context.LeaveRequests.AddAsync(lr);
        await _context.SaveChangesAsync();

        // simulate failure
        _mockBalanceService
            .Setup(s => s.AddLeaveRequestDays(40, 50, 2))
            .ReturnsAsync(false);

        var ok = await _service.SetLeaveRequestToPendingById(6);

        Assert.False(ok);
        // status unchanged
        var updated = await _context.LeaveRequests.FindAsync(6);
        Assert.Equal(LeaveStatus.Approved, updated.Status);
    }
}
