using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class MeetingDTO
{  
    public int MeetingId { get; set; }
    public int AdminId { get; set; }
    public string? AdminName { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public bool? IsOnline { get; set; }
    public string? MeetLocation { get; set; }
    public string? MeetLink { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Purpose { get; set; }
    public DateTime RequestedAt { get; set; }
    public MeetStatus Status { get; set; }
}
