using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CoriCore.Services;

public class EmpUserService : IEmpUserService
{
    private readonly AppDbContext _context;

    public EmpUserService(AppDbContext context)
    {
        _context = context;
    }
    
    // Get all EmpUsers
    public async Task<List<EmpUserDTO>> GetAllEmpUsers()
    {
            var empUsers = await _context.Employees
                .Include(e => e.User)
                .Select(e => new EmpUserDTO
                {
                    // User Information
                    UserId = e.UserId,
                    FullName = e.User.FullName,
                    Email = e.User.Email,
                    ProfilePicture = e.User.ProfilePicture,
                    Role = e.User.Role,

                    // Employee Information
                    EmployeeId = e.EmployeeId,
                    Gender = e.Gender,
                    DateOfBirth = e.DateOfBirth,
                    PhoneNumber = e.PhoneNumber,
                    JobTitle = e.JobTitle,
                    Department = e.Department,
                    SalaryAmount = e.SalaryAmount,
                    PayCycle = e.PayCycle,
                    LastPaidDate = e.LastPaidDate,
                    EmployType = e.EmployType,
                    EmployDate = e.EmployDate,
                    IsSuspended = e.IsSuspended,
                })
                .ToListAsync();
        return empUsers;
    }
}
