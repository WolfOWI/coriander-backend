using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public enum Status
{
    Pending = 0,
    Upcoming = 1,
    Completed = 2
}

public class PerformanceReview
{

    [Key] //Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReviewId { get; set; }

    // Foreign Key
    [Required]
    public int AdminId { get; set; }

    [ForeignKey("AdminId")]
    public Admin Admin { get; set; } = null!;


    [Required]
    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!; 

    // Review Details
        [MaxLength(255)]
        public string? MeetLink { get; set; }  // Nullable

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; } // Nullable with CHECK constraint

        public string? Comment { get; set; }  // Nullable

        public string? DocUrl { get; set; }  // Nullable

        // Enum-like Status
        [Required]
        [Column(TypeName = "varchar(20)")]  // To match ENUM behavior

        public Status Status { get; set; } = Status.Pending;// Default 'Pending'

}
