// Performance Review Service Interface
// ========================================

using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IPerformanceReviewService
{
    Task<IEnumerable<PerformanceReview>> GetPrmByStartDateAdminId(int adminId, DateTime startDate);

    Task<IEnumerable<PerformanceReview>> GetPrmByEmpId(int employeeId);

    Task<List<EmpUserRatingMetricsDTO>> GetAllEmpUserRatingMetrics();

    /// <summary>
    /// Get random employee rating metrics by number of employees
    /// </summary>
    /// <param name="numberOfEmps">The number of employees to get</param>
    /// <returns>A list of employee rating metrics</returns>
    Task<List<EmpUserRatingMetricsDTO>> GetRandomEmpUserRatingMetricsByNum(int numberOfEmps);

    /// <summary>
    /// Get employee rating metrics by employee id
    /// </summary>
    /// <param name="employeeId">The id of the employee</param>
    /// <returns>The employee rating metrics</returns>
    Task<EmpUserRatingMetricsDTO?> GetEmpUserRatingMetricsByEmpId(int employeeId);

    Task<PerformanceReview> CreatePerformanceReview(PerformanceReview review);

    Task<PerformanceReview> UpdatePerformanceReview(int id, PerformanceReview review);

    Task<bool> DeletePerformanceReview(int id);

    Task<IEnumerable<PerformanceReview>> GetAllUpcomingPrm();
    Task<List<TopRatedEmployeesDTO>> GetTopRatedEmployees();
    Task<PerformanceReview> UpdateReviewStatus(int reviewId, ReviewStatus newStatus);

}

