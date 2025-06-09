using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoriCore.Tests.Controllers
{
    public class LeaveRequestControllerTests : IDisposable
    {
        private readonly Mock<ILeaveRequestService> _mockLeaveRequestService;
        private readonly Mock<IApplyForLeaveService> _mockApplyForLeaveService;
        private readonly LeaveRequestController _controller;
        private readonly AppDbContext _context;

        public LeaveRequestControllerTests()
        {
            // Create in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            _mockLeaveRequestService = new Mock<ILeaveRequestService>();
            _mockApplyForLeaveService = new Mock<IApplyForLeaveService>();

            _controller = new LeaveRequestController(
                _context,
                _mockLeaveRequestService.Object,
                _mockApplyForLeaveService.Object
            );
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetLeaveRequestsByEmployeeId_ReturnsOkResult_WithLeaveRequests()
        {
            // Arrange
            var employeeId = 1;
            var expectedRequests = new List<LeaveRequestDTO>
            {
                new LeaveRequestDTO
                {
                    LeaveRequestId = 1,
                    EmployeeId = employeeId,
                    EmployeeName = "John Doe",
                    LeaveTypeId = 1,
                    LeaveType = "Annual Leave",
                    StartDate = DateOnly.FromDateTime(DateTime.Today),
                    EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                    Status = LeaveStatus.Pending,
                    LeaveTypeName = "Annual Leave",
                    DefaultDays = 20
                }
            };

            _mockLeaveRequestService
                .Setup(s => s.GetLeaveRequestsByEmployeeId(employeeId))
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _controller.GetLeaveRequestsByEmployeeId(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<LeaveRequestDTO>>(okResult.Value);
            Assert.Single(returnedRequests);
            Assert.Equal(expectedRequests[0].LeaveRequestId, returnedRequests[0].LeaveRequestId);
        }

        [Fact]
        public async Task GetLeaveRequestsByEmployeeId_ReturnsNotFound_WhenNoRequestsFound()
        {
            // Arrange
            var employeeId = 1;
            var emptyRequests = new List<LeaveRequestDTO>();

            _mockLeaveRequestService
                .Setup(s => s.GetLeaveRequestsByEmployeeId(employeeId))
                .ReturnsAsync(emptyRequests);

            // Act
            var result = await _controller.GetLeaveRequestsByEmployeeId(employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllLeaveRequests_ReturnsOkResult_WithLeaveRequests()
        {
            // Arrange
            var expectedRequests = new List<LeaveRequestDTO>
            {
                new LeaveRequestDTO
                {
                    LeaveRequestId = 1,
                    EmployeeId = 1,
                    EmployeeName = "John Doe",
                    LeaveTypeId = 1,
                    LeaveType = "Annual Leave",
                    StartDate = DateOnly.FromDateTime(DateTime.Today),
                    EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                    Status = LeaveStatus.Pending,
                    LeaveTypeName = "Annual Leave",
                    DefaultDays = 20
                }
            };

            _mockLeaveRequestService
                .Setup(s => s.GetAllLeaveRequests())
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _controller.GetAllLeaveRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<LeaveRequestDTO>>(okResult.Value);
            Assert.Single(returnedRequests);
            Assert.Equal(expectedRequests[0].LeaveRequestId, returnedRequests[0].LeaveRequestId);
        }

        [Fact]
        public async Task GetAllLeaveRequests_ReturnsNotFound_WhenNoRequestsFound()
        {
            // Arrange
            var emptyRequests = new List<LeaveRequestDTO>();

            _mockLeaveRequestService
                .Setup(s => s.GetAllLeaveRequests())
                .ReturnsAsync(emptyRequests);

            // Act
            var result = await _controller.GetAllLeaveRequests();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task SubmitLeaveRequest_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange
            ApplyForLeaveDTO? nullDto = null;

            // Act
            var result = await _controller.SubmitLeaveRequest(nullDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Leave request cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task SubmitLeaveRequest_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var leaveRequestDto = new ApplyForLeaveDTO
            {
                EmployeeId = 1,
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                Comment = "Vacation"
            };

            var createdRequest = new LeaveRequestDTO
            {
                LeaveRequestId = 1,
                EmployeeId = leaveRequestDto.EmployeeId,
                LeaveTypeId = leaveRequestDto.LeaveTypeId,
                StartDate = leaveRequestDto.StartDate,
                EndDate = leaveRequestDto.EndDate,
                Status = LeaveStatus.Pending,
                LeaveTypeName = "Annual Leave"
            };

            _mockApplyForLeaveService
                .Setup(s => s.ApplyForLeave(leaveRequestDto))
                .ReturnsAsync(createdRequest);

            // Act
            var result = await _controller.SubmitLeaveRequest(leaveRequestDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedRequest = Assert.IsType<LeaveRequestDTO>(createdAtActionResult.Value);
            Assert.Equal(createdRequest.LeaveRequestId, returnedRequest.LeaveRequestId);
        }

        [Fact]
        public async Task DeleteLeaveRequest_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            // Arrange
            var leaveRequestId = 1;

            // Act
            var result = await _controller.DeleteLeaveRequest(leaveRequestId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteLeaveRequest_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var leaveRequestId = 1;
            var leaveRequest = new LeaveRequest { LeaveRequestId = leaveRequestId };
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteLeaveRequest(leaveRequestId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.LeaveRequests.FindAsync(leaveRequestId));
        }
    }
}