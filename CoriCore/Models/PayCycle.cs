//-----------------------------------------------------------------------------
// PAYCYCLE ENTITY
//-----------------------------------------------------------------------------
// Kayla Posthumus

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public class PayCycle
{

    [Key] //Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PayCycleId { get; set; }

    [Required]
    [MaxLength(255)]
    public required string PayCycleName { get; set; }

    [Required]
    public int CycleDays { get; set; }

    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================
    public ICollection<Employee>? Employees { get; set; } = new List<Employee>();
    // ========================================
}
