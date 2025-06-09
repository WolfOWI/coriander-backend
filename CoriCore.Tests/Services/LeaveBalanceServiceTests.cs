using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoriCore.Tests.Services;

public class LeaveBalanceServiceTests
{
    private readonly AppDbContext _context;
    private readonly LeaveBalanceService _service;

    public LeaveBalanceServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
        _context = new AppDbContext(options);
        _service = new LeaveBalanceService(_context);
    }

    private async Task<Employee> CreateTestEmployee()
    {
        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = 1,
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT"
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    private async Task<List<LeaveType>> CreateTestLeaveTypes()
    {
        var leaveTypes = new List<LeaveType>
        {
            new LeaveType { LeaveTypeId = 1, LeaveTypeName = "Annual", Description = "Annual Leave", DefaultDays = 20 },
            new LeaveType { LeaveTypeId = 2, LeaveTypeName = "Sick", Description = "Sick Leave", DefaultDays = 10 }
        };
        await _context.LeaveTypes.AddRangeAsync(leaveTypes);
        await _context.SaveChangesAsync();
        return leaveTypes;
    }

    [Fact]
    public async Task GetAllLeaveBalancesByEmployeeId_ReturnsEmptyList_WhenNoBalances()
    {
        // Act
        var result = await _service.GetAllLeaveBalancesByEmployeeId(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllLeaveBalancesByEmployeeId_ReturnsBalances_WhenExists()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();
        var leaveBalances = new List<LeaveBalance>
        {
            new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, RemainingDays = 20 },
            new LeaveBalance { EmployeeId = 1, LeaveTypeId = 2, RemainingDays = 10 }
        };
        await _context.LeaveBalances.AddRangeAsync(leaveBalances);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllLeaveBalancesByEmployeeId(1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, lb => lb.LeaveTypeName == "Annual" && lb.RemainingDays == 20);
        Assert.Contains(result, lb => lb.LeaveTypeName == "Sick" && lb.RemainingDays == 10);
    }

    [Fact]
    public async Task CreateDefaultLeaveBalances_ThrowsException_WhenEmployeeNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateDefaultLeaveBalances(999));
    }

    [Fact]
    public async Task CreateDefaultLeaveBalances_CreatesBalances_WhenEmployeeExists()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();

        // Act
        var result = await _service.CreateDefaultLeaveBalances(1);

        // Assert
        Assert.True(result);
        var balances = await _context.LeaveBalances.Where(lb => lb.EmployeeId == 1).ToListAsync();
        Assert.Equal(2, balances.Count);
        Assert.Contains(balances, lb => lb.LeaveTypeId == 1 && lb.RemainingDays == 20);
        Assert.Contains(balances, lb => lb.LeaveTypeId == 2 && lb.RemainingDays == 10);
    }

    [Fact]
    public async Task GetTotalLeaveBalanceSum_ReturnsCorrectSums()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();
        var leaveBalances = new List<LeaveBalance>
        {
            new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, RemainingDays = 15 }, // 20 default, 15 remaining
            new LeaveBalance { EmployeeId = 1, LeaveTypeId = 2, RemainingDays = 8 }   // 10 default, 8 remaining
        };
        await _context.LeaveBalances.AddRangeAsync(leaveBalances);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTotalLeaveBalanceSum(1);

        // Assert
        Assert.Equal(23, result.TotalRemainingDays); // 15 + 8
        Assert.Equal(30, result.TotalLeaveDays);     // 20 + 10
    }

    [Fact]
    public async Task SubtractLeaveRequestDays_ReturnsFalse_WhenBalanceNotFound()
    {
        // Act
        var result = await _service.SubtractLeaveRequestDays(1, 1, 5);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SubtractLeaveRequestDays_SetsToZero_WhenInsufficientDays()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();
        var leaveBalance = new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, RemainingDays = 5 };
        await _context.LeaveBalances.AddAsync(leaveBalance);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SubtractLeaveRequestDays(1, 1, 10);

        // Assert
        Assert.True(result);
        var updated = await _context.LeaveBalances.FirstAsync(lb => lb.EmployeeId == 1 && lb.LeaveTypeId == 1);
        Assert.Equal(0, updated.RemainingDays);
    }

    [Fact]
    public async Task SubtractLeaveRequestDays_SubtractsDays_WhenSufficientBalance()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();
        var leaveBalance = new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, RemainingDays = 20 };
        await _context.LeaveBalances.AddAsync(leaveBalance);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SubtractLeaveRequestDays(1, 1, 5);

        // Assert
        Assert.True(result);
        var updated = await _context.LeaveBalances.FirstAsync(lb => lb.EmployeeId == 1 && lb.LeaveTypeId == 1);
        Assert.Equal(15, updated.RemainingDays);
    }

    [Fact]
    public async Task AddLeaveRequestDays_ReturnsFalse_WhenBalanceNotFound()
    {
        // Act
        var result = await _service.AddLeaveRequestDays(1, 1, 5);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddLeaveRequestDays_RespectsDefaultDaysLimit()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();
        var leaveBalance = new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, RemainingDays = 15 };
        await _context.LeaveBalances.AddAsync(leaveBalance);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.AddLeaveRequestDays(1, 1, 10); // Try to add 10 days when only 5 can be added

        // Assert
        Assert.True(result);
        var updated = await _context.LeaveBalances.FirstAsync(lb => lb.EmployeeId == 1 && lb.LeaveTypeId == 1);
        Assert.Equal(20, updated.RemainingDays); // Should be capped at default days (20)
    }

    [Fact]
    public async Task AddLeaveRequestDays_AddsCorrectDays_WhenBelowLimit()
    {
        // Arrange
        var employee = await CreateTestEmployee();
        var leaveTypes = await CreateTestLeaveTypes();
        var leaveBalance = new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, RemainingDays = 10 };
        await _context.LeaveBalances.AddAsync(leaveBalance);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.AddLeaveRequestDays(1, 1, 5);

        // Assert
        Assert.True(result);
        var updated = await _context.LeaveBalances.FirstAsync(lb => lb.EmployeeId == 1 && lb.LeaveTypeId == 1);
        Assert.Equal(15, updated.RemainingDays);
    }
}