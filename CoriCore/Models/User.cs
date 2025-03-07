// ========================================
// USER ENTITY
// ========================================
// Wolf Botha

using System;
// For Data Annotations (Like Primary Key, Auto-increment, etc.)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

// Enum for User Role
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
    // userId (Primary Key)
    // ----------------------------------------
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int userId { get; set; }
    // ----------------------------------------

    // fullName (Required)
    // ----------------------------------------
    [Required]
    public string fullName { get; set; } = string.Empty;
    // ----------------------------------------

    // email (Required)
    // ----------------------------------------
    [Required]
    [EmailAddress]
    public string email { get; set; } = string.Empty;
    // ----------------------------------------

    // password (Can be null if user is logged in with Google)
    // ----------------------------------------
    public string? password { get; set; }
    // ----------------------------------------

    // googleId (Can be null if user is NOT logged in with Google)
    // ----------------------------------------
    public string? googleId { get; set; }
    // ----------------------------------------

    // profilePicture (Can be null)
    // ----------------------------------------
    public string? profilePicture { get; set; }
    // ----------------------------------------

    // role (Required)
    // ----------------------------------------
    [Required]
    public UserRole role { get; set; } = UserRole.Unassigned;
    // ----------------------------------------
    // ========================================


    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================

    // One-to-one relationship with Admin (a user can optionally be an admin)
    // ----------------------------------------
    public Admin? Admin { get; set; }
    // ----------------------------------------

    // One-to-one relationship with Employee (a user can optionally be an employee)
    // ----------------------------------------
    public Employee? Employee { get; set; }
    // ----------------------------------------
    
    // ========================================

}
