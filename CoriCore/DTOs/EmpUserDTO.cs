using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EmpUserDTO
{
    // User Information
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public UserRole Role { get; set; }

    // Employee Information
    public int EmployeeId { get; set; }
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal SalaryAmount { get; set; }
    public PayCycle PayCycle { get; set; }
    public DateOnly? PastPayday { get; set; }
    public bool? PastPaydayIsPaid { get; set; }
    public DateOnly? NextPayday { get; set; }
    public EmployType EmployType { get; set; }
    public DateOnly EmployDate { get; set; }
    public bool IsSuspended { get; set; }
}
