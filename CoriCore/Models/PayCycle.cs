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
    public int payCycleId { get; set; }

    [Required]
    [MaxLength(255)]
    public string payCycleName { get; set; }

    [Required]
    public int cycleDays { get; set; }
}
