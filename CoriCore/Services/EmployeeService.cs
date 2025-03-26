using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

namespace CoriCore.Services 
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public EmployeeService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
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
                IsSuspended = dto.IsSuspended
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
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
    }
}