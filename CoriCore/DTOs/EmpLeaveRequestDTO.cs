// Iné Smith

using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EmpLeaveRequestDTO
{

    // LeaveRequest Table
    public int LeaveRequestId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Comment { get; set; }
    public LeaveStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // LeaveType Table
    public string LeaveTypeName { get; set; }

    // User Table
    public int UserId { get; set; }
    public string FullName { get; set; }

    // Employee Table
    public int EmployeeId { get; set; }
    public EmployType EmployType { get; set; }
    public bool IsSuspended { get; set; }

    // LeaveBalance Table 
    public int RemainingDays { get; set; } //(Remaining Days of Leave Type)

    
}
