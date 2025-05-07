using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

// DTO for updating equipment item
// All is optional if only updating one / many fields
public class UpdateEquipmentDTO
{
    public int? EmployeeId { get; set; }
    public int? EquipmentCatId { get; set; }
    public string? EquipmentName { get; set; }
    public DateOnly? AssignedDate { get; set; }
    public EquipmentCondition? Condition { get; set; }
}
