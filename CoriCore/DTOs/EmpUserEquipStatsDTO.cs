using System;

namespace CoriCore.DTOs;

/// <summary>
/// The employee's info with the number of equipment items assigned, and if they have any equipment of the same category as a parameter equipment item's categoy
/// </summary>
public class EmpUserEquipStatsDTO
{
    // Employee Information
    public int EmployeeId { get; set; }

    public bool IsSuspended { get; set; }

    // User Information
    public string? FullName { get; set; } // Nullable for unassigned equipment
    public string? ProfilePicture { get; set; } // Nullable for unassigned equipment

    // Unique Stats
    // --------------
    // Number of Equipment Items Assigned
    public int? NumberOfItems { get; set; } // Nullable for unassigned equipment

    // Does the employee have equipment of the same category?
    public bool HasItemOfSameEquipCat { get; set; }
    
    
}
