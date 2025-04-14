// Apply For Leave Service
// ========================================

using System;
using CoriCore.Interfaces;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Data;

namespace CoriCore.Services;

public class ApplyForLeaveService : IApplyForLeaveService
{
    private readonly AppDbContext _context;

    public ApplyForLeaveService(AppDbContext context)
    {
        _context = context;
    }

    // Create and store a new leave request in the database
    public async Task<ApplyForLeaveDTO> ApplyForLeave(ApplyForLeaveDTO leaveRequest)
    {
        // Map the ApplyForLeaveDTO to the database entity LeaveRequest
        var newLeaveRequest = new LeaveRequest
        {
            EmployeeId = leaveRequest.EmployeeId,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Comment = leaveRequest.Comment,
            Status = LeaveStatus.Pending, // Initial status, can be updated later
            CreatedAt = leaveRequest.CreatedAt
        };

        // Add the new leave request to the database context
        _context.LeaveRequests.Add(newLeaveRequest);

        // Save changes to the database asynchronously
        await _context.SaveChangesAsync();

        // Return the DTO with the ID of the newly created leave request
        leaveRequest.LeaveRequestId = newLeaveRequest.LeaveRequestId;

        return leaveRequest;
    }
}
