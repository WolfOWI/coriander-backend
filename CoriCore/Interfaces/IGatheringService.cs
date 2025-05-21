using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IGatheringService
{
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


    /// <summary>
    /// Get all upcoming and completed gatherings (meetings and performance reviews) for an employee, sorted by start date
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date (descending)</returns>
    Task<IEnumerable<GatheringDTO>> GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(int employeeId);

    /// <summary>
    /// Get all gatherings (meetings and performance reviews) for an admin, sorted by start date
    /// </summary>
    /// <param name="adminId">The ID of the admin</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date</returns>
    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByAdminId(int adminId);

    /// <summary>
    /// Get all gatherings (meetings and performance reviews) for an admin, by status, sorted by start date
    /// </summary>
    /// <param name="adminId">The ID of the admin</param>
    /// <param name="status">The status of the gatherings (can only be "Upcoming" or "Completed")</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date</returns>
    Task<IEnumerable<GatheringDTO>> GetAllGatheringsByAdminIdAndStatus(int adminId, string status);


    /// <summary>
    /// Get upcoming and completed gatherings (meetings and performance reviews) for an admin for a specific month
    /// </summary>
    /// <param name="adminId">The ID of the admin</param>
    /// <param name="month">The month to get the gatherings for (1-12)</param>
    /// <returns>List of gatherings (meetings and performance reviews) sorted by start date</returns>
    Task<IEnumerable<GatheringDTO>> GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(int adminId, string month);


}
