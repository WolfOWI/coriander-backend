using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CoriCore.Controllers;
using CoriCore.Interfaces;
using CoriCore.Models;
using CoriCore.DTOs;
using Microsoft.EntityFrameworkCore;
using CoriCore.Data;
using CoriCore.Services;

namespace CoriCore.Tests.Controllers
{
    public class PerformanceReviewControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PerformanceReviewService _service;
        private readonly PerformanceReviewController _controller;

        public PerformanceReviewControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);
            _service = new PerformanceReviewService(_context);
            _controller = new PerformanceReviewController(_context, _service);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetPerformanceReviews_ReturnsAllReviews()
        {
            // Arrange
            var review = new PerformanceReview
            {
                AdminId = 1,
                EmployeeId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPerformanceReviews();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PerformanceReview>>>(result);
            var reviews = Assert.IsAssignableFrom<IEnumerable<PerformanceReview>>(actionResult.Value);
            Assert.Single(reviews);
        }

        [Fact]
        public async Task GetPerformanceReview_ReturnsReview_WhenFound()
        {
            // Arrange
            var review = new PerformanceReview
            {
                AdminId = 1,
                EmployeeId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPerformanceReview(review.ReviewId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PerformanceReview>>(result);
            var returnedReview = Assert.IsType<PerformanceReview>(actionResult.Value);
            Assert.Equal(review.ReviewId, returnedReview.ReviewId);
        }

        [Fact]
        public async Task GetPerformanceReview_ReturnsNotFound_WhenNotFound()
        {
            // Act
            var result = await _controller.GetPerformanceReview(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetTopEmpUserRatingMetrics_ReturnsOk()
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
            var result = await _controller.GetTopEmpUserRatingMetrics(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var metrics = Assert.IsAssignableFrom<List<EmpUserRatingMetricsDTO>>(okResult.Value);
            Assert.Single(metrics);
        }

        [Fact]
        public async Task GetAllEmpUserRatingMetrics_ReturnsOk()
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
            var result = await _controller.GetAllEmpUserRatingMetrics();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var metrics = Assert.IsAssignableFrom<List<EmpUserRatingMetricsDTO>>(okResult.Value);
            Assert.Single(metrics);
        }

        [Fact]
        public async Task GetEmpUserRatingMetricsByEmpId_ReturnsOk_WhenFound()
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
            var result = await _controller.GetEmpUserRatingMetricsByEmpId(employee.EmployeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var metrics = Assert.IsType<EmpUserRatingMetricsDTO>(okResult.Value);
            Assert.Equal(employee.EmployeeId, metrics.EmployeeId);
        }

        [Fact]
        public async Task GetEmpUserRatingMetricsByEmpId_ReturnsNotFound_WhenNull()
        {
            // Act
            var result = await _controller.GetEmpUserRatingMetricsByEmpId(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetTopRatedEmployees_ReturnsOk_WhenFound()
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
            var result = await _controller.GetTopRatedEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var employees = Assert.IsAssignableFrom<List<TopRatedEmployeesDTO>>(okResult.Value);
            Assert.Single(employees);
        }

        [Fact]
        public async Task GetTopRatedEmployees_ReturnsNotFound_WhenEmpty()
        {
            // Act
            var result = await _controller.GetTopRatedEmployees();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreatePerformanceReview_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.CreatePerformanceReview(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreatePerformanceReview_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
            var empUser = new User { UserId = 2, FullName = "Employee", Role = UserRole.Employee };
            var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
            var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var dto = new PostPerformanceReviewDTO
            {
                AdminId = admin.AdminId,
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await _controller.CreatePerformanceReview(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedReview = Assert.IsType<PerformanceReview>(createdAtActionResult.Value);
            Assert.Equal(ReviewStatus.Upcoming, returnedReview.Status);
        }

        [Fact]
        public async Task UpdatePerformanceReview_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.UpdatePerformanceReview(1, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdatePerformanceReview_ReturnsNoContent_WhenSuccessful()
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
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = ReviewStatus.Upcoming
            };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            var dto = new PostPerformanceReviewDTO
            {
                AdminId = admin.AdminId,
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = ReviewStatus.Completed
            };

            // Act
            var result = await _controller.UpdatePerformanceReview(review.ReviewId, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetPrmByStartDateAdminId_ReturnsOk_WhenFound()
        {
            // Arrange
            var adminUser = new User { UserId = 1, FullName = "Admin", Role = UserRole.Admin };
            var empUser = new User { UserId = 2, FullName = "Employee", Role = UserRole.Employee };
            var admin = new Admin { AdminId = 1, UserId = 1, User = adminUser };
            var employee = new Employee { EmployeeId = 1, UserId = 2, User = empUser };

            var startDate = DateTime.UtcNow.Date;
            var review = new PerformanceReview
            {
                Admin = admin,
                Employee = employee,
                AdminId = admin.AdminId,
                EmployeeId = employee.EmployeeId,
                StartDate = startDate,
                EndDate = startDate.AddDays(1)
            };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPrmByStartDateAdminId(admin.AdminId, startDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReviews = Assert.IsAssignableFrom<IEnumerable<PerformanceReviewDTO>>(okResult.Value);
            Assert.Single(returnedReviews);
        }

        [Fact]
        public async Task GetPrmByStartDateAdminId_ReturnsNotFound_WhenNoReviews()
        {
            // Act
            var result = await _controller.GetPrmByStartDateAdminId(999, DateTime.UtcNow);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetPrmByEmpId_ReturnsOk_WhenFound()
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
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPrmByEmpId(employee.EmployeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReviews = Assert.IsAssignableFrom<IEnumerable<PerformanceReviewDTO>>(okResult.Value);
            Assert.Single(returnedReviews);
        }

        [Fact]
        public async Task GetPrmByEmpId_ReturnsNotFound_WhenNoReviews()
        {
            // Act
            var result = await _controller.GetPrmByEmpId(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllUpcomingPrm_ReturnsOk_WhenFound()
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
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = ReviewStatus.Upcoming
            };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllUpcomingPrm();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReviews = Assert.IsAssignableFrom<IEnumerable<PerformanceReviewDTO>>(okResult.Value);
            Assert.Single(returnedReviews);
        }

        [Fact]
        public async Task GetAllUpcomingPrm_ReturnsNotFound_WhenNoReviews()
        {
            // Act
            var result = await _controller.GetAllUpcomingPrm();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllUpcomingPrmByAdminId_ReturnsOk()
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
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = ReviewStatus.Upcoming
            };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllUpcomingPrmByAdminId(admin.AdminId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReviews = Assert.IsAssignableFrom<IEnumerable<PerformanceReviewDTO>>(okResult.Value);
            Assert.Single(returnedReviews);
        }

        [Fact]
        public async Task UpdateReviewStatus_ReturnsOk_WhenSuccessful()
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
                EndDate = DateTime.UtcNow.AddDays(1),
                Status = ReviewStatus.Upcoming
            };

            _context.Users.AddRange(adminUser, empUser);
            _context.Admins.Add(admin);
            _context.Employees.Add(employee);
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateReviewStatus(review.ReviewId, ReviewStatus.Completed);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PerformanceReview>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedReview = Assert.IsType<PerformanceReview>(okResult.Value);
            Assert.Equal(ReviewStatus.Completed, returnedReview.Status);
        }

        [Fact]
        public async Task UpdateReviewStatus_ReturnsNotFound_WhenReviewNotFound()
        {
            // Act
            var result = await _controller.UpdateReviewStatus(999, ReviewStatus.Completed);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PerformanceReview>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeletePerformanceReview_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var review = new PerformanceReview
            {
                AdminId = 1,
                EmployeeId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeletePerformanceReview(review.ReviewId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePerformanceReview_ReturnsNotFound_WhenReviewNotFound()
        {
            // Act
            var result = await _controller.DeletePerformanceReview(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}