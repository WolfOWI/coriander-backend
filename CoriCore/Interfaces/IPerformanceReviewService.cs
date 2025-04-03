using System;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IPerformanceReviewService
{
    Task<IEnumerable<PerformanceReview>> GetPrmByStartDateAdminId(int adminId, DateTime startDate);

    Task<IEnumerable<PerformanceReview>> GetPrmByEmpId(int employeeId);
}

