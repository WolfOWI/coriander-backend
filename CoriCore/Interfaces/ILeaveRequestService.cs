// Leave Request Service Interface
// ========================================
// Iné Smith

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoriCore.DTOs;

namespace CoriCore.Interfaces
{
    // Interface for managing leave requests
    public interface ILeaveRequestService
    {
        // Get all leave requests by employee ID
        Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployeeId(int employeeId);

        // Calculate the duration between two dates (in days)
        // int DurationBetweenDates(DateTime startDate, DateTime endDate);

        // Add duration to each leave request's data
        // Task<List<LeaveRequestDTO>> AddDurationToLeaveRequests(List<LeaveRequestDTO> leaveRequests);
    }
}