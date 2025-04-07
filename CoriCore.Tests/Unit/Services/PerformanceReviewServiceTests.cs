using System;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Bcpg;
using Xunit;

namespace CoriCore.Tests.Unit.Services;

public class PerformanceReviewServiceTests
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
            Status = Status.Upcoming
        };

        var reviewUpcoming2 = new PerformanceReview
        {
            ReviewId = 2,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(2),
            Status = Status.Upcoming
        };

        var reviewCompleted = new PerformanceReview
        {
            ReviewId = 3,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(-1),
            Status = Status.Completed
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
        Assert.All(result, r => Assert.Equal(Status.Upcoming, r.Status));
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
            Status = Status.Completed
        };

        var cancelledReview = new PerformanceReview
        {
            ReviewId = 2,
            Admin = admin,
            Employee = employee,
            AdminId = admin.AdminId,
            EmployeeId = employee.EmployeeId,
            StartDate = DateTime.Now.AddDays(-5),
            Status = Status.Completed
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
            Status = Status.Upcoming
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

    


}
