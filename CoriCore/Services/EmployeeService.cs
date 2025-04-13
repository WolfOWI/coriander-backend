using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services 
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly ILeaveBalanceService _leaveBalanceService;
        public EmployeeService(AppDbContext context, IUserService userService, ILeaveBalanceService leaveBalanceService)
        {
            _context = context;
            _userService = userService;
            _leaveBalanceService = leaveBalanceService;
        }

        public async Task<(int Code, string Message)> ValidateEmployeeInfoAsync(EmployeeDto employeeDto)
        {
            if (employeeDto == null)
                return (400, "Invalid Employee data");
            if (employeeDto.UserId <= 0)
                return (400, "Missing or invalid UserId");
            if (string.IsNullOrWhiteSpace(employeeDto.PhoneNumber))
                return (400, "Phone number is required");
            if (string.IsNullOrWhiteSpace(employeeDto.JobTitle))
                return (400, "Job title is required");
            if (string.IsNullOrWhiteSpace(employeeDto.Department))
                return (400, "Department is required");

            return (201, "Validation successful");
        }

        public async Task<(int Code, string Message)> CreateEmployeeAsync(EmployeeDto dto)
        {
            var validationResult = await ValidateEmployeeInfoAsync(dto);
            if (validationResult.Code != 201)
                return validationResult;

            var employee = new Employee
            {
                UserId = dto.UserId,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                JobTitle = dto.JobTitle,
                Department = dto.Department,
                SalaryAmount = dto.SalaryAmount,
                PayCycle = dto.PayCycle,
                LastPaidDate = dto.LastPaidDate,
                EmployType = dto.EmployType,
                EmployDate = dto.EmployDate,
                IsSuspended = false // default to false
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Create the employee's default leave balances after the employee is saved
            var balsCreated = await _leaveBalanceService.CreateDefaultLeaveBalances(employee.EmployeeId);
            if (!balsCreated) {
                return (400, "Failed to create default leave balances");
            }

            return (201, "Employee successfully registered");
        }

        public async Task<(int Code, string Message)> RegisterEmployeeAsync(EmployeeDto dto)
        {
            var userCheck = await _userService.EmployeeAdminExistsAsync(dto.UserId);
            if (userCheck == 400)
                return (400, $"User with UserId {dto.UserId} is assigned as an Admin or Employee");

            var validation = await ValidateEmployeeInfoAsync(dto);
            if (validation.Code != 201)
                return validation;

            // var paydayResult = await CalculateNextPayDayAsync((int)dto.PayCycle);
            // dto.NextPayday = paydayResult.NextPayDay;

            var roleSet = await _userService.SetUserRoleAsync(dto.UserId, (int)UserRole.Employee);
            if (roleSet != 201)
                return (400, "Failed to set user role");

            var createResult = await CreateEmployeeAsync(dto);
            return createResult;
        }

        public async Task<(int Code, string Message)> ToggleEmpSuspensionAsync(int employeeId)
        {
            // Find the employee
            var emp = await _context.Employees.FindAsync(employeeId);

            // If employee not found
            if (emp == null)
                {
                    return (404, "Employee not found");
                }

            // (Found) Toggle the suspension status
            emp.IsSuspended = !emp.IsSuspended;
            await _context.SaveChangesAsync();

            // Return the result
            return (200, "Employee suspension status updated to " + (emp.IsSuspended ? "suspended" : "unsuspended"));
        }

        /// <summary>
        /// Delete employee by id
        /// </summary>
        /// <param name="employeeId">The ID of the employee to delete.</param>
        /// <returns>
        /// Tuple result: (int Code, string Message)
        public async Task<(int Code, string Message)> DeleteEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return (404, "Employee not found");
            }

            // Delete the employee
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            // Return the result
            return (200, "Employee deleted successfully");
        }
    }
}