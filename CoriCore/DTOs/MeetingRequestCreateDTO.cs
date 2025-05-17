using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class MeetingRequestCreateDTO
{
    public int AdminId { get; set; }
    public int EmployeeId { get; set; }
    public string? Purpose { get; set; }
    
}
