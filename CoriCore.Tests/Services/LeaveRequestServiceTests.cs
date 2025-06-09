using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.DTOs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoriCore.Tests.Services;

public class LeaveRequestServiceTests
{
    private readonly AppDbContext _context;
    private readonly LeaveRequestService _service;

    public LeaveRequestServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        _context = new AppDbContext(options);
        _service = new LeaveRequestService(_context);
    }

    private async Task<(Employee, User)> CreateTestEmployee()
    {
        var user = new User
        {
            UserId = 1,
            FullName = "Test User",
            Email = "test@example.com",
            Password = "hashedpassword",
            Role = UserRole.Employee
        };
        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = 1,
            User = user,
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT"
        };
        _context.Users.Add(user);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return (employee, user);
    }

    private async Task<LeaveType> CreateTestLeaveType()
    {
        var leaveType = new LeaveType
        {
            LeaveTypeId = 1,
            LeaveTypeName = "Annual",
            Description = "Annual Leave",
            DefaultDays = 20
        };
        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync();
        return leaveType;
    }

    [Fact]
    public async Task GetLeaveRequestsByEmployeeId_ReturnsEmptyList_WhenNoRequests()
    {
        // Arrange
        await CreateTestEmployee();

        // Act
        var result = await _service.GetLeaveRequestsByEmployeeId(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeaveRequestsByEmployeeId_ReturnsRequests_WhenExists()
    {
        // Arrange
        var (employee, user) = await CreateTestEmployee();
        var leaveType = await CreateTestLeaveType();
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            Employee = employee,
            LeaveTypeId = 1,
            LeaveType = leaveType,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Comment = "Test leave request",
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLeaveRequestsByEmployeeId(1);

        // Assert
        Assert.Single(result);
        var request = result.First();
        Assert.Equal(1, request.LeaveRequestId);
        Assert.Equal(1, request.EmployeeId);
        Assert.Equal("Test User", request.EmployeeName);
        Assert.Equal(1, request.LeaveTypeId);
        Assert.Equal("Annual", request.LeaveType);
        Assert.Equal("Annual Leave", request.Description);
        Assert.Equal(20, request.DefaultDays);
        Assert.Equal("Test leave request", request.Comment);
        Assert.Equal(LeaveStatus.Pending, request.Status);
    }

    [Fact]
    public async Task GetAllLeaveRequests_ReturnsEmptyList_WhenNoRequests()
    {
        // Act
        var result = await _service.GetAllLeaveRequests();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllLeaveRequests_ReturnsAllRequests()
    {
        // Arrange
        var (employee, user) = await CreateTestEmployee();
        var leaveType = await CreateTestLeaveType();
        var leaveRequests = new List<LeaveRequest>
        {
            new LeaveRequest
            {
                LeaveRequestId = 1,
                EmployeeId = 1,
                Employee = employee,
                LeaveTypeId = 1,
                LeaveType = leaveType,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                Comment = "First request",
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new LeaveRequest
            {
                LeaveRequestId = 2,
                EmployeeId = 1,
                Employee = employee,
                LeaveTypeId = 1,
                LeaveType = leaveType,
                StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                Comment = "Second request",
                Status = LeaveStatus.Approved,
                CreatedAt = DateTime.UtcNow
            }
        };
        await _context.LeaveRequests.AddRangeAsync(leaveRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllLeaveRequests();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Comment == "First request" && r.Status == LeaveStatus.Pending);
        Assert.Contains(result, r => r.Comment == "Second request" && r.Status == LeaveStatus.Approved);
    }

    [Fact]
    public async Task GetLeaveRequestsByEmployeeId_IncludesAllRequiredData()
    {
        // Arrange
        var (employee, user) = await CreateTestEmployee();
        var leaveType = await CreateTestLeaveType();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            Employee = employee,
            LeaveTypeId = 1,
            LeaveType = leaveType,
            StartDate = startDate,
            EndDate = endDate,
            Comment = "Test request",
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _context.LeaveRequests.AddAsync(leaveRequest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLeaveRequestsByEmployeeId(1);

        // Assert
        var request = Assert.Single(result);
        Assert.Equal(1, request.LeaveRequestId);
        Assert.Equal(1, request.EmployeeId);
        Assert.Equal("Test User", request.EmployeeName);
        Assert.Equal(1, request.LeaveTypeId);
        Assert.Equal("Annual", request.LeaveTypeName);
        Assert.Equal("Annual Leave", request.Description);
        Assert.Equal(20, request.DefaultDays);
        Assert.Equal(startDate, request.StartDate);
        Assert.Equal(endDate, request.EndDate);
        Assert.Equal("Test request", request.Comment);
        Assert.Equal(LeaveStatus.Pending, request.Status);
    }
}