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
                .Include(lr => lr.Employee) 
                    .ThenInclude(e => e.User) // Include Employee data
                .Where(lr => lr.EmployeeId == employeeId) // Filter by EmployeeId
                .Select(lr => new LeaveRequestDTO
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.Employee.User.FullName, // Map Employee Name
                    LeaveTypeId = lr.LeaveTypeId,
                    LeaveType = lr.LeaveType.LeaveTypeName, // Map Leave Type Name
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

        public async Task<List<LeaveRequestDTO>> GetDetailedLeaveRequestsByEmployeeId(int employeeId)
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.LeaveType) // Include LeaveType data
                .Include(lr => lr.Employee) // Include Employee data
                .Where(lr => lr.EmployeeId == employeeId) // Filter by EmployeeId
                .Select(lr => new LeaveRequestDTO
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.Employee != null ? lr.Employee.User.FullName : "Unknown", // Handle null Employee
                    LeaveTypeId = lr.LeaveTypeId,
                    LeaveType = lr.LeaveType != null ? lr.LeaveType.LeaveTypeName : "Unknown", // Handle null LeaveType
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Comment = lr.Comment,
                    Status = lr.Status,
                    CreatedAt = lr.CreatedAt,
                    LeaveTypeName = lr.LeaveType != null ? lr.LeaveType.LeaveTypeName : "Unknown",
                })
                .ToListAsync();

            return leaveRequests;
        }

        public Task<List<LeaveRequestDTO>> GetAllLeaveRequests()
        {
            var leaveRequests = _context.LeaveRequests
                .Include(lr => lr.LeaveType) // Include LeaveType data
                .Include(lr => lr.Employee) // Include Employee data
                .Select(lr => new LeaveRequestDTO
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = lr.Employee.User.FullName, // Map Employee Name
                    LeaveTypeId = lr.LeaveTypeId,
                    LeaveType = lr.LeaveType.LeaveTypeName, // Map Leave Type Name
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
