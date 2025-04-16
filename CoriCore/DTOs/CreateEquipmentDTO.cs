using System;
using System.ComponentModel.DataAnnotations;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class CreateEquipmentDTO
{
    public int? EmployeeId { get; set; }
    public int EquipmentCatId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateOnly AssignedDate { get; set; }
    public EquipmentCondition Condition { get; set; }
}
