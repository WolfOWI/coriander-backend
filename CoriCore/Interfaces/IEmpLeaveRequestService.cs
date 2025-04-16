// Iné Smith

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IEmpLeaveRequestService
{
    // Retrieve all employee leave requests
    Task<List<EmpLeaveRequestDTO>> GetAllEmployeeLeaveRequests();

    // Approve a leave request by Id
    Task<bool> ApproveLeaveRequestById(int leaveRequestId);

    // Reject a leave request by Id
    Task<bool> RejectLeaveRequestById(int leaveRequestId);

    // Set a leave request to pending by Id
    Task<bool> SetLeaveRequestToPendingById(int leaveRequestId);
}
