using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IGatheringService
{

    // GET
    // ========================================
    /// <summary>
    /// Get all meetings and performance reviews for an employee, sorted by start date
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date</returns>
    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByEmployeeId(int employeeId);


    /// <summary>
    /// Get all gatherings (meetings and performance reviews) for an employee, by status, sorted by start date
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <param name="status">The status of the gatherings (can only be "Upcoming" or "Completed")</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date</returns>
    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByEmployeeIdAndStatus(int employeeId, string status);



    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByAdminId(int adminId);
    // ========================================

}
