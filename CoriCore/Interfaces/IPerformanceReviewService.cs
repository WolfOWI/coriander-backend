using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IPerformanceReviewService
{
    Task<IEnumerable<PerformanceReview>> GetPrmByStartDateAdminId(int adminId, DateTime startDate);

    Task<IEnumerable<PerformanceReview>> GetPrmByEmpId(int employeeId);

    Task<List<EmpUserRatingMetricsDTO>> GetAllEmpUserRatingMetrics();

    Task<EmpUserRatingMetricsDTO?> GetEmpUserRatingMetricsByEmpId(int employeeId);

    Task<PerformanceReview> CreatePerformanceReview(PerformanceReview review);

    Task<PerformanceReview> UpdatePerformanceReview(int id, PerformanceReview review);

    Task<bool> DeletePerformanceReview(int id);

    Task<IEnumerable<PerformanceReview>> GetAllUpcomingPrm();

    Task<List<EmpUserRatingMetricsDTO>> GetTopRatedEmployees();
    Task<PerformanceReview> UpdateReviewStatus(int reviewId, ReviewStatus newStatus);

    Task<bool> DeletePrmByEmpId(int employeeId);
}

