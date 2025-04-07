// ========================================
// USER ENTITY
// ========================================
// Wolf Botha

using System;
using System.Collections.Generic;
// For Data Annotations (Like Primary Key, Auto-increment, etc.)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;


/// <summary>
/// Enum for user roles (used for role-based access control)
/// 0 = Unassigned, 1 = Employee, 2 = Admin
/// </summary>
public enum UserRole
{
    Unassigned = 0,
    Employee = 1,
    Admin = 2
}

public class User
{
    // Properties
    // ========================================
    // UserId (Primary Key)
    // ----------------------------------------
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int UserId { get; set; }
    // ----------------------------------------

    // FullName (Required)
    // ----------------------------------------
    [Required]
    public string FullName { get; set; } = string.Empty;
    // ----------------------------------------

    // Email (Required)
    // ----------------------------------------
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    // ----------------------------------------

    // Password (Can be null if user is logged in with Google)
    // ----------------------------------------
    public string? Password { get; set; }
    // ----------------------------------------

    // GoogleId (Can be null if user is NOT logged in with Google)
    // ----------------------------------------
    public string? GoogleId { get; set; }
    // ----------------------------------------

    // ProfilePicture (Can be null)
    // ----------------------------------------
    public string? ProfilePicture { get; set; }
    // ----------------------------------------

    // Role (Required)
    // ----------------------------------------
    [Required]
    public UserRole Role { get; set; } = UserRole.Unassigned;
    // ----------------------------------------

    // IsVerified (Required)
    // ----------------------------------------
    [Required]
    public bool IsVerified { get; set; } = false;
    // ----------------------------------------

    // VerificationCode (Can be null)
    // ----------------------------------------
    public string? VerificationCode { get; set; }
    // ----------------------------------------

    // CodeGeneratedAt (Can be null)
    // ----------------------------------------
    public DateTime? CodeGeneratedAt { get; set; }
    // ----------------------------------------
    // ========================================


    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================

    // One-to-one relationship with Admin (a user can optionally be an admin)
    // ----------------------------------------
    public virtual Admin? Admin { get; set; }
    // ----------------------------------------

    // One-to-one relationship with Employee (a user can optionally be an employee)
    // ----------------------------------------
    public virtual Employee? Employee { get; set; }
    // ----------------------------------------

    // ========================================

}
