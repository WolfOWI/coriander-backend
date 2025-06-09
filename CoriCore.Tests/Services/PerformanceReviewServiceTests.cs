using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Bcpg;
using Xunit;

namespace CoriCore.Tests.Unit.Services;

public class PerformanceReviewServiceTests : IDisposable
{
    private readonly AppDbContext _context;        // Test database
    private readonly PerformanceReviewService _service;      // PerformanceReviewService (to test)

    // Constructor (runs before each test)
    public PerformanceReviewServiceTests()
    {
        // Options for our test database
        var options = new DbContextOptionsBuilder<AppDbContext>()  // Create builder for database options
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())  // Use an in-memory database with a unique name
            .Options;  // Get the final options object

        // Create temporary database in memory just for testing
        _context = new AppDbContext(options);  // Create a new database context with our options

        // Create service using test database
        _service = new PerformanceReviewService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    ///----------------------------------------------------------
    /// Test 1: GetPrmByStartDateAdminId
    /// </summary>

    [Fact]
    public async Task GetPrmByStartDateAdminId_ReturnsPerformanceReviews()
    {
        //Arrange
        var testDate = new DateTime(2024, 12, 1);

        var adminUser = new User { UserId = 1, FullName = "Admin User", Role = UserRole.Admin };
        var employeeUser = new User { UserId = 2, FullName = "Employee User", Role = UserRole.Employee };

        var admin = new Admin { AdminId = 1, UserId = adminUser.UserId, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = employeeUser.UserId, User = employeeUser };


        var review = new PerformanceReview
        {
            ReviewId = 1,
            AdminId = admin.AdminId,
            Admin = admin,
            EmployeeId = employee.EmployeeId,
            Employee = employee,
            StartDate = testDate,
            EndDate = testDate.AddDays(1),
            Rating = 5,
            Comment = "Great job!",
            DocUrl = "http://example.com/doc"
        };

        _context.Users.AddRange(adminUser, employeeUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.Add(review);

        await _context.SaveChangesAsync();

        //Act
        var result = await _service.GetPrmByStartDateAdminId(admin.AdminId, testDate);

        //Assert
        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(review.ReviewId, list[0].ReviewId);
        Assert.Equal("Admin User", list[0].Admin.User.FullName);
        Assert.Equal("Employee User", list[0].Employee.User.FullName);

    }

    [Fact]
    public async Task GetPrmByStartDateAdminId_ReturnsEmpty_WhenNoMatches()
    {
        //Arrange
        //No data in db

        // Act
        var result = await _service.GetPrmByStartDateAdminId(999, DateTime.Now);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    /// <summary>
    ///----------------------------------------------------------
    /// Test 1: GetAllUpcomingPrm
    /// </summary>

    [Fact]
    public async Task GetAllUpcomingPrm_ReturnsOnlyUpcomingStatus()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin One", Role = UserRole.Admin };
        var empUser = new User { UserId = 2, FullName = "Employee One", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

        var reviewUpcoming1 = new PerformanceReview
        {
            ReviewId = 1,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(1),
            Status = ReviewStatus.Upcoming
        };

        var reviewUpcoming2 = new PerformanceReview
        {
            ReviewId = 2,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(2),
            Status = ReviewStatus.Upcoming
        };

        var reviewCompleted = new PerformanceReview
        {
            ReviewId = 3,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(-1),
            Status = ReviewStatus.Completed
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.AddRange(reviewUpcoming1, reviewUpcoming2, reviewCompleted);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUpcomingPrm();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count()); // Only 2 upcoming reviews should be returned
        Assert.All(result, r => Assert.Equal(ReviewStatus.Upcoming, r.Status));
    }

    [Fact]
    public async Task GetAllUpcomingPrm_ReturnsEmpty_WhenNoUpcomingStatus()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser = new User { UserId = 2, FullName = "Employee", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

        var completedReview = new PerformanceReview
        {
            ReviewId = 1,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(-3),
            Status = ReviewStatus.Completed
        };

        var cancelledReview = new PerformanceReview
        {
            ReviewId = 2,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(-5),
            Status = ReviewStatus.Completed
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.AddRange(completedReview, cancelledReview);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUpcomingPrm();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    ///----------------------------------------------------------
    /// Test 1: UpdatePerformanceReview/{id}
    /// </summary>

    [Fact]
    public async Task UpdatePerformanceReview_UpdatesCorrectReview()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin One", Role = UserRole.Admin };
        var empUser = new User { UserId = 2, FullName = "Employee One", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = adminUser.UserId, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = empUser.UserId, User = empUser };

        var originalReview = new PerformanceReview
        {
            ReviewId = 1,
            AdminId = admin.AdminId,
            Admin = admin,
            EmployeeId = employee.EmployeeId,
            Employee = employee,
            StartDate = DateTime.Now.AddDays(3),
            EndDate = DateTime.Now.AddDays(4),
            MeetLocation = "Office A",
            Rating = 3,
            Comment = "Initial comment",
            Status = ReviewStatus.Upcoming
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.Add(originalReview);
        await _context.SaveChangesAsync();

        // Act
        // Modify the existing review
        originalReview.MeetLocation = "Updated Location";
        originalReview.Rating = 5;
        originalReview.Comment = "Updated comment";

        var result = await _service.UpdatePerformanceReview(originalReview.ReviewId, originalReview);

        // Assert
        var updatedReview = await _context.PerformanceReviews.FindAsync(originalReview.ReviewId);

        Assert.NotNull(result);
        Assert.Equal(originalReview.ReviewId, result.ReviewId); // ID should be the same
        Assert.Equal("Updated Location", updatedReview.MeetLocation);
        Assert.Equal(5, updatedReview.Rating);
        Assert.Equal("Updated comment", updatedReview.Comment);
    }

    [Fact]
    public async Task GetPrmByEmpId_ReturnsReviews_WhenFound()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser = new User { UserId = 2, FullName = "Employee", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

        var review = new PerformanceReview
        {
            ReviewId = 1,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now,
            Status = ReviewStatus.Upcoming
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPrmByEmpId(employee.EmployeeId);

        // Assert
        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(review.ReviewId, list[0].ReviewId);
    }

    [Fact]
    public async Task GetPrmByEmpId_ReturnsEmpty_WhenNoReviews()
    {
        // Act
        var result = await _service.GetPrmByEmpId(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreatePerformanceReview_CreatesWithUpcomingStatus()
    {
        // Arrange
        var review = new PerformanceReview
        {
            AdminId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        // Act
        var result = await _service.CreatePerformanceReview(review);

        // Assert
        Assert.Equal(ReviewStatus.Upcoming, result.Status);
        var savedReview = await _context.PerformanceReviews.FindAsync(result.ReviewId);
        Assert.NotNull(savedReview);
        Assert.Equal(ReviewStatus.Upcoming, savedReview.Status);
    }

    [Fact]
    public async Task UpdatePerformanceReview_UpdatesAllFields()
    {
        // Arrange
        var review = new PerformanceReview
        {
            AdminId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            Status = ReviewStatus.Upcoming
        };
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        var updateReview = new PerformanceReview
        {
            AdminId = 1,
            EmployeeId = 1,
            IsOnline = true,
            MeetLocation = "Room 101",
            MeetLink = "https://meet.example.com",
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(3),
            Rating = 5,
            Comment = "Great work!",
            DocUrl = "https://docs.example.com",
            Status = ReviewStatus.Completed
        };

        // Act
        var result = await _service.UpdatePerformanceReview(review.ReviewId, updateReview);

        // Assert
        Assert.Equal(updateReview.IsOnline, result.IsOnline);
        Assert.Equal(updateReview.MeetLocation, result.MeetLocation);
        Assert.Equal(updateReview.MeetLink, result.MeetLink);
        Assert.Equal(updateReview.StartDate, result.StartDate);
        Assert.Equal(updateReview.EndDate, result.EndDate);
        Assert.Equal(updateReview.Rating, result.Rating);
        Assert.Equal(updateReview.Comment, result.Comment);
        Assert.Equal(updateReview.DocUrl, result.DocUrl);
        Assert.Equal(updateReview.Status, result.Status);
    }

    [Fact]
    public async Task UpdatePerformanceReview_ThrowsException_WhenNotFound()
    {
        // Arrange
        var updateReview = new PerformanceReview
        {
            AdminId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _service.UpdatePerformanceReview(999, updateReview));
    }

    [Fact]
    public async Task DeletePerformanceReview_ReturnsFalse_WhenNotFound()
    {
        // Act
        var result = await _service.DeletePerformanceReview(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeletePerformanceReview_DeletesAndReturnsTrue_WhenFound()
    {
        // Arrange
        var review = new PerformanceReview
        {
            AdminId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeletePerformanceReview(review.ReviewId);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.PerformanceReviews.FindAsync(review.ReviewId));
    }

    [Fact]
    public async Task GetAllUpcomingPrmByAdminId_ThrowsException_WhenAdminNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _service.GetAllUpcomingPrmByAdminId(999));
    }

    [Fact]
    public async Task GetAllUpcomingPrmByAdminId_ReturnsUpcomingReviews()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser = new User { UserId = 2, FullName = "Employee", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

        var upcomingReview = new PerformanceReview
        {
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(1),
            Status = ReviewStatus.Upcoming
        };

        var completedReview = new PerformanceReview
        {
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(-1),
            Status = ReviewStatus.Completed
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.AddRange(upcomingReview, completedReview);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUpcomingPrmByAdminId(admin.AdminId);

        // Assert
        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(ReviewStatus.Upcoming, list[0].Status);
        Assert.Equal(adminUser.FullName, list[0].AdminName);
        Assert.Equal(empUser.FullName, list[0].EmployeeName);
    }

    [Fact]
    public async Task UpdateReviewStatus_UpdatesStatus_WhenFound()
    {
        // Arrange
        var review = new PerformanceReview
        {
            AdminId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            Status = ReviewStatus.Upcoming
        };
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.UpdateReviewStatus(review.ReviewId, ReviewStatus.Completed);

        // Assert
        Assert.Equal(ReviewStatus.Completed, result.Status);
        var savedReview = await _context.PerformanceReviews.FindAsync(review.ReviewId);
        Assert.NotNull(savedReview);
        Assert.Equal(ReviewStatus.Completed, savedReview.Status);
    }

    [Fact]
    public async Task UpdateReviewStatus_ThrowsException_WhenNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _service.UpdateReviewStatus(999, ReviewStatus.Completed));
    }

    [Fact]
    public async Task GetTopEmpUserRatingMetrics_ReturnsTopEmployees()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser1 = new User { UserId = 2, FullName = "Employee1", Role = UserRole.Employee };
        var empUser2 = new User { UserId = 3, FullName = "Employee2", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee1 = new Employee { EmployeeId = 1, UserId = 2, User = empUser1 };
        var employee2 = new Employee { EmployeeId = 2, UserId = 3, User = empUser2 };

        var review1 = new PerformanceReview
        {
            Admin = admin,
            Employee = employee1,
            AdminId = admin.AdminId,
            EmployeeId = employee1.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 5
        };

        var review2 = new PerformanceReview
        {
            Admin = admin,
            Employee = employee2,
            AdminId = admin.AdminId,
            EmployeeId = employee2.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 3
        };

        _context.Users.AddRange(adminUser, empUser1, empUser2);
        _context.Admins.Add(admin);
        _context.Employees.AddRange(employee1, employee2);
        _context.PerformanceReviews.AddRange(review1, review2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTopEmpUserRatingMetrics(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(employee1.EmployeeId, result[0].EmployeeId);
        Assert.Equal(5, result[0].AverageRating);
    }

    [Fact]
    public async Task GetAllEmpUserRatingMetrics_ReturnsAllEmployees()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser1 = new User { UserId = 2, FullName = "Employee1", Role = UserRole.Employee };
        var empUser2 = new User { UserId = 3, FullName = "Employee2", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee1 = new Employee { EmployeeId = 1, UserId = 2, User = empUser1 };
        var employee2 = new Employee { EmployeeId = 2, UserId = 3, User = empUser2 };

        var review1 = new PerformanceReview
        {
            Admin = admin,
            Employee = employee1,
            AdminId = admin.AdminId,
            EmployeeId = employee1.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 5
        };

        var review2 = new PerformanceReview
        {
            Admin = admin,
            Employee = employee2,
            AdminId = admin.AdminId,
            EmployeeId = employee2.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 3
        };

        _context.Users.AddRange(adminUser, empUser1, empUser2);
        _context.Admins.Add(admin);
        _context.Employees.AddRange(employee1, employee2);
        _context.PerformanceReviews.AddRange(review1, review2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllEmpUserRatingMetrics();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, m => m.EmployeeId == employee1.EmployeeId && m.AverageRating == 5);
        Assert.Contains(result, m => m.EmployeeId == employee2.EmployeeId && m.AverageRating == 3);
    }

    [Fact]
    public async Task GetEmpUserRatingMetricsByEmpId_ReturnsMetrics_WhenFound()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser = new User { UserId = 2, FullName = "Employee", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

        var review = new PerformanceReview
        {
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 5
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetEmpUserRatingMetricsByEmpId(employee.EmployeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.EmployeeId, result.EmployeeId);
        Assert.Equal(5, result.AverageRating);
    }

    [Fact]
    public async Task GetEmpUserRatingMetricsByEmpId_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _service.GetEmpUserRatingMetricsByEmpId(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopRatedEmployees_ReturnsTopEmployees()
    {
        // Arrange
        var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
        var empUser1 = new User { UserId = 2, FullName = "Employee1", Role = UserRole.Employee };
        var empUser2 = new User { UserId = 3, FullName = "Employee2", Role = UserRole.Employee };
        var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
        var employee1 = new Employee { EmployeeId = 1, UserId = 2, User = empUser1 };
        var employee2 = new Employee { EmployeeId = 2, UserId = 3, User = empUser2 };

        var review1 = new PerformanceReview
        {
            Admin = admin,
            Employee = employee1,
            AdminId = admin.AdminId,
            EmployeeId = employee1.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 5
        };

        var review2 = new PerformanceReview
        {
            Admin = admin,
            Employee = employee2,
            AdminId = admin.AdminId,
            EmployeeId = employee2.EmployeeId,
            StartDate = DateTime.UtcNow,
            Rating = 3
        };

        _context.Users.AddRange(adminUser, empUser1, empUser2);
        _context.Admins.Add(admin);
        _context.Employees.AddRange(employee1, employee2);
        _context.PerformanceReviews.AddRange(review1, review2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTopRatedEmployees();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Check first employee (highest rated)
        var firstEmployee = result[0];
        Assert.NotNull(firstEmployee.Employees);
        Assert.NotNull(firstEmployee.Ratings);
        Assert.Single(firstEmployee.Employees);
        Assert.Single(firstEmployee.Ratings);
        Assert.Equal("Employee1", firstEmployee.Employees[0].FullName);
        Assert.Equal(5, firstEmployee.Ratings[0].AverageRating);

        // Check second employee
        var secondEmployee = result[1];
        Assert.NotNull(secondEmployee.Employees);
        Assert.NotNull(secondEmployee.Ratings);
        Assert.Single(secondEmployee.Employees);
        Assert.Single(secondEmployee.Ratings);
        Assert.Equal("Employee2", secondEmployee.Employees[0].FullName);
        Assert.Equal(3, secondEmployee.Ratings[0].AverageRating);
    }
}
