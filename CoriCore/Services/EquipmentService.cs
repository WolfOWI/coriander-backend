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

    public async Task<List<EquipmentDTO>> GetEquipmentByEmployeeId(int employeeId)
    {
        var equipment = await _context.Equipments
            .Include(e => e.EquipmentCategory)
            .Where(e => e.EmployeeId == employeeId)
            .Select(e => new EquipmentDTO
            {
                EquipmentId = e.EquipmentId,
                EmployeeId = e.EmployeeId,
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
    public async Task<IEnumerable<Equipment>> CreateEquipmentItemsAsync(List<EquipmentDTO> equipmentDTOs)
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

    // Delete all equipment items by employee id
    public async Task<string> DeleteEquipmentsByEmployeeIdAsync(int employeeId)
    {

        // Check if the employee exists
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            return "Employee not found";
        }

        var equipments = await _context.Equipments.Where(e => e.EmployeeId == employeeId).ToListAsync();
        if (equipments.Count == 0)
        {
            return "No equipment items found";
        }

        // Delete the equipment items
        _context.Equipments.RemoveRange(equipments);

        // Save changes to the database
        await _context.SaveChangesAsync();

        return "Equipment items deleted successfully";
    }

}