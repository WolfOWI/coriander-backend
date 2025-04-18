using System;

namespace CoriCore.DTOs;

public class EmpEquipItemDTO
{
    // Equipment Information
    public EquipmentDTO Equipment { get; set; }

    // User Information
    public string? FullName { get; set; } // Nullable for unassigned equipment
    public string? ProfilePicture { get; set; } // Nullable for unassigned equipment

    // Number of Equipment Items
    public int? NumberOfItems { get; set; } // Nullable for unassigned equipment
    

}
