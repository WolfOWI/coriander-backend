using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

namespace CoriCore.Services;

public class EmployeeService : IEmployeeService
{
    public Task<(bool Success, string Message)> CreateEmployee(EmployeeDto employeeDto)
    {
        throw new NotImplementedException();
    }

    public Task<(bool Success, string Message)> RegisterEmployee(EmployeeDto employeeDto)
    {
        throw new NotImplementedException();
    }

    public Task<int> ValidateEmployeeInfo(EmployeeDto employeeDto)
    {
        throw new NotImplementedException();
    }
}
