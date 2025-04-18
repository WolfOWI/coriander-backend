// Leave Balance Service Interface
// ========================================

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface ILeaveBalanceService
{
    /// <summary>
    /// Get all leave balances (with their types) by employee id
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>A list of leave balances</returns>
    Task<List<LeaveBalanceDTO>> GetAllLeaveBalancesByEmployeeId(int employeeId);

    /// <summary>
    /// Create all default leave balances for a new employee
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>A boolean indicating the success of the operation</returns>
    Task<bool> CreateDefaultLeaveBalances(int employeeId);

    /// <summary>
    /// Get the total remaining days and total leave days for an employee by Id
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>A DTO containing the total remaining days and total leave days</returns>
    Task<LeaveBalanceSumDTO> GetTotalLeaveBalanceSum(int employeeId);
}
