using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class MeetingRequestDTO
{
    public int MeetingId { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Purpose { get; set; }
    public DateTime RequestedAt { get; set; }
    public MeetStatus Status { get; set; }

}
