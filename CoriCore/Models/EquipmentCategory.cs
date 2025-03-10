// ========================================
// EQUIPMENT CATEGORY ENTITY
// ========================================
// Iné Smith

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

// ========================================
// EQUIPMENT CATEGORY ENTITY
// ========================================

public class EquipmentCategory
{
    // EquipmentCatId (Primary Key)
    // ----------------------------------------
    [Key]
    public int EquipmentCatId { get; set; }
    // ----------------------------------------

    // EquipmentCatName
    // ----------------------------------------
    [Required]
    public string EquipmentCatName { get; set; } = string.Empty;
    // ----------------------------------------

    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================
    public ICollection<Equipment>? Equipment { get; set; } = new List<Equipment>(); 
    // ========================================
}

