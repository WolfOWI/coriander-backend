// ========================================
// ADMIN ENTITY
// ========================================
// Wolf Botha

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoriCore.Models;

public class Admin
{

    
    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int adminId { get; set; }

    [Required] // Not Null
     public int userId { get; set; } // Foreign Key

    [ForeignKey("userId")] // Explicitly linking UserId to User.UserId
    public User User { get; set; } = null!;

}
