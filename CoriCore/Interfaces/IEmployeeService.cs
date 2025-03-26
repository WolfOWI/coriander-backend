using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces
{
    /// <summary>
    /// Interface for handling Employee-related services and operations.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Validates the provided employee information.
        /// </summary>
        /// <param name="employeeDto">EmployeeDto object containing employee details.</param>
        /// <returns>
        /// Tuple result: (int Code, string Message)
        /// 201 - Success
        /// 400 - Missing or incorrect field
        /// </returns>
        Task<(int Code, string Message)> ValidateEmployeeInfoAsync(EmployeeDto employeeDto);

        /// <summary>
        /// Calculates the next payday based on the pay cycle.
        /// </summary>
        /// <param name="payCycle">
        /// PayCycle int values:
        /// 0 = Monthly,
        /// 1 = Bi-Weekly,
        /// 2 = Weekly
        /// </param>
        /// <returns>
        /// Tuple result: (int Code, string Message, DateOnly NextPayDay)
        /// </returns>
        // Task<(int Code, string Message, DateOnly NextPayDay)> CalculateNextPayDayAsync(int payCycle);

        /// <summary>
        /// Creates a new employee record.
        /// </summary>
        /// <param name="employeeDto">EmployeeDto object containing employee details.</param>
        /// <returns>
        /// Tuple result: (int Code, string Message)
        /// 201 - Registered Employee
        /// 400 - Invalid data with corresponding error message
        /// </returns>
        Task<(int Code, string Message)> CreateEmployeeAsync(EmployeeDto employeeDto);

        /// <summary>
        /// Registers a new employee by processing and validating data, setting roles, and creating the record.
        /// </summary>
        /// <param name="employeeDto">EmployeeDto object containing employee details.</param>
        /// <returns>
        /// Tuple result: (int Code, string Message)
        /// 201 - Employee Successfully registered
        /// 400 - Corresponding error code from validation or creation step
        /// </returns>
        Task<(int Code, string Message)> RegisterEmployeeAsync(EmployeeDto employeeDto);
    }
}
