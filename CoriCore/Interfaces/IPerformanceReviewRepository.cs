using System;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IPerformanceReviewRepository
{
    Task<IEnumerable<PerformanceReview>> GetPrmByStartDateAdminId(int adminId, DateTime startDate);

    Task<IEnumerable<PerformanceReview>> GetPrmByEmpId(int employeeId);
}

