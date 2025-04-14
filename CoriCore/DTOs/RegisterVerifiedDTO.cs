using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class RegisterVerifiedDTO
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? ProfilePicture { get; set; }
    public required string Code { get; set; }
    public UserRole Role { get; set; } = UserRole.Unassigned;
}
