using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoriCore.Tests.Unit.Services;

public class GatheringServiceTests
{
    private readonly AppDbContext _context;
    private readonly GatheringService _service;
    private readonly Mock<IMeetingService> _mockMeetingService;
    private readonly Mock<IPerformanceReviewService> _mockPerformanceReviewService;

    public GatheringServiceTests()
    {
        // In-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);

        _mockMeetingService = new Mock<IMeetingService>();
        _mockPerformanceReviewService = new Mock<IPerformanceReviewService>();

        _service = new GatheringService(
            _context,
            _mockMeetingService.Object,
            _mockPerformanceReviewService.Object
        );
    }

    [Fact]
    public async Task GetAllGatheringsByEmployeeId_ReturnsEmptyList_WhenNoGatheringsExist()
    {
        // Arrange
        var employeeId = 1;
        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
            .ReturnsAsync(new List<MeetingDTO>());
        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
            .ReturnsAsync(new List<MeetingDTO>());

        // Act
        var result = await _service.GetAllGatheringsByEmployeeId(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllGatheringsByEmployeeId_ReturnsCombinedGatherings()
    {
        // Arrange
        var employeeId = 1;

        // Create admin and employee entities for both meeting and performance review
        var adminUser = new User
        {
            UserId = 2,
            FullName = "Admin User",
            Email = "admin@example.com",
            Role = UserRole.Admin
        };

        var empUser = new User
        {
            UserId = employeeId,
            FullName = "Employee User",
            Email = "employee@example.com",
            Role = UserRole.Employee
        };

        var admin = new Admin
        {
            AdminId = 2,
            UserId = 2,
            User = adminUser
        };

        var employee = new Employee
        {
            EmployeeId = employeeId,
            UserId = employeeId,
            User = empUser,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Lecturer",
            Department = "DV300",
            SalaryAmount = 5550000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        var meeting = new MeetingDTO
        {
            MeetingId = 1,
            AdminId = admin.AdminId,
            AdminName = adminUser.FullName,
            EmployeeId = employee.EmployeeId,
            EmployeeName = empUser.FullName,
            IsOnline = true,
            MeetLink = "google.com",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(1).AddHours(1),
            Purpose = "Test Meeting",
            RequestedAt = DateTime.Now,
            Status = MeetStatus.Upcoming
        };

        var review = new PerformanceReview
        {
            ReviewId = 1,
            AdminId = 2,
            EmployeeId = employeeId,
            Admin = admin,
            Employee = employee,
            IsOnline = false,
            MeetLocation = "Centurion",
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(2).AddHours(1),
            Rating = 4,
            Comment = "Good performance",
            Status = ReviewStatus.Upcoming
        };

        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
            .ReturnsAsync(new List<MeetingDTO> { meeting });
        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
            .ReturnsAsync(new List<MeetingDTO>());

        // Add all entities to context
        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.PerformanceReviews.Add(review);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllGatheringsByEmployeeId(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, g => g.Type == GatheringType.Meeting);
        Assert.Contains(result, g => g.Type == GatheringType.PerformanceReview);
    }

    [Fact]
    public async Task GetAllGatheringsByAdminId_ReturnsEmptyList_WhenNoGatheringsExist()
    {
        // Arrange
        var adminId = 1;
        _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming))
            .ReturnsAsync(new List<MeetingDTO>());
        _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Completed))
            .ReturnsAsync(new List<MeetingDTO>());

        // Act
        var result = await _service.GetAllGatheringsByAdminId(adminId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUpcomingAndCompletedGatheringsByAdminIdAndMonth_ThrowsException_ForInvalidMonth()
    {
        // Arrange
        var adminId = 1;
        var invalidMonth = "13";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, invalidMonth));
    }

    [Fact]
    public async Task GetAllGatheringsByEmployeeIdAndStatus_ReturnsOnlyUpcomingGatherings()
    {
        // Arrange
        var employeeId = 1;
        var upcomingMeeting = new MeetingDTO
        {
            MeetingId = 1,
            AdminId = 2,
            EmployeeId = employeeId,
            StartDate = DateTime.Now.AddDays(1),
            Status = MeetStatus.Upcoming
        };

        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
            .ReturnsAsync(new List<MeetingDTO> { upcomingMeeting });
        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
            .ReturnsAsync(new List<MeetingDTO>());

        // Act
        var result = await _service.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Upcoming");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(MeetStatus.Upcoming, result.First().MeetingStatus);
    }
}