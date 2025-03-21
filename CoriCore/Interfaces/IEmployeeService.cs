using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces
{
    /// <summary>
    /// Interface for employee-related business logic operations
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Validates the employee and pay cycle data
        /// </summary>
        /// <param name="employeeDto">Employee input object</param>
        /// <param name="payCycleDto">PayCycle input object</param>
        /// <returns>201 for valid data, 400 for validation failure</returns>
        Task<int> ValidateEmployeeInfo(EmployeeDto employeeDto);

        /// <summary>
        /// Creates an employee record and links it to an existing PayCycle
        /// </summary>
        /// <param name="employeeDto">Employee data including PayCycleId</param>
        /// <returns>201: success, 400: error message</returns>
        Task<(bool Success, string Message)> CreateEmployee(EmployeeDto employeeDto);

        /// <summary>
        /// Registers a new employee, performing full validation and pay cycle creation
        /// </summary>
        /// <param name="employeeDto">Employee input object</param>
        /// <param name="payCycleDto">PayCycle input object</param>
        /// <returns>201: Employee Successfully registered, 400: Corresponding error</returns>
        Task<(bool Success, string Message)> RegisterEmployee(EmployeeDto employeeDto);
    }
}
