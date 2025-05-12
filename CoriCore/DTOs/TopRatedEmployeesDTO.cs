using System;

namespace CoriCore.DTOs;

public class TopRatedEmployeesDTO
{
    public List<EmpUserDTO>? Employees { get; set; } = new List<EmpUserDTO>();
    
    public List<EmpUserRatingMetricsDTO>? Ratings { get; set; } = new List<EmpUserRatingMetricsDTO>();
}
