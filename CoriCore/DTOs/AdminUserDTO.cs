using System;

namespace CoriCore.DTOs;

public class AdminUserDTO
{
    public int AdminId { get; set; }
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }

}
