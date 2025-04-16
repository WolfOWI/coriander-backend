// Apply For Leave Service
// Iné Smith

using System;
using CoriCore.Interfaces;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class ApplyForLeaveService : IApplyForLeaveService // Inherits from IApplyForLeaveService
// This service handles the business logic for applying for leave.
{
    private readonly AppDbContext _context;

    // Constructor for ApplyForLeaveService
    // It takes an AppDbContext as a parameter and assigns it to the private field.
    public ApplyForLeaveService(AppDbContext context)
    {
        _context = context;
    }

    // Create and store a new leave request in the database
    public async Task<LeaveRequestDTO> ApplyForLeave(ApplyForLeaveDTO leaveRequest)
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
            CreatedAt = DateTime.UtcNow // Set to current date and time (UTC Format)
        };

        // Add the new leave request to the database context
        _context.LeaveRequests.Add(newLeaveRequest);

        // Save changes to the database asynchronously
        await _context.SaveChangesAsync();

        // Load the created leave request with its type
        var createdLeaveRequest = await _context.LeaveRequests
            .Include(lr => lr.LeaveType)
            .FirstOrDefaultAsync(lr => lr.LeaveRequestId == newLeaveRequest.LeaveRequestId);

        // Make sure it was created and loaded correctly
        if (createdLeaveRequest == null || createdLeaveRequest.LeaveType == null)
        {
            throw new Exception("Failed to create leave request or load it's data");
        }

        // Map the created LeaveRequest to the LeaveRequestDTO and return all the details
        return new LeaveRequestDTO
        {
            LeaveRequestId = createdLeaveRequest.LeaveRequestId,
            EmployeeId = createdLeaveRequest.EmployeeId,
            LeaveTypeId = createdLeaveRequest.LeaveTypeId,
            StartDate = createdLeaveRequest.StartDate,
            EndDate = createdLeaveRequest.EndDate,
            Comment = createdLeaveRequest.Comment,
            Status = createdLeaveRequest.Status,
            CreatedAt = createdLeaveRequest.CreatedAt,
            LeaveTypeName = createdLeaveRequest.LeaveType.LeaveTypeName,
            Description = createdLeaveRequest.LeaveType.Description,
            DefaultDays = createdLeaveRequest.LeaveType.DefaultDays
        };
    }
}
