// Equipment Service
// ========================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

namespace CoriCore.Services;

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _context;

    public EquipmentService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<EquipmentDTO>> GetEquipmentByEmployeeId(int employeeId)
    {
        var equipment = await _context.Equipments
            .Include(e => e.EquipmentCategory)
            .Where(e => e.EmployeeId.HasValue && e.EmployeeId.Value == employeeId)
            .Select(e => new EquipmentDTO
            {
                EquipmentId = e.EquipmentId,
                EmployeeId = e.EmployeeId ?? 0,
                EquipmentCatId = e.EquipmentCatId,
                EquipmentCategoryName = e.EquipmentCategory.EquipmentCatName,
                EquipmentName = e.EquipmentName,
                AssignedDate = e.AssignedDate,
                Condition = e.Condition
            })
            .ToListAsync();

        return equipment;
    }

    //CreateEquipmentItems but give is as a list
    //CreateEquipmentItems([{equipment 1}, {equipment 2}, {equipments 3}])
    public async Task<IEnumerable<Equipment>> CreateEquipmentItemsAsync(List<CreateEquipmentDTO> equipmentDTOs)
    {
        // Map EquipmentDTOs to Equipment entities
        var equipmentItems = equipmentDTOs.Select(dto => new Equipment
        {
            EmployeeId = dto.EmployeeId,
            EquipmentCatId = dto.EquipmentCatId,
            EquipmentName = dto.EquipmentName,
            AssignedDate = dto.AssignedDate,
            Condition = dto.Condition
        }).ToList();

        // Add the equipment items to the context
        _context.Equipments.AddRange(equipmentItems);

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Return the created equipment items
        return equipmentItems;
    }

    //Edit and update the equipment items by id
    public async Task<EquipmentDTO> EditEquipmentItemAsync(int equipmentId, UpdateEquipmentDTO equipmentDTO)
    {
        // Get the equipment item from the database (with the equipment category included)
        var equipment = await _context.Equipments
            .Include(e => e.EquipmentCategory)
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);
        
        if (equipment == null)
        {
            throw new KeyNotFoundException($"Equipment with ID {equipmentId} not found.");
        }

        // Update the properties of the equipment item if they are provided
        if (equipmentDTO.EmployeeId.HasValue) equipment.EmployeeId = equipmentDTO.EmployeeId;
        if (equipmentDTO.EquipmentCatId.HasValue) equipment.EquipmentCatId = equipmentDTO.EquipmentCatId.Value;
        if (!string.IsNullOrEmpty(equipmentDTO.EquipmentName)) equipment.EquipmentName = equipmentDTO.EquipmentName;
        if (equipmentDTO.AssignedDate.HasValue) equipment.AssignedDate = equipmentDTO.AssignedDate;
        if (equipmentDTO.Condition.HasValue) equipment.Condition = equipmentDTO.Condition.Value;

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Return the updated equipment item
        return new EquipmentDTO
        {
            EquipmentId = equipment.EquipmentId,
            EmployeeId = equipment.EmployeeId ?? 0,
            EquipmentCatId = equipment.EquipmentCatId,
            EquipmentCategoryName = equipment.EquipmentCategory?.EquipmentCatName ?? string.Empty,
            EquipmentName = equipment.EquipmentName,
            AssignedDate = equipment.AssignedDate,
            Condition = equipment.Condition
        };
    }

    //Delete equipment item by id
    public async Task<bool> DeleteEquipmentItemAsync(int equipmentId)
    {
        var equipment = await _context.Equipments.FindAsync(equipmentId);
        if (equipment == null)
        {
            return false; // Equipment not found
        }

        _context.Equipments.Remove(equipment);
        await _context.SaveChangesAsync();
        return true; // Equipment deleted successfully
    }

    public async Task<(int Code, string Message)> AssignEquipmentAsync(int employeeId, List<int> equipmentIds)
    {
        foreach (var equipmentId in equipmentIds)
        {
            var equipment = await _context.Equipments.FindAsync(equipmentId);

            if (equipment == null)
                return (404, $"Equipment with ID {equipmentId} not found");

            // If the equipment is already assigned to another employee
            if (equipment.EmployeeId.HasValue && equipment.EmployeeId != employeeId)
                return (400, $"Equipment ID {equipmentId} is already assigned to another employee");

            // Safe to assign
            equipment.EmployeeId = employeeId;
            equipment.AssignedDate = DateOnly.FromDateTime(DateTime.Now);
        }

        await _context.SaveChangesAsync();
        return (200, "Equipment assigned successfully");
    }

    // <inheritdoc/>
    public async Task<(int Code, string Message)> ForceAssignEquipmentAsync(int employeeId, List<int> equipmentIds)
    {
        foreach (var equipmentId in equipmentIds)
        {
            var equipment = await _context.Equipments.FindAsync(equipmentId);

            if (equipment == null){
                return (404, $"Equipment with ID {equipmentId} not found");
            }

            // Assign the equipment to the employee, regardless of whether it's already assigned to another employee
            equipment.EmployeeId = employeeId;
            equipment.AssignedDate = DateOnly.FromDateTime(DateTime.Now);
        }

        await _context.SaveChangesAsync();
        return (200, "Equipment assigned successfully");
    }
    
    /// <inheritdoc/>
    public async Task<List<EmpEquipItemDTO>> GetAllAssignedEquipItems()
    {
        // Get all assigned equipment items with their counts
        var equipmentWithCounts = await _context.Equipments
            .Include(e => e.EquipmentCategory)
            .Include(e => e.Employee)
            .Where(e => e.EmployeeId.HasValue)
            .Include(e => e.Employee.User)
            .GroupBy(e => e.EmployeeId)
            .Select(g => new
            {
                EquipmentItems = g.ToList(),
                Count = g.Count()
            })
            .ToListAsync();

        // Map equipment items to EmpEquipItemDTO
        var empEquipItems = equipmentWithCounts
            .SelectMany(g => g.EquipmentItems.Select(e => new EmpEquipItemDTO
            {
                Equipment = new EquipmentDTO
                {
                    EquipmentId = e.EquipmentId,
                    EmployeeId = e.EmployeeId,
                    EquipmentCatId = e.EquipmentCatId,
                    EquipmentCategoryName = e.EquipmentCategory.EquipmentCatName,
                    EquipmentName = e.EquipmentName,
                    AssignedDate = e.AssignedDate,
                    Condition = e.Condition
                },
                FullName = e.Employee?.User?.FullName,
                ProfilePicture = e.Employee?.User?.ProfilePicture,
                EmployDate = e.Employee?.EmployDate,
                IsSuspended = e.Employee?.IsSuspended,
                NumberOfItems = g.Count
            }))
            .ToList();

        return empEquipItems;
    }

    /// <inheritdoc/>
    public async Task<List<EquipmentDTO>> GetAllUnassignedEquipItems()
    {
        var equipment = await _context.Equipments
            .Include(e => e.EquipmentCategory)
            .Where(e => !e.EmployeeId.HasValue) // Where EmployeeId is null
            .Select(e => new EquipmentDTO
            {
                EquipmentId = e.EquipmentId,
                EquipmentCatId = e.EquipmentCatId,
                EquipmentCategoryName = e.EquipmentCategory.EquipmentCatName,
                EquipmentName = e.EquipmentName,
                AssignedDate = e.AssignedDate,
                Condition = e.Condition
            })
            .ToListAsync();

        return equipment;
    }
    
    /// <inheritdoc/>
    public async Task<(int Code, string Message)> UnlinkEquipmentFromEmployee(int equipmentId)
    {
        // Find the equipment item
        var equipment = await _context.Equipments.FindAsync(equipmentId);

        // If the equipment item is not found, return a 404 error
        if (equipment == null)
        {
            return (404, $"Equipment with ID {equipmentId} not found");
        }

        // Check if the equipment item is already unlinked
        if (!equipment.EmployeeId.HasValue)
        {
            return (400, $"Equipment with ID {equipmentId} is already unlinked");
        }

        // Unlink the equipment item from the employee
        equipment.EmployeeId = null;
        equipment.AssignedDate = null;

        // Save the changes to the database
        await _context.SaveChangesAsync();

        // Return a success message
        return (200, "Equipment unlinked from employee successfully");
    }

    /// <inheritdoc/>
    public async Task<(int Code, string Message)> MassUnlinkEquipmentFromEmployee(int employeeId)
    {
        // First verify if the employee exists
        var employee = await _context.Employees.FindAsync(employeeId);

        if (employee == null)
        {
            return (404, $"Employee with ID {employeeId} not found");
        }

        // Then verify if the employee has any equipment assigned to them
        var equipment = await _context.Equipments.Where(e => e.EmployeeId == employeeId).ToListAsync();

        if (equipment == null)
        {
            return (404, $"No equipment found for employee with ID {employeeId}");
        }

        // Unlink the equipment item(s) from the employee
        try
        {
            foreach (var item in equipment)
            {
                item.EmployeeId = null;
                item.AssignedDate = null;
            }

            await _context.SaveChangesAsync();

            return (200, $"{equipment.Count} equipment item(s) unlinked from employee successfully.");
        }
        catch (Exception ex)
        {
            return (500, $"Error unlinking equipment from employee: {ex.Message}");
        }
    }
}