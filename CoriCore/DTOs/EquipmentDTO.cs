using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EquipmentDTO
{
    public int EquipmentId { get; set; }
    public int EmployeeId { get; set; }
    public int EquipmentCatId { get; set; }
    public string? EquipmentCategoryName { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateOnly AssignedDate { get; set; }
    public EquipmentCondition Condition { get; set; }
} 