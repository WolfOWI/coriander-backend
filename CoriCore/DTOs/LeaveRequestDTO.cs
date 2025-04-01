// Iné Smith

using System;

namespace CoriCore.DTOs;

public class LeaveRequestDTO
{
    // LeaveRequest Table
    public int LeaveRequestId { get; set; }  // Primary Key
    public int EmployeeId { get; set; }  // Foreign Key
    public int LeaveTypeId { get; set; }  // Foreign Key
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }  // e.g., Approved, Pending, Rejected
    public DateTime CreatedAt { get; set; }  // Date when the request was created
    public int Duration { get; set; }  // Duration of the leave in days

    // LeaveType Table (Nested within LeaveRequestDTO)
    public string LeaveTypeName { get; set; }
    public string Description { get; set; }
    public int DefaultDays { get; set; }
}