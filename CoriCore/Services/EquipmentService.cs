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
    public async Task<Equipment> EditEquipmentItemAsync(int equipmentId, EquipmentDTO equipmentDTO)
    {
        var equipment = await _context.Equipments.FindAsync(equipmentId);
        if (equipment == null)
        {
            throw new KeyNotFoundException($"Equipment with ID {equipmentId} not found.");
        }

        // Update the properties of the equipment item
        equipment.EmployeeId = equipmentDTO.EmployeeId;
        equipment.EquipmentName = equipmentDTO.EquipmentName;
        equipment.AssignedDate = equipmentDTO.AssignedDate;
        equipment.Condition = equipmentDTO.Condition;

        // Save changes to the database
        await _context.SaveChangesAsync();

        return equipment;
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

    // public async Task<List<EmpEquipItemDTO>> GetAllEmpEquipItems()
    // {
    //     // var empEquipItems = await _context.Equipments
    //     //     .Include(e => e.Employee)
    //     //     .Select(e => new EmpEquipItemDTO
    //     //     {
    //     //         Equipment = new EquipmentDTO
    //     //         {
    //     //             EquipmentId = e.EquipmentId,
    //     //             EquipmentName = e.EquipmentName,
    //     //             AssignedDate = e.AssignedDate,
    //     //             Condition = e.Condition
    //     //         },
    //     //         FullName = e.Employee.FullName,
    //     //         NumberOfItems = e.EmployeeId.HasValue ? 1 : 0
    //     //     })
    //     //     .ToListAsync();

    //     // return empEquipItems;
    // }

}