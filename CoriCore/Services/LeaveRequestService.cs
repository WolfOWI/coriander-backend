// Leave Request Service
// ========================================
// Iné Smith

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services
{
    public class LeaveRequestService : ILeaveRequestService // Implementing the ILeaveRequestService interface
    {
        private readonly AppDbContext _context; // Dependency injection for the database context

        // Constructor for LeaveRequestService
        public LeaveRequestService(AppDbContext context)
        {
            _context = context;
        }

        // Get all Leave Requests with their LeaveType details, filtered by EmployeeId
        public async Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.EmployeeId == employeeId)
                .Select(lr => new LeaveRequestDTO
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    LeaveTypeId = lr.LeaveTypeId,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Comment = lr.Comment,
                    Status = lr.Status,
                    CreatedAt = lr.CreatedAt,
                    LeaveTypeName = lr.LeaveType.LeaveTypeName,
                    Description = lr.LeaveType.Description,
                    DefaultDays = lr.LeaveType.DefaultDays
                })
                .ToListAsync();

            return leaveRequests;
        }
    }
}
