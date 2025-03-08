// ========================================
// EMPLOYEE ENTITY
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
    // equipmentCatId (Primary Key)
    // ----------------------------------------
    [Key]
    public int equipmentCatId { get; set; }
    // ----------------------------------------

    // equipmentCatName
    // ----------------------------------------
    [Required]
    public string equipmentCatName { get; set; } = string.Empty;
    // ----------------------------------------
}

