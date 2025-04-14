using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class EmployeeDto
{
    public int UserId { get; set; }
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string PhoneNumber { get; set; }
    public required string JobTitle { get; set; }
    public required string Department { get; set; }
    public decimal SalaryAmount { get; set; }
    public PayCycle PayCycle { get; set; }
    public DateOnly? LastPaidDate { get; set; }
    public EmployType EmployType { get; set; }
    public DateOnly EmployDate { get; set; }
    public bool IsSuspended { get; set; } = false; // Default to False
}

