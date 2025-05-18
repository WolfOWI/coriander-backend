using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

/// <summary>
/// DTO for updating a meeting request such as an admin accepting a meeting request & filling in details, or simply editing the details
/// </summary>
public class MeetingUpdateDTO
{
    public bool? IsOnline { get; set; }
    public string? MeetLocation { get; set; }
    public string? MeetLink { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
