using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public enum MeetStatus
{
    Requested = 1,
    Upcoming = 2,
    Rejected = 3,
    Completed = 4
}

public class Meeting
{
    [Key] //Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MeetingId { get; set; }

    [Required]
    public int AdminId { get; set; }
    [ForeignKey("AdminId")]
    public Admin? Admin { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }

    // Meeting Details
    public bool? IsOnline { get; set; }

    [MaxLength(255)]
    public string? MeetLocation { get; set; }
    
    [MaxLength(255)]
    public string? MeetLink { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Purpose { get; set; }

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public MeetStatus Status { get; set; } = MeetStatus.Requested;

    
}
