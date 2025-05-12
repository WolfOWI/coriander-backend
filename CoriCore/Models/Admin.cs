using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public class Admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int AdminId { get; set; }

    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")] // Link UserId to User.UserId
    public virtual User User { get; set; } = null!;


    // Google Meet Token fields
    public string? GMeetAccessToken { get; set; }
    public string? GMeetRefreshToken { get; set; }
    public DateTime? GMeetTokenGeneratedAt { get; set; }
    public int? GMeetTokenExpiresIn { get; set; }

    public ICollection<PerformanceReview>? PerformanceReviews { get; set; } = new List<PerformanceReview>();

}
