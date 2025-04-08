using System;
using CoriCore.Models;

namespace CoriCore.DTOs;


public class PerformanceReviewDTO
{
    public int ReviewId { get; set; }
    public int AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    
    // Review Details
    public bool IsOnline { get; set; }
    public string? MeetLocation { get; set; }
    public string? MeetLink { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public string? DocUrl { get; set; }
    public ReviewStatus Status { get; set; }
} 