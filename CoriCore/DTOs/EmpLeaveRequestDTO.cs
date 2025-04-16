// Iné Smith

using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EmpLeaveRequestDTO
{
    // User Table
    public int UserId { get; set; }
    public string FullName { get; set; }
    public bool IsVerified { get; set; }
    public string VerificationCode { get; set; }

    // Employee Table
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; }

    // LeaveRequest Table
    public int LeaveRequestId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Comment { get; set; }
    public LeaveStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // LeaveBalance Table
    public int RemainingDays { get; set; }

    // LeaveType Table
    public string LeaveTypeName { get; set; }
}
