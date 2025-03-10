// Ruan Klopper
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public enum LeaveStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public class LeaveRequest
{
    // ---------------------------------------
    // LeaveRequestId - Primary Key
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int LeaveRequestId { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // EmployeeId - Foreign Key
    [Required]
    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
    // ---------------------------------------

    // ---------------------------------------
    // LeaveTypeId - Foreign Key
    [Required]
    public int LeaveTypeId { get; set; }

    [ForeignKey("LeaveTypeId")]
    public virtual LeaveType LeaveType { get; set; } = null!;
    // ---------------------------------------

    // ---------------------------------------
    // StartDate - Required
    [Required]
    public DateOnly StartDate { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // EndDate - Required
    [Required]
    public DateOnly EndDate { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // Comment - Text
    public string? Comment { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // Status - ENUM
    [Required]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    // ---------------------------------------
}
