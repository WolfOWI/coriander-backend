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


    // Get EmpUser by ID
    public async Task<EmpUserDTO> GetEmpUserById(int id)
    {
        var empUser = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        // If the employee-user is not found
        if (empUser == null)
        {
            throw new Exception("Employee-User not found");
        }

        return new EmpUserDTO
        {
            // User Information
            UserId = empUser.UserId,
            FullName = empUser.User.FullName,
            Email = empUser.User.Email,
            ProfilePicture = empUser.User.ProfilePicture,
            Role = empUser.User.Role,

            // Employee Information
            EmployeeId = empUser.EmployeeId,
            Gender = empUser.Gender,
            DateOfBirth = empUser.DateOfBirth,
            PhoneNumber = empUser.PhoneNumber,
            JobTitle = empUser.JobTitle,
            Department = empUser.Department,
            SalaryAmount = empUser.SalaryAmount,
            PayCycle = empUser.PayCycle,
            LastPaidDate = empUser.LastPaidDate,
            EmployType = empUser.EmployType,
            EmployDate = empUser.EmployDate,
            IsSuspended = empUser.IsSuspended,
        };
    }

}
