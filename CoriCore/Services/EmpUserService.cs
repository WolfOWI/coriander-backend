// EmpUser Service
// ========================================

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CoriCore.Services;

public class EmpUserService : IEmpUserService
{
    private readonly AppDbContext _context;
    private readonly IEquipmentService _equipmentService;

    public EmpUserService(AppDbContext context, IEquipmentService equipmentService)
    {
        _context = context;
        _equipmentService = equipmentService;
    }
    
    // Get all EmpUsers
    /// <inheritdoc/>
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
                    GoogleId = e.User.GoogleId,
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
    /// <inheritdoc/>
    public async Task<EmpUserDTO> GetEmpUserByEmpId(int id)
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
            GoogleId = empUser.User.GoogleId,
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
    
    // Update EmpUser details by ID
    /// <inheritdoc/>
    public async Task<(int Code, string Message)> UpdateEmpUserDetailsByIdAsync(int id, EmployeeUpdateDTO updateDto)
        {
            // Find the employee and include the user
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return (404, "Employee not found");
            }

            // Update only the fields that are provided (not null)
            if (updateDto.FullName != null) employee.User.FullName = updateDto.FullName;
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
            if (updateDto.ProfilePicture != null) employee.User.ProfilePicture = updateDto.ProfilePicture;

            try
            {
                await _context.SaveChangesAsync();
                return (200, "Employee user details updated successfully");
            }
            catch (DbUpdateException ex)
            {
                return (400, $"Error updating employee user: {ex.Message}");
            }
        }

    /// <inheritdoc/>
    public async Task<List<EmpUserEquipStatsDTO>> GetAllEmpsEquipStats(int comparedEquipId)
    {

        // Get the category ID of the equipment we're comparing against
        var comparedEquipment = await _context.Equipments
            .FirstOrDefaultAsync(e => e.EquipmentId == comparedEquipId);

        // If the equipment is not found
        if (comparedEquipment == null)
        {
            throw new Exception($"Equipment with ID {comparedEquipId} not found");
        }

        // CategoryId of the equipment we're comparing against
        var comparedCategoryId = comparedEquipment.EquipmentCatId;

        // List to store results & return
        var empUserEquipStats = new List<EmpUserEquipStatsDTO>();

    
        // Get all employee users
        var employees = await _context.Employees
            .Include(e => e.User)
            .ToListAsync();

        // Add stats for each employee
        foreach (var employee in employees)
        {
            // Get equipment items for this employee (from equipment service)
            var equipmentItems = await _equipmentService.GetEquipmentByEmployeeId(employee.EmployeeId);

            bool hasSameEquipCat = false;

            // For each item of employee, check if same category as the compared equip item's category
            foreach (var item in equipmentItems)
            {
                if (item.EquipmentCatId == comparedCategoryId)
                {
                    hasSameEquipCat = true;
                    break; // Break if found
                }
            }
            
            // Create DTO for this employee
            var empStats = new EmpUserEquipStatsDTO
            {
                EmployeeId = employee.EmployeeId,
                IsSuspended = employee.IsSuspended,
                FullName = employee.User.FullName,
                ProfilePicture = employee.User.ProfilePicture,
                NumberOfItems = equipmentItems.Count,
                HasItemOfSameEquipCat = hasSameEquipCat
            };
            
            // Add to list
            empUserEquipStats.Add(empStats);
        }

        return empUserEquipStats;
    }
}
