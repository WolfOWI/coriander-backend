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

    public ICollection<PerformanceReview>? PerformanceReviews { get; set; } = new List<PerformanceReview>();

}
