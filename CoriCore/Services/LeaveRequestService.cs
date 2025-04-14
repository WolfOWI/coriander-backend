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
        private readonly AppDbContext _context;

        public LeaveRequestService(AppDbContext context)
        {
            _context = context;
        }

        // Get all Leave Requests with their LeaveType details, filtered by EmployeeId
        public async Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.LeaveType) // Include LeaveType data
                .Where(lr => lr.EmployeeId == employeeId) // Filter by EmployeeId
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

        // // Calculate the duration between two dates
        // public int DurationBetweenDates(DateTime startDate, DateTime endDate)
        // {
        //     var timeDiff = endDate - startDate;
        //     return (int)timeDiff.TotalDays; // Convert time difference to days
        // }

        // // Add duration to each leave request's data
        // public async Task<List<LeaveRequestDTO>> AddDurationToLeaveRequests(List<LeaveRequestDTO> leaveRequests)
        // {
        //     foreach (var request in leaveRequests)
        //     {
        //         request.Duration = DurationBetweenDates(request.StartDate, request.EndDate);
        //     }
        //     return await Task.FromResult(leaveRequests);
        // }
    }
}
