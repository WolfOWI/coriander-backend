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
    }
}