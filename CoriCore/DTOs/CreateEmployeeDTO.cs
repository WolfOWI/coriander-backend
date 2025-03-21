using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class CreateEmployeeDto
{
    public int UserId { get; set; }
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string PhoneNumber { get; set; }
    public required string JobTitle { get; set; }
    public required string Department { get; set; }
    public decimal SalaryAmount { get; set; }
    public int PayCycleId { get; set; }
    public DateOnly? PastPayday { get; set; }
    public bool? PastPaydayIsPaid { get; set; }
    public DateOnly? NextPayday { get; set; }
    public EmployType EmployType { get; set; }
    public DateOnly EmployDate { get; set; }
    public bool IsSuspended { get; set; }
    public DateOnly? SuspensionEndDate { get; set; }
}

