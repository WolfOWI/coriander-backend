using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class PerformanceReviewService : IPerformanceReviewService
{
    // ========================================
    // DEPENDENCY INJECTION
    // ========================================
    private readonly AppDbContext _context;

    public PerformanceReviewService(AppDbContext context)
    {
        _context = context;
    }

    // ========================================
    // GET PERFORMANCE REVIEW BY START DATE AND ADMIN ID
    // ========================================
    public async Task<IEnumerable<PerformanceReview>> GetPrmByStartDateAdminId(int adminId, DateTime startDate)
    {
        return await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User) // Get Admin's Name
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User) // Get Employee's Name
            .Where(pr => pr.AdminId == adminId && pr.StartDate.Date == startDate.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<PerformanceReview>> GetPrmByEmpId(int employeeId)
    {
        return await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User) // Include Admin's Name
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User) // Include Employee's Name
            .Where(pr => pr.EmployeeId == employeeId)
            .ToListAsync();
    }

    // Returns rating metrics for all employees (with atleast 1 rating)
    // EmployeeId, FullName, AverageRating, NumberOfRatings, MostRecentRating
    public async Task<List<EmpUserRatingMetricsDTO>> GetAllEmpUserRatingMetrics()
    {
        // Fetch all the data
        var reviews = await _context.PerformanceReviews
            .Include(pr => pr.Employee) // include the employee
            .ThenInclude(e => e.User) // include the user
            .Where(pr => pr.Rating != null) // filter to only include reviews that have a rating
            .ToListAsync();

        // If no reviews are found
        if (reviews.Count == 0)
        {
            return new List<EmpUserRatingMetricsDTO>(); // Return empty list
        }

            // Calculations
        var results = reviews
            // Group the reviews by employee (using both EmployeeId and FullName)
            .GroupBy(pr => new { pr.EmployeeId, pr.Employee.User.FullName })
            // For empUser, create new DTO
            .Select(group => new EmpUserRatingMetricsDTO
            {
                EmployeeId = group.Key.EmployeeId,
                FullName = group.Key.FullName,

                // Average rating
                AverageRating = Math.Round(group.Where(pr => pr.Rating.HasValue)  // Only include reviews with ratings
                .Average(pr => pr.Rating!.Value), 2), // Calculate avg (rounded to 2)

                // Number of ratings
                NumberOfRatings = group.Count(),

                // Most recent rating
                MostRecentRating = group
                    .Where(pr => pr.Rating.HasValue)
                    .OrderByDescending(pr => pr.StartDate)
                    .First()
                    .Rating!.Value // Won't be null because of the filter above
            })
            .ToList();

        return results;
    }

    public async Task<EmpUserRatingMetricsDTO?> GetEmpUserRatingMetricsByEmpId(int employeeId)
    {
        // Fetch reviews for the specific employee
        var reviews = await _context.PerformanceReviews
            .Include(pr => pr.Employee)
            .ThenInclude(e => e.User)
            .Where(pr => pr.EmployeeId == employeeId && pr.Rating != null)
            .ToListAsync();

        // If no reviews are found
        if (reviews.Count == 0)
        {
            return null;
        }

        // Get the employee's name
        var employeeName = reviews.First().Employee.User.FullName;

        // Calculate metrics
        var metrics = new EmpUserRatingMetricsDTO
        {
            EmployeeId = employeeId,
            FullName = employeeName,
            AverageRating = Math.Round(reviews.Average(pr => pr.Rating!.Value), 2),
            NumberOfRatings = reviews.Count,
            MostRecentRating = reviews
                .OrderByDescending(pr => pr.StartDate)
                .First()
                .Rating!.Value
        };

        return metrics;
    }
}