// ========================================
// ADMIN ENTITY
// ========================================
// Wolf Botha

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public class Admin
{
    // Properties
    // ========================================

    // AdminId (Primary Key)
    // ----------------------------------------
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int AdminId { get; set; }
    // ----------------------------------------

    // UserId (Foreign Key)
    // ----------------------------------------
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")] // Link UserId to User.UserId
    public virtual User User { get; set; } = null!;
    // ----------------------------------------

    // ========================================


    // RELATIONSHIPS (Not Foreign Keys)
    // ========================================

    // One-to-many relationship with PerformanceReview (an admin can have zero or many performance reviews)
    // ----------------------------------------
    public ICollection<PerformanceReview>? PerformanceReviews { get; set; } = new List<PerformanceReview>();
    // ----------------------------------------

    // ========================================

}
