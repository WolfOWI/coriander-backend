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
    // employeeId (Primary Key)
    // ----------------------------------------
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int employeeId { get; set; }
    // ----------------------------------------

    // userId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int userId { get; set; }

    [ForeignKey("userId")]
    public User User { get; set; } = null!;
    // ----------------------------------------

    // gender
    // ----------------------------------------
    [Required]
    public Gender gender { get; set; } = Gender.Other;
    // ----------------------------------------

    // dateOfBirth
    // ----------------------------------------
    [Required]
    public DateOnly dateOfBirth { get; set; }
    // ----------------------------------------

    // phone (nullable)
    // ----------------------------------------
    [Phone]
    public string phone { get; set; } = string.Empty;
    // ----------------------------------------

    // jobTitle
    // ----------------------------------------
    [Required]
    public string jobTitle { get; set; } = string.Empty;
    // ----------------------------------------

    // department
    // ----------------------------------------
    [Required]
    public string department { get; set; } = string.Empty;
    // ----------------------------------------

    // salary
    // ----------------------------------------
    [Required]
    public decimal salary { get; set; }
    // ----------------------------------------
    
    // TODO Add this later (after pay cycle is implemented)
    // payCycleId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int payCycleId { get; set; }
    // [ForeignKey("payCycleId")]
    // public PayCycle PayCycle { get; set; } = null!;
    // ----------------------------------------

    // lastPayday (nullable)
    // ----------------------------------------
    public DateOnly? lastPayday { get; set; }
    // ----------------------------------------

    // lastPayDayIsPaid (nullable)
    // ----------------------------------------
    public bool? lastPayDayIsPaid { get; set; } = false;
    // ----------------------------------------

    // nextPayday (nullable)
    // ----------------------------------------
    public DateOnly? nextPayday { get; set; }
    // ----------------------------------------

    // employType
    // ----------------------------------------
    [Required]
    public EmployType employType { get; set; } = EmployType.FullTime;
    // ----------------------------------------

    // employDate
    // ----------------------------------------
    [Required]
    public DateOnly employDate { get; set; }
    // ----------------------------------------

    // isSuspended
    // ----------------------------------------
    public bool isSuspended { get; set; } = false;
    // ----------------------------------------

    // suspensionEndDate (nullable)
    // ----------------------------------------
    public DateOnly? suspensionEndDate { get; set; }
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
