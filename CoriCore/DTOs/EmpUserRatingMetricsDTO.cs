using System;

namespace CoriCore.DTOs;

public class EmpUserRatingMetricsDTO
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = String.Empty;
    public double AverageRating { get; set; }
    public int NumberOfRatings { get; set; }
    public int MostRecentRating { get; set; }
    
}
