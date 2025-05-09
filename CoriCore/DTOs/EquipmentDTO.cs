// Equipment DTO
// ========================================

using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EquipmentDTO
{
    public int EquipmentId { get; set; }
    public int? EmployeeId { get; set; } // Nullable for unassigned equipment
    public int EquipmentCatId { get; set; }
    public string? EquipmentCategoryName { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateOnly? AssignedDate { get; set; } // Nullable for unassigned equipment
    public EquipmentCondition Condition { get; set; }
} 