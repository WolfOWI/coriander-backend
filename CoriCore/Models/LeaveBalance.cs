using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public class LeaveBalance
{
    // ---------------------------------------
    // LeaveBalanceId - Primary Key
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int LeaveBalanceId { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // EmployeeId - Foreign Key
    [Required]
    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;
    // ---------------------------------------

    // ---------------------------------------
    // LeaveTypeId - Foreign Key
    [Required]
    public int LeaveTypeId { get; set; }

    [ForeignKey("LeaveTypeId")]
    public LeaveType LeaveType { get; set; } = null!;
    // ---------------------------------------

    // ---------------------------------------
    // RemainingDays - Int
    public int RemainingDays { get; set; }
    // ---------------------------------------
}
