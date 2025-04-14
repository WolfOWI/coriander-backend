// EmpUser Update DTO
// ========================================

using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EmployeeUpdateDTO
{
    public string? FullName { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public decimal? SalaryAmount { get; set; }
    public PayCycle? PayCycle { get; set; }
    public DateOnly? LastPaidDate { get; set; }
    public EmployType? EmployType { get; set; }
    public DateOnly? EmployDate { get; set; }
    public bool? IsSuspended { get; set; }
} 