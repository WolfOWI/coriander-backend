using System;

namespace CoriCore.DTOs;

public class LeaveBalanceDTO
{
    // LeaveBalance
    public int LeaveBalanceId { get; set; }
    public int RemainingDays { get; set; }

    // LeaveType
    public string? LeaveTypeName { get; set; }
    public string? Description { get; set; }
    public int DefaultDays { get; set; }

}
