using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Tests.Unit.Services;

public class MeetingServiceTests
{
    private readonly AppDbContext _context;
    private readonly MeetingService _service;

    public MeetingServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _service = new MeetingService(_context);
    }

    [Fact]
    public async Task GetMeetingsByEmployeeIdAndStatus_ReturnsEmptyList_WhenNoMeetingsExist()
    {
        // Arrange
        var employeeId = 1;
        var status = MeetStatus.Upcoming;

        // Act
        var result = await _service.GetMeetingsByEmployeeIdAndStatus(employeeId, status);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMeetingsByEmployeeIdAndStatus_ReturnsMeetingsForCorrectEmployeeAndStatus()
    {
        // Arrange
        var adminUser = new User
        {
            UserId = 1,
            FullName = "Admin User",
            Email = "admin@example.com",
            Role = UserRole.Admin
        };

        var empUser = new User
        {
            UserId = 2,
            FullName = "Employee User",
            Email = "employee@example.com",
            Role = UserRole.Employee
        };

        var admin = new Admin
        {
            AdminId = 1,
            UserId = 1,
            User = adminUser
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = 2,
            User = empUser,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        var meeting = new Meeting
        {
            MeetingId = 1,
            AdminId = 1,
            EmployeeId = 1,
            Admin = admin,
            Employee = employee,
            IsOnline = true,
            MeetLink = "https://meet.google.com/test",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(1).AddHours(1),
            Purpose = "Performance Review",
            RequestedAt = DateTime.Now,
            Status = MeetStatus.Upcoming
        };

        _context.Users.AddRange(adminUser, empUser);
        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetMeetingsByEmployeeIdAndStatus(1, MeetStatus.Upcoming);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var meetingDto = result.First();
        Assert.Equal(meeting.MeetingId, meetingDto.MeetingId);
        Assert.Equal(admin.User.FullName, meetingDto.AdminName);
        Assert.Equal(employee.User.FullName, meetingDto.EmployeeName);
    }

    [Fact]
    public async Task CreateMeetingRequest_ThrowsException_WhenAdminNotFound()
    {
        // Arrange
        var dto = new MeetingRequestCreateDTO
        {
            AdminId = 999,
            EmployeeId = 1,
            Purpose = "Test Meeting"
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateMeetingRequest(dto));
    }

    [Fact]
    public async Task CreateMeetingRequest_ThrowsException_WhenEmployeeNotFound()
    {
        // Arrange
        var admin = new Admin { AdminId = 1, UserId = 1 };
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        var dto = new MeetingRequestCreateDTO
        {
            AdminId = 1,
            EmployeeId = 999,
            Purpose = "Test Meeting"
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateMeetingRequest(dto));
    }

    [Fact]
    public async Task CreateMeetingRequest_CreatesSuccessfully_WhenValidData()
    {
        // Arrange
        var admin = new Admin { AdminId = 1, UserId = 1 };
        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = 2,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        _context.Admins.Add(admin);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var dto = new MeetingRequestCreateDTO
        {
            AdminId = 1,
            EmployeeId = 1,
            Purpose = "Test Meeting"
        };

        // Act
        var result = await _service.CreateMeetingRequest(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.AdminId, result.AdminId);
        Assert.Equal(dto.EmployeeId, result.EmployeeId);
        Assert.Equal(dto.Purpose, result.Purpose);

        var meetingInDb = await _context.Meetings.FirstAsync();
        Assert.Equal(MeetStatus.Requested, meetingInDb.Status);
    }

    [Fact]
    public async Task ConfirmAndUpdateMeetingRequest_ReturnsCode404_WhenMeetingNotFound()
    {
        // Arrange
        var dto = new MeetingUpdateDTO
        {
            IsOnline = true,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(1).AddHours(1)
        };

        // Act
        var result = await _service.ConfirmAndUpdateMeetingRequest(999, dto);

        // Assert
        Assert.Equal(404, result.Code);
        Assert.Equal("Meeting not found", result.Message);
    }

    [Fact]
    public async Task ConfirmAndUpdateMeetingRequest_UpdatesSuccessfully_WhenMeetingExists()
    {
        // Arrange
        var meeting = new Meeting
        {
            MeetingId = 1,
            AdminId = 1,
            EmployeeId = 1,
            Purpose = "Test Meeting",
            Status = MeetStatus.Requested,
            RequestedAt = DateTime.Now
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        var dto = new MeetingUpdateDTO
        {
            IsOnline = true,
            MeetLink = "https://meet.google.com/test",
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(1).AddHours(1)
        };

        // Act
        var result = await _service.ConfirmAndUpdateMeetingRequest(1, dto);

        // Assert
        Assert.Equal(200, result.Code);
        Assert.Equal("Meeting request updated successfully", result.Message);

        var updatedMeeting = await _context.Meetings.FindAsync(1);
        Assert.Equal(MeetStatus.Upcoming, updatedMeeting.Status);
        Assert.True(updatedMeeting.IsOnline);
        Assert.Equal(dto.MeetLink, updatedMeeting.MeetLink);
    }

    [Fact]
    public async Task MarkMeetingAsCompleted_ReturnsCode404_WhenMeetingNotFound()
    {
        // Arrange & Act
        var result = await _service.MarkMeetingAsCompleted(999);

        // Assert
        Assert.Equal(404, result.Code);
    }

    [Fact]
    public async Task MarkMeetingAsCompleted_UpdatesStatusSuccessfully()
    {
        // Arrange
        var meeting = new Meeting
        {
            MeetingId = 1,
            AdminId = 1,
            EmployeeId = 1,
            Purpose = "Test Meeting",
            Status = MeetStatus.Upcoming,
            RequestedAt = DateTime.Now
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.MarkMeetingAsCompleted(1);

        // Assert
        Assert.Equal(200, result.Code);

        var updatedMeeting = await _context.Meetings.FindAsync(1);
        Assert.Equal(MeetStatus.Completed, updatedMeeting.Status);
    }
}