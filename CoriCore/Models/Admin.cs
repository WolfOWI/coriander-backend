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
    // adminId (Primary Key)
    // ----------------------------------------
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int adminId { get; set; }
    // ----------------------------------------

    // userId (Foreign Key)
    // ----------------------------------------
    [Required] // Not Null
    public int userId { get; set; }

    [ForeignKey("userId")] // Link UserId to User.UserId
    public User User { get; set; } = null!;
    // ----------------------------------------

}
