using System;

namespace CoriCore.DTOs.Page_Specific;

public class AdminDashboardPageDTO
{
    public AdminUserDTO? AdminUser { get; set; }
    public List<EmpUserRatingMetricsDTO>? EmpUserRatingMetrics { get; set; }
    public List<TopRatedEmployeesDTO>? TopRatedEmployees { get; set; }
    public List<LeaveRequestDTO>? LeaveRequests { get; set; }
    public EmpTotalStatsDTO? EmployeeStatusTotals { get; set; }

}
