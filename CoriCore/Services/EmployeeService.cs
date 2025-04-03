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

        // public async Task<(int Code, string Message, DateOnly NextPayDay)> CalculateNextPayDayAsync(int payCycle)
        // {
        //     var today = DateOnly.FromDateTime(DateTime.Today);
        //     DateOnly nextPayDay = payCycle switch
        //     {
        //         (int)PayCycle.Monthly => new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)),
        //         (int)PayCycle.Weekly => today.AddDays(7 - (int)today.DayOfWeek),
        //         (int)PayCycle.BiWeekly => today.AddDays(14 - (int)today.DayOfWeek),
        //         _ => today
        //     };

        //     return (201, "Next payday calculated successfully", nextPayDay);
        // }

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
            if (userCheck != 201)
                return (400, $"User with UserId {dto.UserId} is already registered");

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

        public async Task<(int Code, string Message)> UpdateEmployeeDetailsByIdAsync(int id, EmployeeUpdateDTO updateDto)
        {
            // Find the employee
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return (404, "Employee not found");
            }

            // Update only the fields that are provided (not null)
            if (updateDto.Gender.HasValue) employee.Gender = updateDto.Gender.Value;
            if (updateDto.DateOfBirth.HasValue) employee.DateOfBirth = updateDto.DateOfBirth.Value;
            if (updateDto.PhoneNumber != null) employee.PhoneNumber = updateDto.PhoneNumber;
            if (updateDto.JobTitle != null) employee.JobTitle = updateDto.JobTitle;
            if (updateDto.Department != null) employee.Department = updateDto.Department;
            if (updateDto.SalaryAmount.HasValue) employee.SalaryAmount = updateDto.SalaryAmount.Value;
            if (updateDto.PayCycle.HasValue) employee.PayCycle = updateDto.PayCycle.Value;
            if (updateDto.LastPaidDate.HasValue) employee.LastPaidDate = updateDto.LastPaidDate.Value;
            if (updateDto.EmployType.HasValue) employee.EmployType = updateDto.EmployType.Value;
            if (updateDto.EmployDate.HasValue) employee.EmployDate = updateDto.EmployDate.Value;
            if (updateDto.IsSuspended.HasValue) employee.IsSuspended = updateDto.IsSuspended.Value;

            try
            {
                await _context.SaveChangesAsync();
                return (200, "Employee details updated successfully");
            }
            catch (DbUpdateException ex)
            {
                return (400, $"Error updating employee: {ex.Message}");
            }
        }
    }
}