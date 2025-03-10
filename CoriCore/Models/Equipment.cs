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
    [Required]
    public int EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
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
    [Required]
    public DateTime AssignedDate { get; set; }
    // ----------------------------------------

    // Condition
    // ----------------------------------------
    [Required]
    public EquipmentCondition Condition { get; set; } = EquipmentCondition.New;
    // ----------------------------------------


    

}