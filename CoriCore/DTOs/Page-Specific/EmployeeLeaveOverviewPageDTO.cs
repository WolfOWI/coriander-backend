using System;

namespace CoriCore.DTOs.Page_Specific;

public class EmployeeLeaveOverviewPageDTO
{
    public IEnumerable<LeaveRequestDTO> LeaveRequests { get; set; }
    public IEnumerable<LeaveBalanceDTO> LeaveBalances { get; set; }
}
