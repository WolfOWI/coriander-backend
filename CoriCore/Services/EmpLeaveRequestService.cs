// Iné Smith

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class EmpLeaveRequestService : IEmpLeaveRequestService
{
    private readonly AppDbContext _context;

    public EmpLeaveRequestService(AppDbContext context)
    {
        _context = context;
    }

    // Get all employee leave requests
    public async Task<List<EmpLeaveRequestDTO>> GetAllEmployeeLeaveRequests()
    {
        var leaveRequests = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.Employee.LeaveBalances)
            .Select(lr => new EmpLeaveRequestDTO
            {
                LeaveRequestId = lr.LeaveRequestId,
                LeaveTypeId = lr.LeaveType.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Comment = lr.Comment,
                Status = lr.Status,
                CreatedAt = lr.CreatedAt,
                LeaveTypeName = lr.LeaveType.LeaveTypeName,
                UserId = lr.Employee.User.UserId,
                FullName = lr.Employee.User.FullName,
                EmployeeId = lr.Employee.EmployeeId,
                EmployType = lr.Employee.EmployType,
                IsSuspended = lr.Employee.IsSuspended,
                // Find the remaining days of the specific leave type of the leave request
                RemainingDays = lr.Employee.LeaveBalances.Where(lb => lb.LeaveTypeId == lr.LeaveType.LeaveTypeId)
                    .Select(lb => lb.RemainingDays)
                    .FirstOrDefault(),
            })
            .ToListAsync();

        return leaveRequests;
    }

    // Get pending leave requests
    public async Task<List<EmpLeaveRequestDTO>> GetPendingLeaveRequests()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.Employee.LeaveBalances)
            .Where(lr => lr.Status == LeaveStatus.Pending)
            .Select(lr => new EmpLeaveRequestDTO
            {
                LeaveRequestId = lr.LeaveRequestId,
                LeaveTypeId = lr.LeaveType.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Comment = lr.Comment,
                Status = lr.Status,
                CreatedAt = lr.CreatedAt,
                LeaveTypeName = lr.LeaveType.LeaveTypeName,
                UserId = lr.Employee.User.UserId,
                FullName = lr.Employee.User.FullName,
                EmployeeId = lr.Employee.EmployeeId,
                EmployType = lr.Employee.EmployType,
                IsSuspended = lr.Employee.IsSuspended,
                RemainingDays = lr.Employee.LeaveBalances
                    .Where(lb => lb.LeaveTypeId == lr.LeaveType.LeaveTypeId)
                    .Select(lb => lb.RemainingDays)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    // Get approved leave requests
    public async Task<List<EmpLeaveRequestDTO>> GetApprovedLeaveRequests()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.Employee.LeaveBalances)
            .Where(lr => lr.Status == LeaveStatus.Approved)
            .Select(lr => new EmpLeaveRequestDTO
            {
                LeaveRequestId = lr.LeaveRequestId,
                LeaveTypeId = lr.LeaveType.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Comment = lr.Comment,
                Status = lr.Status,
                CreatedAt = lr.CreatedAt,
                LeaveTypeName = lr.LeaveType.LeaveTypeName,
                UserId = lr.Employee.User.UserId,
                FullName = lr.Employee.User.FullName,
                EmployeeId = lr.Employee.EmployeeId,
                EmployType = lr.Employee.EmployType,
                IsSuspended = lr.Employee.IsSuspended,
                RemainingDays = lr.Employee.LeaveBalances
                    .Where(lb => lb.LeaveTypeId == lr.LeaveType.LeaveTypeId)
                    .Select(lb => lb.RemainingDays)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    // Get rejected leave requests
    public async Task<List<EmpLeaveRequestDTO>> GetRejectedLeaveRequests()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User)
            .Include(lr => lr.LeaveType)
            .Include(lr => lr.Employee.LeaveBalances)
            .Where(lr => lr.Status == LeaveStatus.Rejected)
            .Select(lr => new EmpLeaveRequestDTO
            {
                LeaveRequestId = lr.LeaveRequestId,
                LeaveTypeId = lr.LeaveType.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Comment = lr.Comment,
                Status = lr.Status,
                CreatedAt = lr.CreatedAt,
                LeaveTypeName = lr.LeaveType.LeaveTypeName,
                UserId = lr.Employee.User.UserId,
                FullName = lr.Employee.User.FullName,
                EmployeeId = lr.Employee.EmployeeId,
                EmployType = lr.Employee.EmployType,
                IsSuspended = lr.Employee.IsSuspended,
                RemainingDays = lr.Employee.LeaveBalances
                    .Where(lb => lb.LeaveTypeId == lr.LeaveType.LeaveTypeId)
                    .Select(lb => lb.RemainingDays)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    // Approve a leave request
    public async Task<bool> ApproveLeaveRequestById(int leaveRequestId)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
        if (leaveRequest == null) return false;

        leaveRequest.Status = LeaveStatus.Approved;
        await _context.SaveChangesAsync();
        return true;
    }

    // Reject a leave request
    public async Task<bool> RejectLeaveRequestById(int leaveRequestId)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
        if (leaveRequest == null) return false;

        leaveRequest.Status = LeaveStatus.Rejected;
        await _context.SaveChangesAsync();
        return true;
    }

    // Set leave request to pending
    public async Task<bool> SetLeaveRequestToPendingById(int leaveRequestId)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
        if (leaveRequest == null) return false;

        leaveRequest.Status = LeaveStatus.Pending;
        await _context.SaveChangesAsync();
        return true;
    }
}
