// ========================================
// EMPLOYEE ENTITY
// ========================================
// Wolf Botha

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

// Enum for Gender
public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2
}

// Enum for EmployType
public enum EmployType
{
    FullTime = 0,
    PartTime = 1,
    Contract = 2,
    Intern = 3
}

// Enum for PayCycle
public enum PayCycle
{
    Monthly = 0,
    BiWeekly = 1,
    Weekly = 2,
}

public class Employee
{
    // Properties
    // ========================================
    // EmployeeId (Primary Key)
    // ----------------------------------------
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int EmployeeId { get; set; }
    // ----------------------------------------

    // UserId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    // ----------------------------------------

    // Gender
    // ----------------------------------------
    [Required]
    public Gender Gender { get; set; } = Gender.Other;
    // ----------------------------------------

    // DateOfBirth
    // ----------------------------------------
    [Required]
    public DateOnly DateOfBirth { get; set; }
    // ----------------------------------------

    // PhoneNumber (nullable)
    // ----------------------------------------
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    // ----------------------------------------

    // JobTitle
    // ----------------------------------------
    [Required]
    public string JobTitle { get; set; } = string.Empty;
    // ----------------------------------------

    // Department
    // ----------------------------------------
    [Required]
    public string Department { get; set; } = string.Empty;
    // ----------------------------------------

    // SalaryAmount
    // ----------------------------------------
    [Required]
    public decimal SalaryAmount { get; set; }
    // ----------------------------------------

    // PayCycle
    // ----------------------------------------
    [Required]
    public PayCycle PayCycle { get; set; }
    // ----------------------------------------

    // PastPayday (nullable)
    // ----------------------------------------
    public DateOnly? PastPayday { get; set; }
    // ----------------------------------------

    // PastPaydayIsPaid (nullable)
    // ----------------------------------------
    public bool? PastPaydayIsPaid { get; set; } = false;
    // ----------------------------------------

    // NextPayday (nullable)
    // ----------------------------------------
    public DateOnly? NextPayday { get; set; }
    // ----------------------------------------

    // EmployType
    // ----------------------------------------
    [Required]
    public EmployType EmployType { get; set; } = EmployType.FullTime;
    // ----------------------------------------

    // EmployDate
    // ----------------------------------------
    [Required]
    public DateOnly EmployDate { get; set; }
    // ----------------------------------------

    // IsSuspended
    // ----------------------------------------
    public bool IsSuspended { get; set; } = false;
    // ----------------------------------------

    // ========================================


    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================
    public ICollection<PerformanceReview>? PerformanceReviews { get; set; } = new List<PerformanceReview>();
    public ICollection<Equipment>? Equipment { get; set; } = new List<Equipment>();
    public ICollection<LeaveBalance>? LeaveBalances { get; set; } = new List<LeaveBalance>();
    public ICollection<LeaveRequest>? LeaveRequests { get; set; } = new List<LeaveRequest>();
    // ========================================
}
