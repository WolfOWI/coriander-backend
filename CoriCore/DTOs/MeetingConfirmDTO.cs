using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

/// <summary>
/// DTO for confirming a meeting request (by admin), with all the meeting details
/// </summary>
public class MeetingConfirmDTO
{
    public bool IsOnline { get; set; }
    public string? MeetLocation { get; set; }
    public string? MeetLink { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
