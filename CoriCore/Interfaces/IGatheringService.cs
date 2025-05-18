using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IGatheringService
{

    // GET
    // ========================================
    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByAdminId(int adminId);
    
    /// <summary>
    /// Get all meetings and performance reviews for an employee, sorted by start date
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date</returns>
    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByEmployeeId(int employeeId);
    // ========================================

}
