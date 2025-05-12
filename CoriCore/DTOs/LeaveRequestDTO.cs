// Iné Smith

using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class LeaveRequestDTO
{
    // LeaveRequest Table
    // This class represents a leave request made by an employee.
    // It contains properties for the leave request details and the associated leave type.
    public int LeaveRequestId { get; set; }  // Primary Key
    public int EmployeeId { get; set; }  // Foreign Key

    public string EmployeeName { get; set; } = string.Empty;  // Employee Name
    public int LeaveTypeId { get; set; }  // Foreign Key
    public string LeaveType { get; set; } = string.Empty;  // Leave Type Name
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Comment { get; set; }
    public LeaveStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }


    // LeaveType Table (Nested within LeaveRequestDTO)
    public required string LeaveTypeName { get; set; }
    public string? Description { get; set; }
    public int DefaultDays { get; set; }
}