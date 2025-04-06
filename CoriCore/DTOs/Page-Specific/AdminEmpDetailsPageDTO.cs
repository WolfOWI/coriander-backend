using System;
using CoriCore.DTOs;

namespace CoriCore.DTOs.Page_Specific;

public class AdminEmpDetailsPageDTO
{

    public EmpUserDTO? EmpUser { get; set; }
    public List<EquipmentDTO>? Equipment { get; set; }
    public List<LeaveBalanceDTO>? LeaveBalances { get; set; }
    public EmpUserRatingMetricsDTO? EmpUserRatingMetrics { get; set; }
    public List<PerformanceReviewDTO>? PerformanceReviews { get; set; }

}
