// ========================================
// EMPLOYEE ENTITY
// ========================================
// Wolf Botha

using System;
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
    public User User { get; set; } = null!;
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

    // PayCycleId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int PayCycleId { get; set; }
    [ForeignKey("PayCycleId")]
    public PayCycle PayCycle { get; set; } = null!;
    // ----------------------------------------

    // LastPayday (nullable)
    // ----------------------------------------
    public DateOnly? LastPayday { get; set; }
    // ----------------------------------------

    // LastPayDayIsPaid (nullable)
    // ----------------------------------------
    public bool? LastPayDayIsPaid { get; set; } = false;
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

    // SuspensionEndDate (nullable)
    // ----------------------------------------
    public DateOnly? SuspensionEndDate { get; set; }
    // ----------------------------------------
    // ========================================


    // TODO Add relationships later
    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================
    // One-to-many relationship with PerformanceReview (an employee can have zero or many performance reviews)
    // ----------------------------------------
    // public ICollection<PerformanceReview>? PerformanceReviews { get; set; } = new List<PerformanceReview>();
    // ----------------------------------------

    // One-to-many relationship with Equipment (an employee can have zero or many equipment items)
    // ----------------------------------------
    // public ICollection<Equipment>? EquipmentItems {get; set;} = new List<Equipment>();
    // ----------------------------------------

    // One-to-many relationship with LeaveBalance (an employee MUST have many leave balances)
    // ----------------------------------------
    // public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    // ----------------------------------------

    // One-to-many relationship with LeaveRequest (an employee can have zero or many leave requests)
    // ----------------------------------------
    // public ICollection<LeaveRequest>? LeaveRequests { get; set; } = new List<LeaveRequest>();
    // ----------------------------------------

    // ========================================
}
