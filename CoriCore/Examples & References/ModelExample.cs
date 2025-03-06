using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogAdoptionApp.Models
{
    // ============================================================
    // 🐶 DOG ENTITY (Represents a dog in the system)
    // ============================================================
    
    // 📌 The [Index] attribute optimizes database lookups for the "Name" column
    // Setting `IsUnique = false` means multiple dogs can have the same name.
    [Index(nameof(Name), IsUnique = false)]
    public class Dog
    {
        // ✅ PRIMARY KEY (UNIQUE IDENTIFIER FOR EACH DOG)
        [Key]  // Marks this property as the PRIMARY KEY (PK)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-incrementing ID
        public int DogId { get; set; }

        // ✅ NAME (REQUIRED, WITH MAX LENGTH)
        [Required]  // Prevents null values (this field must always have a value)
        [StringLength(50)]  // Limits the length to 50 characters
        public string Name { get; set; } = string.Empty;

        // ✅ AGE (RANGE CONSTRAINT)
        [Required]
        [Range(0, 30)]  // Ensures the age is between 0 and 30 years
        public int Age { get; set; }

        // ✅ ENUM TYPE (PREDEFINED BREEDS)
        [Required]
        public BreedType Breed { get; set; }  // Uses the BreedType Enum defined below

        // ✅ WEIGHT (DECIMAL VALUE WITH TWO DECIMAL PLACES)
        [Required]
        [Column(TypeName = "decimal(5,2)")]  // Stores decimal values like 12.50 kg
        public decimal Weight { get; set; }

        // ✅ BOOLEAN FLAG (DEFAULTS TO TRUE)
        [Required]
        public bool IsAvailableForAdoption { get; set; } = true;  // Defaults to "available"

        // ✅ ONE-TO-MANY RELATIONSHIP (A DOG CAN HAVE MULTIPLE ADOPTION REQUESTS)
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
    }

    // ============================================================
    // 📌 ENUM: BREED TYPE (Defines allowed breed options)
    // ============================================================
    public enum BreedType
    {
        Labrador,
        GermanShepherd,
        Beagle,
        Bulldog,
        Poodle,
        Mixed
    }

    // ============================================================
    // 🏡 ADOPTER ENTITY (Represents a person adopting a dog)
    // ============================================================
    public class Adopter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdopterId { get; set; }

        [Required]
        [StringLength(100)]  // Ensures full names do not exceed 100 characters
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]  // Ensures the input follows a valid email format
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]  // Ensures a valid phone number format
        public string PhoneNumber { get; set; } = string.Empty;

        // ✅ ONE-TO-MANY: An adopter can submit multiple adoption requests
        public ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
    }

    // ============================================================
    // 📜 ADOPTION REQUEST ENTITY (Tracks dog adoption applications)
    // ============================================================
    public class AdoptionRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        // ✅ FOREIGN KEY TO DOG TABLE (Each request is linked to one Dog)
        [Required]
        public int DogId { get; set; }  // Foreign key for Dog

        [ForeignKey("DogId")]
        public Dog Dog { get; set; } = null!;  // Navigation property (linked object)

        // ✅ FOREIGN KEY TO ADOPTER TABLE (Each request is linked to one Adopter)
        [Required]
        public int AdopterId { get; set; }  // Foreign key for Adopter

        [ForeignKey("AdopterId")]
        public Adopter Adopter { get; set; } = null!;  // Navigation property (linked object)

        // ✅ ENUM STATUS (Tracks whether adoption is pending, approved, or rejected)
        [Required]
        public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;  // Defaults to "Pending"

        // ✅ AUTOMATIC TIMESTAMP (Stores the date of the request)
        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    }

    // ============================================================
    // 📌 ENUM: ADOPTION STATUS (Tracks application progress)
    // ============================================================
    public enum AdoptionStatus
    {
        Pending,
        Approved,
        Rejected
    }

    // ============================================================
    // 🌎 LOCATION ENTITY (Represents a shelter or foster home)
    // ============================================================
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        // ✅ MANY-TO-MANY: A location can have multiple dogs, and dogs can be in multiple locations
        public ICollection<Dog> Dogs { get; set; } = new List<Dog>();
    }

    // ============================================================
    // 🔄 MANY-TO-MANY JOIN TABLE (Dogs ↔ Locations)
    // ============================================================
    public class DogLocation
    {
        [Key]
        public int DogId { get; set; }

        [Key]
        public int LocationId { get; set; }

        [ForeignKey("DogId")]
        public Dog Dog { get; set; } = null!;

        [ForeignKey("LocationId")]
        public Location Location { get; set; } = null!;
    }
}
