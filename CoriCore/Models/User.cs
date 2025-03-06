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

    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int user_id { get; set; }

    [Required] // Not Null
    public string full_name { get; set; } = string.Empty;

    [Required] // Not Null
    [EmailAddress] // Email Address
    public string email { get; set; } = string.Empty;

    // Can be null (if user is logged in with Google)
    public string? password { get; set; }

    // Can be null (if user is NOT logged in with Google)
    public string? google_id { get; set; }

    // Can be null
    public string? profile_picture { get; set; }

    [Required] // Not Null
    public UserRole role { get; set; } = UserRole.Unassigned;
    

}
