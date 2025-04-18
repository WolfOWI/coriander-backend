// Ruan Klopper

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public class LeaveType
{
    // ---------------------------------------
    // LeaveRequestId - Primary Key
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int LeaveTypeId { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // LeaveTypeName - VARCHAR
    [Required]
    public required string LeaveTypeName { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // Description - Text
    public string? Description { get; set; }
    // ---------------------------------------

    // ---------------------------------------
    // DeafultDays - int
    [Required]
    public required int DefaultDays { get; set; }
    // ---------------------------------------

    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================
    public ICollection<LeaveRequest>? LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance>? LeaveBalances { get; set; } = new List<LeaveBalance>();
    // ========================================
}
