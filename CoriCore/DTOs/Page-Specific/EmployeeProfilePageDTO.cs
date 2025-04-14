// Employee Profile Page (Employee) DTO
// ========================================
// Wolf Botha

using System;

namespace CoriCore.DTOs.Page_Specific;

public class EmployeeProfilePageDTO
{
    public EmpUserDTO? EmpUser { get; set; }
    public EmpUserRatingMetricsDTO? EmpUserRatingMetrics { get; set; }
    public List<EquipmentDTO>? Equipment { get; set; }

}
