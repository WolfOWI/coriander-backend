// ========================================
// EMPLOYEE ENTITY
// ========================================
// Iné Smith

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

// Enum for EquipmentCondition
public enum EquipmentCondition
{
    New = 0,
    Good = 1,
    Decent = 2,
    Used = 3
}

public class Equipment
{
    // EquipmentId (Primary Key)
    // ----------------------------------------
    [Key]
    public int equipmentId { get; set; }
    // ----------------------------------------

    // employeeId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int employeeId { get; set; }

    [ForeignKey("employeeId")]
    public Employee Employee { get; set; } = null!;
    // ----------------------------------------

    // equipmentCatId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int equipmentCatId { get; set; }

    [ForeignKey("equipmentCatId")]
    public EquipmentCategory EquipmentCategory { get; set; } = null!;
    // ----------------------------------------

    // assignedDate
    // ----------------------------------------
    [Required]
    public DateTime assignedDate { get; set; }
    // ----------------------------------------

    // condition
    // ----------------------------------------
    [Required]
    public EquipmentCondition condition { get; set; } = EquipmentCondition.New;
    // ----------------------------------------
}