// Leave Balance Service Interface
// ========================================

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface ILeaveBalanceService
{
    // Get all leave balances (with their types) by employee id
    Task<List<LeaveBalanceDTO>> GetAllLeaveBalancesByEmployeeId(int employeeId);


     // Create all default leave balances for a new employee
    Task<bool> CreateDefaultLeaveBalances(int employeeId);

    // Get the total remaining days and total leave days for an employee by Id
    Task<LeaveBalanceSumDTO> GetTotalLeaveBalanceSum(int employeeId);
}
