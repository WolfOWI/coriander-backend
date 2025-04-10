using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

//Provides the methods to perform CRUD operations on Performance Reviews
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

    // ========================================
    // GET PERFORMANCE REVIEW BY EMP ID
    // ========================================
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

    // ========================================
    // CREATE PRM 
    // SET STATUS TO 1 (UPCOMING)
    // USE PERFORMANCE REVIEW DTO TO CREATE A NEW REVIEW
    // ========================================
    public async Task<PerformanceReview> CreatePerformanceReview(PerformanceReview review)
    {
        review.Status = ReviewStatus.Upcoming; // Set initial status
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    // ========================================
    // UPDATE PERFORMANCE REVIEW
    // ========================================
    public async Task<PerformanceReview> UpdatePerformanceReview(int id, PerformanceReview review)
    {
        var existingReview = await _context.PerformanceReviews.FindAsync(id);
        if (existingReview == null)
        {
            throw new Exception("Performance review not found");
        }

        // Update properties
        existingReview.AdminId = review.AdminId;
        existingReview.EmployeeId = review.EmployeeId;
        existingReview.IsOnline = review.IsOnline;
        existingReview.MeetLocation = review.MeetLocation;
        existingReview.MeetLink = review.MeetLink;
        existingReview.StartDate = review.StartDate;
        existingReview.EndDate = review.EndDate;
        existingReview.Rating = review.Rating;
        existingReview.Comment = review.Comment;
        existingReview.DocUrl = review.DocUrl;
        existingReview.Status = review.Status;

        await _context.SaveChangesAsync();
        return existingReview;
    }

    // ========================================
    // DELETE PERFORMANCE REVIEW
    // ========================================
    public Task<bool> DeletePerformanceReview(int id)
    {
        var review = _context.PerformanceReviews.Find(id);
        if (review == null)
        {
            return Task.FromResult(false); // Review not found
        }

        _context.PerformanceReviews.Remove(review);
        _context.SaveChangesAsync();
        return Task.FromResult(true); // Review deleted successfully
    }

    // ========================================
    // GET ALL PERFORMANCE REVIEWS WITH STATUS 1 (Upcoming)
    // ========================================
    public async Task<IEnumerable<PerformanceReview>> GetAllUpcomingPrm()
    {
        return await _context.PerformanceReviews
            .Include(pr => pr.Admin) 
                .ThenInclude(a => a.User) // Include related Admin User data
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User) // Include related Employee User data
            .Where(pr => pr.Status == ReviewStatus.Upcoming) // Filter by status 'Upcoming'
            .ToListAsync();
    }

/// <summary>
/// support method to delete performance review by employee id
/// </summary>
    public Task<bool> DeletePrmByEmpId(int employeeId)
    {
        var reviews = _context.PerformanceReviews.Where(r => r.EmployeeId == employeeId).ToList();
        if (reviews.Count == 0)
        {
            return Task.FromResult(false); // No reviews found for the employee
        }

        _context.PerformanceReviews.RemoveRange(reviews);
        _context.SaveChangesAsync();
        return Task.FromResult(true); // Reviews deleted successfully
    }
}