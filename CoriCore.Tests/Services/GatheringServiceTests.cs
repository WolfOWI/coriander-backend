using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoriCore.Tests.Unit.Services
{
    public class GatheringServiceTests
    {
        private readonly GatheringService _gatheringService;
        private readonly AppDbContext _context;
        private readonly Mock<IMeetingService> _mockMeetingService;
        private readonly Mock<IPerformanceReviewService> _mockPerformanceReviewService;

        public GatheringServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);

            // Setup mocks for dependencies
            _mockMeetingService = new Mock<IMeetingService>();
            _mockPerformanceReviewService = new Mock<IPerformanceReviewService>();

            // Create the service with dependencies
            _gatheringService = new GatheringService(
                _context,
                _mockMeetingService.Object,
                _mockPerformanceReviewService.Object
            );
        }

        #region GetAllGatheringsByEmployeeId Tests

        [Fact]
        public async Task GetAllGatheringsByEmployeeId_ReturnsCorrectGatherings_WhenDataExists()
        {
            // Arrange
            int employeeId = 1;
            var testMeetings = CreateTestMeetingDTOs(employeeId);
            var testReviews = CreateTestPerformanceReviews(employeeId);

            // Setup meeting service mock to return meetings for all statuses
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Upcoming));
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Completed));
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Requested));
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Rejected));

            // Add performance reviews to database
            _context.PerformanceReviews.AddRange(testReviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _gatheringService.GetAllGatheringsByEmployeeId(employeeId);

            // Assert
            var gatherings = result.ToList();
            Assert.NotEmpty(gatherings);

            // Verify meetings are included
            var meetingGatherings = gatherings.Where(g => g.Type == GatheringType.Meeting).ToList();
            Assert.Equal(testMeetings.Count(), meetingGatherings.Count);

            // Verify performance reviews are included
            var reviewGatherings = gatherings.Where(g => g.Type == GatheringType.PerformanceReview).ToList();
            Assert.Equal(testReviews.Count, reviewGatherings.Count);

            // Verify gatherings are sorted by start date
            var sortedGatherings = gatherings.OrderBy(g => g.StartDate).ThenBy(g => g.Type).ToList();
            Assert.Equal(sortedGatherings, gatherings.ToList());
        }

        [Fact]
        public async Task GetAllGatheringsByEmployeeId_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            int employeeId = 999; // Non-existent employee

            // Setup mocks to return empty lists
            foreach (MeetStatus status in Enum.GetValues(typeof(MeetStatus)))
            {
                _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, status))
                    .ReturnsAsync(new List<MeetingDTO>());
            }

            // Act
            var result = await _gatheringService.GetAllGatheringsByEmployeeId(employeeId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllGatheringsByEmployeeId_HandlesMeetingsWithNullValues_Gracefully()
        {
            // Arrange
            int employeeId = 1;
            var meetingWithNulls = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    EmployeeId = employeeId,
                    AdminId = 1,
                    AdminName = null, // Null value to test handling
                    EmployeeName = null, // Null value to test handling
                    Purpose = "Test Meeting",
                    Status = MeetStatus.Upcoming,
                    StartDate = DateTime.Now.AddDays(1)
                }
            };

            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
                .ReturnsAsync(meetingWithNulls);

            // Setup other statuses to return empty
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected))
                .ReturnsAsync(new List<MeetingDTO>());

            // Act
            var result = await _gatheringService.GetAllGatheringsByEmployeeId(employeeId);

            // Assert
            var gatherings = result.ToList();
            Assert.Single(gatherings);
            Assert.Equal(string.Empty, gatherings[0].AdminName); // Should handle null gracefully
            Assert.Equal(string.Empty, gatherings[0].EmployeeName); // Should handle null gracefully
        }

        #endregion

        #region GetAllGatheringsByEmployeeIdAndStatus Tests

        [Fact]
        public async Task GetAllGatheringsByEmployeeIdAndStatus_ReturnsUpcomingGatherings_WhenStatusIsUpcoming()
        {
            // Arrange
            int employeeId = 1;
            var upcomingMeetings = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    EmployeeId = employeeId,
                    Status = MeetStatus.Upcoming,
                    Purpose = "Upcoming Meeting",
                    StartDate = DateTime.Now.AddDays(1)
                }
            };

            var upcomingReviews = new List<PerformanceReview>
            {
                CreatePerformanceReview(1, 1, employeeId, ReviewStatus.Upcoming, DateTime.Now.AddDays(2))
            };

            // Setup mocks
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
                .ReturnsAsync(upcomingMeetings);
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected))
                .ReturnsAsync(new List<MeetingDTO>());

            _context.PerformanceReviews.AddRange(upcomingReviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _gatheringService.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Upcoming");

            // Assert
            var gatherings = result.ToList();
            Assert.Single(gatherings); // Only meetings are filtered by status, not reviews
            Assert.Equal(MeetStatus.Upcoming, gatherings[0].MeetingStatus);
        }

        [Fact]
        public async Task GetAllGatheringsByEmployeeIdAndStatus_ReturnsCompletedGatherings_WhenStatusIsCompleted()
        {
            // Arrange
            int employeeId = 1;
            var completedMeetings = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 2,
                    EmployeeId = employeeId,
                    Status = MeetStatus.Completed,
                    Purpose = "Completed Meeting",
                    StartDate = DateTime.Now.AddDays(-1)
                }
            };

            var completedReviews = new List<PerformanceReview>
            {
                CreatePerformanceReview(2, 1, employeeId, ReviewStatus.Completed, DateTime.Now.AddDays(-2))
            };

            // Setup mocks for completed gatherings
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
                .ReturnsAsync(completedMeetings);
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected))
                .ReturnsAsync(new List<MeetingDTO>());

            _context.PerformanceReviews.AddRange(completedReviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _gatheringService.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Completed");

            // Assert
            var gatherings = result.ToList();
            Assert.Single(gatherings); // Only meetings are filtered by status, not reviews
            Assert.Equal(MeetStatus.Completed, gatherings[0].MeetingStatus);
        }

        #endregion

        #region GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending Tests

        [Fact]
        public async Task GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending_ReturnsSortedGatherings()
        {
            // Arrange
            int employeeId = 1;
            var upcomingMeetings = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    EmployeeId = employeeId,
                    Status = MeetStatus.Upcoming,
                    StartDate = DateTime.Now.AddDays(1),
                    Purpose = "Future Meeting"
                }
            };

            var completedMeetings = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 2,
                    EmployeeId = employeeId,
                    Status = MeetStatus.Completed,
                    StartDate = DateTime.Now.AddDays(-1),
                    Purpose = "Past Meeting"
                }
            };

            // Setup mocks for both upcoming and completed
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming))
                .ReturnsAsync(upcomingMeetings);
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed))
                .ReturnsAsync(completedMeetings);
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected))
                .ReturnsAsync(new List<MeetingDTO>());

            // Act
            var result = await _gatheringService.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId);

            // Assert
            var gatherings = result.ToList();
            Assert.Equal(2, gatherings.Count);

            // Should be sorted by StartDate descending
            Assert.True(gatherings[0].StartDate >= gatherings[1].StartDate);
            Assert.Equal("Future Meeting", gatherings[0].Purpose); // Most recent first
        }

        #endregion

        #region GetAllGatheringsByAdminId Tests

        [Fact]
        public async Task GetAllGatheringsByAdminId_ReturnsCorrectGatherings_WhenDataExists()
        {
            // Arrange
            int adminId = 1;
            var testMeetings = CreateTestMeetingDTOsForAdmin(adminId);
            var testReviews = CreateTestPerformanceReviewsForAdmin(adminId);

            // Setup meeting service mock for all statuses
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Upcoming));
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Completed))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Completed));
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Requested))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Requested));
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Rejected))
                .ReturnsAsync(testMeetings.Where(m => m.Status == MeetStatus.Rejected));

            // Add performance reviews to database
            _context.PerformanceReviews.AddRange(testReviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _gatheringService.GetAllGatheringsByAdminId(adminId);

            // Assert
            var gatherings = result.ToList();
            Assert.NotEmpty(gatherings);

            // Verify meetings are included
            var meetingGatherings = gatherings.Where(g => g.Type == GatheringType.Meeting).ToList();
            Assert.Equal(testMeetings.Count(), meetingGatherings.Count);

            // Verify performance reviews are included
            var reviewGatherings = gatherings.Where(g => g.Type == GatheringType.PerformanceReview).ToList();
            Assert.Equal(testReviews.Count, reviewGatherings.Count);

            // Verify all gatherings belong to the admin
            Assert.All(gatherings, g => Assert.Equal(adminId, g.AdminId));
        }

        #endregion

        #region GetUpcomingAndCompletedGatheringsByAdminIdAndMonth Tests

        [Fact]
        public async Task GetUpcomingAndCompletedGatheringsByAdminIdAndMonth_ReturnsCorrectMonth_WhenValidMonth()
        {
            // Arrange
            int adminId = 1;
            int targetMonth = 6; // June
            var currentYear = DateTime.Now.Year;

            var juneUpcomingMeetings = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    AdminId = adminId,
                    Status = MeetStatus.Upcoming,
                    StartDate = new DateTime(currentYear, 6, 15), // June 15th
                    Purpose = "June Meeting"
                }
            };

            var mayUpcomingMeetings = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 2,
                    AdminId = adminId,
                    Status = MeetStatus.Upcoming,
                    StartDate = new DateTime(currentYear, 5, 15), // May 15th - should be filtered out
                    Purpose = "May Meeting"
                }
            };

            // Setup mocks
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming))
                .ReturnsAsync(juneUpcomingMeetings.Concat(mayUpcomingMeetings));
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Completed))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Requested))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Rejected))
                .ReturnsAsync(new List<MeetingDTO>());

            // Act
            var result = await _gatheringService.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, targetMonth.ToString());

            // Assert
            var gatherings = result.ToList();
            Assert.Single(gatherings); // Only June meeting should be included
            Assert.Equal(targetMonth, gatherings[0].StartDate?.Month);
            Assert.Equal("June Meeting", gatherings[0].Purpose);
        }

        [Fact]
        public async Task GetUpcomingAndCompletedGatheringsByAdminIdAndMonth_ThrowsException_WhenInvalidMonth()
        {
            // Arrange
            int adminId = 1;
            string invalidMonth = "13"; // Invalid month

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _gatheringService.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, invalidMonth));
        }

        [Fact]
        public async Task GetUpcomingAndCompletedGatheringsByAdminIdAndMonth_ThrowsException_WhenNonNumericMonth()
        {
            // Arrange
            int adminId = 1;
            string invalidMonth = "January"; // Non-numeric

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _gatheringService.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, invalidMonth));
        }

        [Fact]
        public async Task GetUpcomingAndCompletedGatheringsByAdminIdAndMonth_HandlesNullStartDates_Gracefully()
        {
            // Arrange
            int adminId = 1;
            int targetMonth = 6;

            var meetingsWithNullDates = new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    AdminId = adminId,
                    Status = MeetStatus.Upcoming,
                    StartDate = null, // Null start date
                    Purpose = "Meeting with null date"
                }
            };

            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming))
                .ReturnsAsync(meetingsWithNullDates);
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Completed))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Requested))
                .ReturnsAsync(new List<MeetingDTO>());
            _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Rejected))
                .ReturnsAsync(new List<MeetingDTO>());

            // Act
            var result = await _gatheringService.GetUpcomingAndCompletedGatheringsByAdminIdAndMonth(adminId, targetMonth.ToString());

            // Assert
            Assert.Empty(result); // Should filter out null dates
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates test meeting DTOs for a specific employee
        /// </summary>
        private static List<MeetingDTO> CreateTestMeetingDTOs(int employeeId)
        {
            return new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    AdminId = 1,
                    AdminName = "Admin One",
                    EmployeeId = employeeId,
                    EmployeeName = "Employee One",
                    Purpose = "Weekly Standup",
                    Status = MeetStatus.Upcoming,
                    StartDate = DateTime.Now.AddDays(1)
                },
                new MeetingDTO
                {
                    MeetingId = 2,
                    AdminId = 1,
                    AdminName = "Admin One",
                    EmployeeId = employeeId,
                    EmployeeName = "Employee One",
                    Purpose = "Performance Discussion",
                    Status = MeetStatus.Completed,
                    StartDate = DateTime.Now.AddDays(-2)
                }
            };
        }

        /// <summary>
        /// Creates test meeting DTOs for a specific admin
        /// </summary>
        private static List<MeetingDTO> CreateTestMeetingDTOsForAdmin(int adminId)
        {
            return new List<MeetingDTO>
            {
                new MeetingDTO
                {
                    MeetingId = 1,
                    AdminId = adminId,
                    AdminName = "Admin One",
                    EmployeeId = 1,
                    EmployeeName = "Employee One",
                    Purpose = "Team Meeting",
                    Status = MeetStatus.Upcoming,
                    StartDate = DateTime.Now.AddDays(1)
                },
                new MeetingDTO
                {
                    MeetingId = 2,
                    AdminId = adminId,
                    AdminName = "Admin One",
                    EmployeeId = 2,
                    EmployeeName = "Employee Two",
                    Purpose = "Project Review",
                    Status = MeetStatus.Completed,
                    StartDate = DateTime.Now.AddDays(-1)
                }
            };
        }

        /// <summary>
        /// Creates test performance reviews for a specific employee
        /// </summary>
        private List<PerformanceReview> CreateTestPerformanceReviews(int employeeId)
        {
            var admin = new Admin { AdminId = 1, UserId = 1, User = new User { UserId = 1, FullName = "Admin User" } };
            var employee = new Employee { EmployeeId = employeeId, UserId = 2, User = new User { UserId = 2, FullName = "Employee User" } };

            _context.Users.AddRange(admin.User, employee.User);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);

            return new List<PerformanceReview>
            {
                CreatePerformanceReview(1, 1, employeeId, ReviewStatus.Upcoming, DateTime.Now.AddDays(3)),
                CreatePerformanceReview(2, 1, employeeId, ReviewStatus.Completed, DateTime.Now.AddDays(-5))
            };
        }

        /// <summary>
        /// Creates test performance reviews for a specific admin
        /// </summary>
        private List<PerformanceReview> CreateTestPerformanceReviewsForAdmin(int adminId)
        {
            var admin = new Admin { AdminId = adminId, UserId = 1, User = new User { UserId = 1, FullName = "Admin User" } };
            var employee1 = new Employee { EmployeeId = 1, UserId = 2, User = new User { UserId = 2, FullName = "Employee One" } };
            var employee2 = new Employee { EmployeeId = 2, UserId = 3, User = new User { UserId = 3, FullName = "Employee Two" } };

            _context.Users.AddRange(admin.User, employee1.User, employee2.User);
            _context.Admins.Add(admin);
            _context.Employees.AddRange(employee1, employee2);

            return new List<PerformanceReview>
            {
                CreatePerformanceReview(1, adminId, 1, ReviewStatus.Upcoming, DateTime.Now.AddDays(2)),
                CreatePerformanceReview(2, adminId, 2, ReviewStatus.Completed, DateTime.Now.AddDays(-3))
            };
        }

        /// <summary>
        /// Helper method to create a performance review
        /// </summary>
        private static PerformanceReview CreatePerformanceReview(int reviewId, int adminId, int employeeId, ReviewStatus status, DateTime startDate)
        {
            return new PerformanceReview
            {
                ReviewId = reviewId,
                AdminId = adminId,
                EmployeeId = employeeId,
                StartDate = startDate,
                EndDate = startDate.AddHours(1),
                Status = status,
                IsOnline = true,
                MeetLocation = "Conference Room A",
                Comment = "Test review comment"
            };
        }

        #endregion
    }
}