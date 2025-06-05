using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoriCore.Tests.Unit.Services
{

    public class MeetingServiceTests
    {
        private readonly MeetingService _meetingService;
        private readonly AppDbContext _context;

        public MeetingServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);
            _meetingService = new MeetingService(_context);
        }

        #region CreateMeetingRequest Tests

        [Fact]
        public async Task CreateMeetingRequest_CreatesSuccessfully_WhenValidData()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            var employee = CreateTestEmployee(1, "Employee User");

            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var meetingRequest = new MeetingRequestCreateDTO
            {
                AdminId = admin.AdminId,
                EmployeeId = employee.EmployeeId,
                Purpose = "Weekly one-on-one meeting"
            };

            // Act
            var result = await _meetingService.CreateMeetingRequest(meetingRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(meetingRequest.Purpose, result.Purpose);

            // Verify meeting was saved to database
            var savedMeeting = await _context.Meetings.FirstOrDefaultAsync();
            Assert.NotNull(savedMeeting);
            Assert.Equal(admin.AdminId, savedMeeting.AdminId);
            Assert.Equal(employee.EmployeeId, savedMeeting.EmployeeId);
            Assert.Equal(MeetStatus.Requested, savedMeeting.Status);
            Assert.True(savedMeeting.RequestedAt <= DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateMeetingRequest_ThrowsException_WhenAdminNotFound()
        {
            // Arrange
            var employee = CreateTestEmployee(1, "Employee User");
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var meetingRequest = new MeetingRequestCreateDTO
            {
                AdminId = 999, // Non-existent admin
                EmployeeId = employee.EmployeeId,
                Purpose = "Test meeting"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _meetingService.CreateMeetingRequest(meetingRequest));
            Assert.Equal("Admin not found", exception.Message);
        }

        [Fact]
        public async Task CreateMeetingRequest_ThrowsException_WhenEmployeeNotFound()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            var meetingRequest = new MeetingRequestCreateDTO
            {
                AdminId = admin.AdminId,
                EmployeeId = 999, // Non-existent employee
                Purpose = "Test meeting"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _meetingService.CreateMeetingRequest(meetingRequest));
            Assert.Equal("Employee not found", exception.Message);
        }

        #endregion

        #region ConfirmAndUpdateMeetingRequest Tests

        [Fact]
        public async Task ConfirmAndUpdateMeetingRequest_UpdatesSuccessfully_WhenMeetingExists()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Requested);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingUpdateDTO
            {
                IsOnline = true,
                MeetLocation = "Conference Room A",
                MeetLink = "https://meet.google.com/test",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(1)
            };

            // Act
            var result = await _meetingService.ConfirmAndUpdateMeetingRequest(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting request updated successfully", result.Message);

            // Verify updates were applied
            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.NotNull(updatedMeeting);
            Assert.Equal(updateDto.IsOnline, updatedMeeting.IsOnline);
            Assert.Equal(updateDto.MeetLocation, updatedMeeting.MeetLocation);
            Assert.Equal(updateDto.MeetLink, updatedMeeting.MeetLink);
            Assert.Equal(MeetStatus.Upcoming, updatedMeeting.Status); // Should be set to Upcoming
        }

        [Fact]
        public async Task ConfirmAndUpdateMeetingRequest_ReturnsNotFound_WhenMeetingDoesNotExist()
        {
            // Arrange
            var updateDto = new MeetingUpdateDTO
            {
                IsOnline = true,
                MeetLocation = "Conference Room A"
            };

            // Act
            var result = await _meetingService.ConfirmAndUpdateMeetingRequest(999, updateDto);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Equal("Meeting not found", result.Message);
        }

        [Fact]
        public async Task ConfirmAndUpdateMeetingRequest_HandlesNullFields_Gracefully()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Requested);
            meeting.MeetLocation = "Original Location";
            meeting.MeetLink = "Original Link";
            meeting.StartDate = null;
            meeting.EndDate = null;

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingUpdateDTO
            {
                IsOnline = false,
                MeetLocation = null, // Should not update
                MeetLink = null,     // Should not update
                StartDate = null,    // Should not update
                EndDate = null       // Should not update
            };

            // Act
            var result = await _meetingService.ConfirmAndUpdateMeetingRequest(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(200, result.Code);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal("Original Location", updatedMeeting.MeetLocation); // Should remain unchanged
            Assert.Equal("Original Link", updatedMeeting.MeetLink); // Should remain unchanged
            Assert.Null(updatedMeeting.StartDate); // Should remain null
            Assert.Null(updatedMeeting.EndDate); // Should remain null
        }

        #endregion

        #region UpdateMeeting Tests

        [Fact]
        public async Task UpdateMeeting_UpdatesAllFields_WhenMeetingExists()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Upcoming);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingUpdateDTO
            {
                IsOnline = false,
                MeetLocation = "New Conference Room",
                MeetLink = "https://teams.microsoft.com/new",
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(2).AddHours(2),
                Status = MeetStatus.Completed
            };

            // Act
            var result = await _meetingService.UpdateMeeting(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting request updated successfully", result.Message);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(updateDto.IsOnline, updatedMeeting.IsOnline);
            Assert.Equal(updateDto.MeetLocation, updatedMeeting.MeetLocation);
            Assert.Equal(updateDto.MeetLink, updatedMeeting.MeetLink);
            Assert.Equal(updateDto.StartDate, updatedMeeting.StartDate);
            Assert.Equal(updateDto.EndDate, updatedMeeting.EndDate);
            Assert.Equal(MeetStatus.Completed, updatedMeeting.Status);
        }

        [Fact]
        public async Task UpdateMeeting_PreservesStatus_WhenStatusNotProvided()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Upcoming);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingUpdateDTO
            {
                IsOnline = true,
                Status = null // Don't change status
            };

            // Act
            var result = await _meetingService.UpdateMeeting(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(200, result.Code);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(MeetStatus.Upcoming, updatedMeeting.Status); // Should remain unchanged
        }

        #endregion

        #region UpdateMeetingRequest Tests

        [Fact]
        public async Task UpdateMeetingRequest_UpdatesSuccessfully_WhenStatusIsRequested()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Requested);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingRequestUpdateDTO
            {
                AdminId = 2,
                Purpose = "Updated purpose for the meeting"
            };

            // Act
            var result = await _meetingService.UpdateMeetingRequest(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting request updated successfully", result.Message);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(2, updatedMeeting.AdminId);
            Assert.Equal("Updated purpose for the meeting", updatedMeeting.Purpose);
        }

        [Fact]
        public async Task UpdateMeetingRequest_ReturnsError_WhenStatusIsNotRequested()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Upcoming); // Not requested status
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingRequestUpdateDTO
            {
                Purpose = "This should not work"
            };

            // Act
            var result = await _meetingService.UpdateMeetingRequest(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Meeting request is not in requested status", result.Message);
        }

        [Fact]
        public async Task UpdateMeetingRequest_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Requested);
            meeting.Purpose = "Original Purpose";
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var updateDto = new MeetingRequestUpdateDTO
            {
                AdminId = null, // Don't update
                Purpose = "New Purpose" // Update this
            };

            // Act
            var result = await _meetingService.UpdateMeetingRequest(meeting.MeetingId, updateDto);

            // Assert
            Assert.Equal(200, result.Code);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(1, updatedMeeting.AdminId); // Should remain unchanged
            Assert.Equal("New Purpose", updatedMeeting.Purpose); // Should be updated
        }

        #endregion

        #region Status Change Tests

        [Fact]
        public async Task RejectMeetingRequest_UpdatesStatus_WhenMeetingExists()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Requested);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Act
            var result = await _meetingService.RejectMeetingRequest(meeting.MeetingId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting request rejected successfully", result.Message);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(MeetStatus.Rejected, updatedMeeting.Status);
        }

        [Fact]
        public async Task MarkMeetingAsCompleted_UpdatesStatus_WhenMeetingExists()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Upcoming);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Act
            var result = await _meetingService.MarkMeetingAsCompleted(meeting.MeetingId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting marked as completed successfully", result.Message);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(MeetStatus.Completed, updatedMeeting.Status);
        }

        [Fact]
        public async Task MarkMeetingAsUpcoming_UpdatesStatus_WhenMeetingExists()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Completed);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Act
            var result = await _meetingService.MarkMeetingAsUpcoming(meeting.MeetingId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting marked as upcoming successfully", result.Message);

            var updatedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Equal(MeetStatus.Upcoming, updatedMeeting.Status);
        }

        [Fact]
        public async Task StatusChangeMethods_ReturnNotFound_WhenMeetingDoesNotExist()
        {
            // Arrange
            int nonExistentMeetingId = 999;

            // Act & Assert - Test all status change methods
            var rejectResult = await _meetingService.RejectMeetingRequest(nonExistentMeetingId);
            Assert.Equal(404, rejectResult.Code);
            Assert.Equal("Meeting not found", rejectResult.Message);

            var completeResult = await _meetingService.MarkMeetingAsCompleted(nonExistentMeetingId);
            Assert.Equal(404, completeResult.Code);
            Assert.Equal("Meeting not found", completeResult.Message);

            var upcomingResult = await _meetingService.MarkMeetingAsUpcoming(nonExistentMeetingId);
            Assert.Equal(404, upcomingResult.Code);
            Assert.Equal("Meeting not found", upcomingResult.Message);
        }

        #endregion

        #region DeleteMeeting Tests

        [Fact]
        public async Task DeleteMeeting_RemovesMeeting_WhenMeetingExists()
        {
            // Arrange
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Requested);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Act
            var result = await _meetingService.DeleteMeeting(meeting.MeetingId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Meeting deleted successfully", result.Message);

            // Verify meeting was deleted
            var deletedMeeting = await _context.Meetings.FindAsync(meeting.MeetingId);
            Assert.Null(deletedMeeting);
        }

        [Fact]
        public async Task DeleteMeeting_ReturnsNotFound_WhenMeetingDoesNotExist()
        {
            // Arrange & Act
            var result = await _meetingService.DeleteMeeting(999);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Equal("Meeting not found", result.Message);
        }

        #endregion

        #region GetMeetingsByEmployeeIdAndStatus Tests

        [Fact]
        public async Task GetMeetingsByEmployeeIdAndStatus_ReturnsCorrectMeetings_WhenDataExists()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            var employee = CreateTestEmployee(1, "Employee User");

            var upcomingMeeting = CreateTestMeeting(1, 1, MeetStatus.Upcoming, admin, employee);
            var completedMeeting = CreateTestMeeting(2, 1, MeetStatus.Completed, admin, employee);
            var requestedMeeting = CreateTestMeeting(3, 1, MeetStatus.Requested, admin, employee);

            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.Meetings.AddRange(upcomingMeeting, completedMeeting, requestedMeeting);
            await _context.SaveChangesAsync();

            // Act
            var upcomingResults = await _meetingService.GetMeetingsByEmployeeIdAndStatus(1, MeetStatus.Upcoming);
            var completedResults = await _meetingService.GetMeetingsByEmployeeIdAndStatus(1, MeetStatus.Completed);

            // Assert
            var upcomingList = upcomingResults.ToList();
            Assert.Single(upcomingList);
            Assert.Equal(MeetStatus.Upcoming, upcomingList[0].Status);
            Assert.Equal("Admin User", upcomingList[0].AdminName);
            Assert.Equal("Employee User", upcomingList[0].EmployeeName);

            var completedList = completedResults.ToList();
            Assert.Single(completedList);
            Assert.Equal(MeetStatus.Completed, completedList[0].Status);
        }

        [Fact]
        public async Task GetMeetingsByEmployeeIdAndStatus_ReturnsEmpty_WhenNoMatchingMeetings()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            var employee = CreateTestEmployee(1, "Employee User");
            var meeting = CreateTestMeeting(1, 1, MeetStatus.Completed, admin, employee);

            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Act - Look for upcoming meetings when only completed exists
            var results = await _meetingService.GetMeetingsByEmployeeIdAndStatus(1, MeetStatus.Upcoming);

            // Assert
            Assert.Empty(results);
        }

        #endregion

        #region GetMeetingsByAdminIdAndStatus Tests

        [Fact]
        public async Task GetMeetingsByAdminIdAndStatus_ReturnsCorrectMeetings_WhenDataExists()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            var employee1 = CreateTestEmployee(1, "Employee One");
            var employee2 = CreateTestEmployee(2, "Employee Two");

            var meeting1 = CreateTestMeeting(1, 1, MeetStatus.Upcoming, admin, employee1);
            var meeting2 = CreateTestMeeting(2, 2, MeetStatus.Upcoming, admin, employee2);
            var meeting3 = CreateTestMeeting(3, 1, MeetStatus.Completed, admin, employee1);

            _context.Admins.Add(admin);
            _context.Employees.AddRange(employee1, employee2);
            _context.Meetings.AddRange(meeting1, meeting2, meeting3);
            await _context.SaveChangesAsync();

            // Act
            var upcomingResults = await _meetingService.GetMeetingsByAdminIdAndStatus(1, MeetStatus.Upcoming);

            // Assert
            var upcomingList = upcomingResults.ToList();
            Assert.Equal(2, upcomingList.Count); // Two upcoming meetings for this admin
            Assert.All(upcomingList, m => Assert.Equal(MeetStatus.Upcoming, m.Status));
            Assert.All(upcomingList, m => Assert.Equal(1, m.AdminId));
        }

        #endregion

        #region GetAllPendingRequestsByAdminId Tests

        [Fact]
        public async Task GetAllPendingRequestsByAdminId_ReturnsOnlyRequestedMeetings_OrderedByRequestedDate()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            var employee1 = CreateTestEmployee(1, "Employee One");
            var employee2 = CreateTestEmployee(2, "Employee Two");

            var olderRequest = CreateTestMeeting(1, 1, MeetStatus.Requested, admin, employee1);
            olderRequest.RequestedAt = DateTime.UtcNow.AddDays(-2);

            var newerRequest = CreateTestMeeting(2, 2, MeetStatus.Requested, admin, employee2);
            newerRequest.RequestedAt = DateTime.UtcNow.AddDays(-1);

            var upcomingMeeting = CreateTestMeeting(3, 1, MeetStatus.Upcoming, admin, employee1);

            _context.Admins.Add(admin);
            _context.Employees.AddRange(employee1, employee2);
            _context.Meetings.AddRange(olderRequest, newerRequest, upcomingMeeting);
            await _context.SaveChangesAsync();

            // Act
            var results = await _meetingService.GetAllPendingRequestsByAdminId(1);

            // Assert
            var requestList = results.ToList();
            Assert.Equal(2, requestList.Count); // Only requested meetings
            Assert.All(requestList, r => Assert.Equal(MeetStatus.Requested, r.Status));

            // Should be ordered by RequestedAt descending (newest first)
            Assert.True(requestList[0].RequestedAt >= requestList[1].RequestedAt);
            Assert.Equal("Employee Two", requestList[0].EmployeeName); // Newer request first
        }

        [Fact]
        public async Task GetAllPendingRequestsByAdminId_ReturnsEmpty_WhenNoRequestedMeetings()
        {
            // Arrange
            var admin = CreateTestAdmin(1, "Admin User");
            var employee = CreateTestEmployee(1, "Employee User");
            var upcomingMeeting = CreateTestMeeting(1, 1, MeetStatus.Upcoming, admin, employee);

            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.Meetings.Add(upcomingMeeting);
            await _context.SaveChangesAsync();

            // Act
            var results = await _meetingService.GetAllPendingRequestsByAdminId(1);

            // Assert
            Assert.Empty(results);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a test admin with user
        /// </summary>
        private static Admin CreateTestAdmin(int adminId, string fullName)
        {
            var user = new User
            {
                UserId = adminId + 100, // Ensure unique user IDs
                FullName = fullName,
                Email = $"admin{adminId}@example.com",
                Role = UserRole.Admin
            };

            return new Admin
            {
                AdminId = adminId,
                UserId = user.UserId,
                User = user
            };
        }

        /// <summary>
        /// Creates a test employee with user
        /// </summary>
        private static Employee CreateTestEmployee(int employeeId, string fullName)
        {
            var user = new User
            {
                UserId = employeeId + 200, // Ensure unique user IDs
                FullName = fullName,
                Email = $"employee{employeeId}@example.com",
                Role = UserRole.Employee
            };

            return new Employee
            {
                EmployeeId = employeeId,
                UserId = user.UserId,
                User = user,
                Gender = Gender.Male,
                DateOfBirth = new DateOnly(1990, 1, 1),
                PhoneNumber = $"123-456-789{employeeId}",
                JobTitle = "Test Employee",
                Department = "Test Department",
                SalaryAmount = 50000,
                PayCycle = PayCycle.Monthly,
                EmployDate = DateOnly.FromDateTime(DateTime.Now),
                EmployType = EmployType.FullTime
            };
        }

        /// <summary>
        /// Creates a test meeting
        /// </summary>
        private static Meeting CreateTestMeeting(int adminId, int employeeId, MeetStatus status, Admin admin = null, Employee employee = null)
        {
            return new Meeting
            {
                AdminId = adminId,
                EmployeeId = employeeId,
                Purpose = $"Test meeting for employee {employeeId}",
                RequestedAt = DateTime.UtcNow,
                Status = status,
                IsOnline = true,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(1),
                Admin = admin,
                Employee = employee
            };
        }

        #endregion
    }
}