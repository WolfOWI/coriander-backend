// Iné Smith

using System;
using CoriCore.Models;


namespace CoriCore.DTOs;

public class ApplyForLeaveDTO
{
    // LeaveRequest Table
    public int EmployeeId { get; set; }  // Foreign Key
    public int LeaveTypeId { get; set; }  // Foreign Key
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Comment { get; set; }
    public LeaveStatus Status { get; set; }  // e.g., Approved, Pending, Rejected
    public DateTime CreatedAt { get; set; }  // Date when the request was created
    // public int Duration { get; set; }  // Duration of the leave in days
}
