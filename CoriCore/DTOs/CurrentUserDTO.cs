using System;

namespace CoriCore.DTOs;

public class CurrentUserDTO
{
    public int UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; } // "Admin" or "Employee" or "Unassigned"
    public bool IsLinked { get; set; } // true if EmployeeId/AdminId exists
    public int? EmployeeId { get; set; }
    public int? AdminId { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsVerified { get; set; }

}
