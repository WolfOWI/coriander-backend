using System;
using CoriCore.Models;

namespace CoriCore.DTOs;

public class CreateEmployeeDTO
{
    public int UserId { get; set; }
    public CoriCore.Models.Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal SalaryAmount { get; set; }
    public CoriCore.Models.EmployType EmployType { get; set; }
    public DateOnly EmployDate { get; set; }
    public bool IsSuspended { get; set; }
    public DateOnly? SuspensionEndDate { get; set; }

    // PayCycle fields
    public string PayCycleName { get; set; } = string.Empty;
    public int CycleDays { get; set; }

    // Optionally, Equipment fields (can be an empty list)
    public List<CreateEquipmentDTO>? Equipment { get; set; } = new List<CreateEquipmentDTO>();
}
