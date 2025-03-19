using System;
using System.ComponentModel.DataAnnotations;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class CreateEquipmentDTO
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int EquipmentCatId { get; set; }

    /// <summary>
    /// When the equipment was assigned (defaults to now if omitted)
    /// </summary>
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;
}
