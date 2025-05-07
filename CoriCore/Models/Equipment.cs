// ========================================
// EQUIPMENT ENTITY
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
    public int EquipmentId { get; set; }
    // ----------------------------------------

    // EmployeeId (Foreign Key)
    // ----------------------------------------
    public int? EmployeeId { get; set; } // Nullable for unassigned equipment

    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; } = null!;
    // ----------------------------------------

    // EquipmentName
    // ----------------------------------------
    [Required]
    public string EquipmentName { get; set; } = string.Empty;
    // ----------------------------------------

    // EquipmentCatId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int EquipmentCatId { get; set; }

    [ForeignKey("EquipmentCatId")]
    public virtual EquipmentCategory EquipmentCategory { get; set; } = null!;
    // ----------------------------------------

    // AssignedDate
    // ----------------------------------------
    public DateOnly? AssignedDate { get; set; } // Nullable for unassigned equipment
    // ----------------------------------------

    // Condition
    // ----------------------------------------
    [Required]
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;
    // ----------------------------------------




}