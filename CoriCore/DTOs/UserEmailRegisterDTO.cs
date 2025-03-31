using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class UserEmailRegisterDTO
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public UserRole Role { get; set; } = UserRole.Unassigned;
}
