namespace CoriCore.DTOs;

public class PostPerformanceReviewDTO
{
    public int AdminId { get; set; }
    public int EmployeeId { get; set; }
    public bool IsOnline { get; set; }
    public string? MeetLocation { get; set; }
    public string? MeetLink { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public string? DocUrl { get; set; }
    public ReviewStatusDTO Status { get; set; } = ReviewStatusDTO.Upcoming;
}
