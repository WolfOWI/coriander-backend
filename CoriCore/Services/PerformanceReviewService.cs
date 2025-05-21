// Performance Review Service
// ========================================

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

    /// <inheritdoc/>
    public async Task<List<EmpUserRatingMetricsDTO>> GetTopEmpUserRatingMetrics(int count)
{
    // Get all employee rating metrics
    var allEmployeeMetrics = await GetAllEmpUserRatingMetrics();

    // Order by average rating descending, then by number of ratings descending
    var topEmployees = allEmployeeMetrics
        .OrderByDescending(emp => emp.AverageRating)
        .ThenByDescending(emp => emp.NumberOfRatings)
        .Take(count) // Take up to 'count' employees (e.g., 5)
        .ToList();

    return topEmployees;
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

    // ========================================
    // GET ALL UPCOMING PERFORMANCE REVIEWS THAT THE ADMIN IS INVOLVED IN (BY ADMIN ID)
    // ========================================
    public async Task<IEnumerable<PerformanceReviewDTO>> GetAllUpcomingPrmByAdminId(int adminId)
    {
        // Check if the admin exists
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
        {
            throw new Exception("Admin not found");
        }

        // Get all upcoming performance reviews that the admin is involved in
        var upcomingReviews = await _context.PerformanceReviews
            .Include(pr => pr.Admin)
                .ThenInclude(a => a.User)
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User)
            .Where(pr => pr.AdminId == adminId && pr.Status == ReviewStatus.Upcoming)
            .ToListAsync();

        // Convert to DTOs
        return upcomingReviews.Select(pr => new PerformanceReviewDTO
        {
            ReviewId = pr.ReviewId,
            AdminId = pr.AdminId,
            AdminName = pr.Admin.User.FullName,
            EmployeeId = pr.EmployeeId,
            EmployeeName = pr.Employee.User.FullName,
            IsOnline = pr.IsOnline,
            MeetLocation = pr.MeetLocation,
            MeetLink = pr.MeetLink,
            StartDate = pr.StartDate,
            EndDate = pr.EndDate,
            Rating = pr.Rating,
            Comment = pr.Comment,
            DocUrl = pr.DocUrl,
            Status = pr.Status
        });
    }

    // ========================================
    // GET TOP RATED EMPLOYEES
    // ========================================
    /// <summary>
    /// Get the Top rated employess
    /// </summary>
    public async Task<List<TopRatedEmployeesDTO>> GetTopRatedEmployees()
    {
        var reviews = await _context.PerformanceReviews
            .Include(pr => pr.Employee)
                .ThenInclude(e => e.User)
            .Where(pr => pr.Rating != null)
            .ToListAsync();

        if (!reviews.Any())
            return new List<TopRatedEmployeesDTO>();

        var groupedRatings = reviews
            .GroupBy(pr => pr.EmployeeId)
            .Select(group =>
            {
                var firstReview = group.First();
                var emp = firstReview.Employee;
                var user = emp.User;

                return new TopRatedEmployeesDTO
                {
                    Employees = new List<EmpUserDTO>
                    {
                        new EmpUserDTO
                        {
                            UserId = user.UserId,
                            FullName = user.FullName,
                            Email = user.Email,
                            GoogleId = user.GoogleId,
                            ProfilePicture = user.ProfilePicture,
                            Role = user.Role,
                            EmployeeId = emp.EmployeeId,
                            Gender = emp.Gender,
                            DateOfBirth = emp.DateOfBirth,
                            PhoneNumber = emp.PhoneNumber,
                            JobTitle = emp.JobTitle,
                            Department = emp.Department,
                            SalaryAmount = emp.SalaryAmount,
                            PayCycle = emp.PayCycle,
                            LastPaidDate = emp.LastPaidDate,
                            EmployType = emp.EmployType,
                            EmployDate = emp.EmployDate,
                            IsSuspended = emp.IsSuspended
                        }
                    },
                    Ratings = new List<EmpUserRatingMetricsDTO>
                    {
                        new EmpUserRatingMetricsDTO
                        {
                            EmployeeId = emp.EmployeeId,
                            FullName = user.FullName,
                            AverageRating = Math.Round(group.Average(pr => pr.Rating!.Value), 2),
                            NumberOfRatings = group.Count(),
                        }
                    }
                };
            })
            .OrderByDescending(e => e.Ratings!.First().AverageRating)
            .ThenByDescending(e => e.Ratings!.First().NumberOfRatings)
            .Take(3)
            .ToList();

        return groupedRatings;
    }


    //Toggle status from 1 to 2 (Completed)
    public async Task<PerformanceReview> UpdateReviewStatus(int reviewId, ReviewStatus newStatus)
    {
        var review = await _context.PerformanceReviews.FindAsync(reviewId);

        if (review == null)
        {
            throw new Exception("Performance review not found");
        }

        review.Status = newStatus;
        await _context.SaveChangesAsync();

        return review;
    }
}