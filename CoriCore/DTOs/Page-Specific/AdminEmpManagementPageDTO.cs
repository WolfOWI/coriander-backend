// Employee Management Page (Admin) DTO
// ========================================
// Wolf Botha

using System;
using CoriCore.DTOs;

namespace CoriCore.DTOs.Page_Specific;

public class AdminEmpManagePageListItemDTO
{
    public EmpUserDTO? EmpUser { get; set; }

    public EmpUserRatingMetricsDTO? EmpUserRatingMetrics { get; set; }

    public LeaveBalanceSumDTO? TotalLeaveBalanceSum { get; set; }
}
