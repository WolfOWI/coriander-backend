// Leave Balance Service
// ========================================

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class LeaveBalanceService : ILeaveBalanceService
{
    // Dependency Injection
    private readonly AppDbContext _context;

    public LeaveBalanceService(AppDbContext context)
    {
        _context = context;
    }

    // Get all leave balances (with their types) by employee id
    /// <inheritdoc/>
    public async Task<List<LeaveBalanceDTO>> GetAllLeaveBalancesByEmployeeId(int employeeId)
    {
        var leaveBalances = await _context.LeaveBalances.Include(lb => lb.LeaveType).Where(lb => lb.EmployeeId == employeeId).ToListAsync();
        return leaveBalances.Select(lb => new LeaveBalanceDTO
        {
            LeaveBalanceId = lb.LeaveBalanceId,
            RemainingDays = lb.RemainingDays,
            LeaveTypeName = lb.LeaveType.LeaveTypeName,
            Description = lb.LeaveType.Description,
            DefaultDays = lb.LeaveType.DefaultDays,
        }).ToList();
    }

    // Create all default leave balances for a new employee
    /// <inheritdoc/>
    public async Task<bool> CreateDefaultLeaveBalances(int employeeId)
    {

        // Check if the employee exists
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        // Get all leave types
        var leaveTypes = await _context.LeaveTypes.ToListAsync();

        // Create a list of leave balances
        var leaveBalances = new List<LeaveBalance>();

        // For each leave type, create a leave balance with default days
        foreach (var leaveType in leaveTypes)
        {
            leaveBalances.Add(new LeaveBalance
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveType.LeaveTypeId,
                RemainingDays = leaveType.DefaultDays,
            });
        }

        // Add the leave balances to the database
        await _context.LeaveBalances.AddRangeAsync(leaveBalances);
        await _context.SaveChangesAsync();

        // Return true if successful
        return true;
    }

    // Get the total remaining days and total leave days for an employee by Id
    /// <inheritdoc/>
    public async Task<LeaveBalanceSumDTO> GetTotalLeaveBalanceSum(int employeeId)
    {
        var leaveBalances = await GetAllLeaveBalancesByEmployeeId(employeeId);
        return new LeaveBalanceSumDTO
        {
            TotalRemainingDays = leaveBalances.Sum(lb => lb.RemainingDays),
            TotalLeaveDays = leaveBalances.Sum(lb => lb.DefaultDays)
        };
    }

    // Subtract leave request days from an employee's leave balance of a specific leave type
    /// <inheritdoc/>
    public async Task<bool> SubtractLeaveRequestDays(int employeeId, int leaveTypeId, int days)
    {
        var leaveBalance = await _context.LeaveBalances
            .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveTypeId == leaveTypeId);
        if (leaveBalance == null)
        {
            return false; // Leave balance not found
        }
        if (leaveBalance.RemainingDays < days)
        {   
            // If the leave balance is less than the days requested, make the remaining days 0
            leaveBalance.RemainingDays = 0;
        }
        else
        {
            // Subtract the leave request days from the leave balance
            leaveBalance.RemainingDays -= days;
        }
        await _context.SaveChangesAsync();
        return true;
    }
}
