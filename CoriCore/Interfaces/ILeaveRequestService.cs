//Iné Smith

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

// Get a list of all the leave requests of an employee by their id.
public interface ILeaveRequestService
{
    // Get all leave requests by employee ID
    Task<List<LeaveRequestDTO>> GetAllLeaveRequestsByEmpId(int employeeId);
    Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployeeId(int employeeId);

    // Returns a list of leave requests of a single employee (LeaveRequestDTO)

    // Calculate the durations of the leave, and add it to the data.
    int DurationBetweenDates(DateTime startDate, DateTime endDate); // durationBetweenDates (date 1, date 2)
    Task<List<LeaveRequestDTO>> AddDurationToLeaveRequests(List<LeaveRequestDTO> leaveRequests);

    // Returns the number of dates between two dates
}

// Returns a list of leave requests of a single employee (LeaveRequestDTO) with duration data.




