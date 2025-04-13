using System;

namespace CoriCore.DTOs;

public class EmpTotalStatsDTO
{
    public int TotalEmployees { get; set; }
    public int TotalFullTimeEmployees { get; set; }
    public int TotalPartTimeEmployees { get; set; }
    public int TotalContractEmployees { get; set; }
    public int TotalInternEmployees { get; set; }
    public int TotalSuspendedEmployees { get; set; }
    
}
