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
                EmployeeId = lr.Employee.EmployeeId,
                LeaveTypeId = lr.LeaveType.LeaveTypeId,
                LeaveTypeName = lr.LeaveType.LeaveTypeName,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Comment = lr.Comment,
                Status = lr.Status,
                CreatedAt = lr.CreatedAt,
                RemainingDays = lr.Employee.LeaveBalances.Where(lb => lb.LeaveTypeId == lr.LeaveType.LeaveTypeId)
                    .Select(lb => lb.RemainingDays)
                    .FirstOrDefault(),
                FullName = lr.Employee.User.FullName,
                IsVerified = lr.Employee.User.IsVerified,
                VerificationCode = lr.Employee.User.VerificationCode
            })
            .ToListAsync();

        return leaveRequests;
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
