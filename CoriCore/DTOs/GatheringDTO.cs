using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public enum GatheringType
{
    PerformanceReview = 1,
    Meeting = 2,
}

public class GatheringDTO
{
    // Common properties
    public int Id { get; set; }
    public GatheringType Type { get; set; }
    public int AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public bool? IsOnline { get; set; }
    public string? MeetLocation { get; set; }
    public string? MeetLink { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Meeting-specific properties
    public string? Purpose { get; set; }
    public DateTime? RequestedAt { get; set; }
    public MeetStatus? MeetingStatus { get; set; }

    // Performance Review-specific properties
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public string? DocUrl { get; set; }
    public ReviewStatus? ReviewStatus { get; set; }
} 