using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

// DTO for registering a new admin user via email
public class AdminUserEmailRegisterDTO
{
    // User Model
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public UserRole Role { get; set; } = UserRole.Admin;

    // Admin Model
    public int AdminId { get; set; }
}
