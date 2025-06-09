using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoriCore.Controllers;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CoriCore.Tests.Controllers
{
    public class EmpLeaveRequestControllerTests
    {
        private readonly Mock<IEmpLeaveRequestService> _mockLeaveRequestService;
        private readonly EmpLeaveRequestController _controller;

        public EmpLeaveRequestControllerTests()
        {
            _mockLeaveRequestService = new Mock<IEmpLeaveRequestService>();
            _controller = new EmpLeaveRequestController(_mockLeaveRequestService.Object);
        }

        private static EmpLeaveRequestDTO CreateSampleLeaveRequest(int id = 1, LeaveStatus status = LeaveStatus.Pending)
        {
            return new EmpLeaveRequestDTO
            {
                LeaveRequestId = id,
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                Status = status,
                LeaveTypeName = "Annual Leave",
                UserId = 1,
                FullName = "John Doe",
                EmployeeId = 1,
                EmployType = EmployType.FullTime,
                IsSuspended = false,
                RemainingDays = 15
            };
        }

        [Fact]
        public async Task GetAllEmployeeLeaveRequests_ReturnsOkResult_WithLeaveRequests()
        {
            // Arrange
            var expectedRequests = new List<EmpLeaveRequestDTO>
            {
                CreateSampleLeaveRequest(1),
                CreateSampleLeaveRequest(2)
            };

            _mockLeaveRequestService
                .Setup(s => s.GetAllEmployeeLeaveRequests())
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _controller.GetAllEmployeeLeaveRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<EmpLeaveRequestDTO>>(okResult.Value);
            Assert.Equal(2, returnedRequests.Count);
        }

        [Fact]
        public async Task GetAllEmployeeLeaveRequests_ReturnsNotFound_WhenNoRequestsFound()
        {
            // Arrange
            _mockLeaveRequestService
                .Setup(s => s.GetAllEmployeeLeaveRequests())
                .ReturnsAsync(new List<EmpLeaveRequestDTO>());

            // Act
            var result = await _controller.GetAllEmployeeLeaveRequests();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No leave requests found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPendingLeaveRequests_ReturnsOkResult_WithPendingRequests()
        {
            // Arrange
            var expectedRequests = new List<EmpLeaveRequestDTO>
            {
                CreateSampleLeaveRequest(1, LeaveStatus.Pending),
                CreateSampleLeaveRequest(2, LeaveStatus.Pending)
            };

            _mockLeaveRequestService
                .Setup(s => s.GetPendingLeaveRequests())
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _controller.GetPendingLeaveRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<EmpLeaveRequestDTO>>(okResult.Value);
            Assert.All(returnedRequests, request => Assert.Equal(LeaveStatus.Pending, request.Status));
        }

        [Fact]
        public async Task GetApprovedLeaveRequests_ReturnsOkResult_WithApprovedRequests()
        {
            // Arrange
            var expectedRequests = new List<EmpLeaveRequestDTO>
            {
                CreateSampleLeaveRequest(1, LeaveStatus.Approved),
                CreateSampleLeaveRequest(2, LeaveStatus.Approved)
            };

            _mockLeaveRequestService
                .Setup(s => s.GetApprovedLeaveRequests())
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _controller.GetApprovedLeaveRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<EmpLeaveRequestDTO>>(okResult.Value);
            Assert.All(returnedRequests, request => Assert.Equal(LeaveStatus.Approved, request.Status));
        }

        [Fact]
        public async Task GetRejectedLeaveRequests_ReturnsOkResult_WithRejectedRequests()
        {
            // Arrange
            var expectedRequests = new List<EmpLeaveRequestDTO>
            {
                CreateSampleLeaveRequest(1, LeaveStatus.Rejected),
                CreateSampleLeaveRequest(2, LeaveStatus.Rejected)
            };

            _mockLeaveRequestService
                .Setup(s => s.GetRejectedLeaveRequests())
                .ReturnsAsync(expectedRequests);

            // Act
            var result = await _controller.GetRejectedLeaveRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<EmpLeaveRequestDTO>>(okResult.Value);
            Assert.All(returnedRequests, request => Assert.Equal(LeaveStatus.Rejected, request.Status));
        }

        [Fact]
        public async Task ApproveLeaveRequestById_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var leaveRequestId = 1;
            _mockLeaveRequestService
                .Setup(s => s.ApproveLeaveRequestById(leaveRequestId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveLeaveRequestById(leaveRequestId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ApproveLeaveRequestById_ReturnsNotFound_WhenRequestNotFound()
        {
            // Arrange
            var leaveRequestId = 999;
            _mockLeaveRequestService
                .Setup(s => s.ApproveLeaveRequestById(leaveRequestId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ApproveLeaveRequestById(leaveRequestId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Leave request with ID {leaveRequestId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task RejectLeaveRequestById_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var leaveRequestId = 1;
            _mockLeaveRequestService
                .Setup(s => s.RejectLeaveRequestById(leaveRequestId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RejectLeaveRequestById(leaveRequestId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RejectLeaveRequestById_ReturnsNotFound_WhenRequestNotFound()
        {
            // Arrange
            var leaveRequestId = 999;
            _mockLeaveRequestService
                .Setup(s => s.RejectLeaveRequestById(leaveRequestId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RejectLeaveRequestById(leaveRequestId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Leave request with ID {leaveRequestId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task SetLeaveRequestToPendingById_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var leaveRequestId = 1;
            _mockLeaveRequestService
                .Setup(s => s.SetLeaveRequestToPendingById(leaveRequestId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SetLeaveRequestToPendingById(leaveRequestId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SetLeaveRequestToPendingById_ReturnsNotFound_WhenRequestNotFound()
        {
            // Arrange
            var leaveRequestId = 999;
            _mockLeaveRequestService
                .Setup(s => s.SetLeaveRequestToPendingById(leaveRequestId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SetLeaveRequestToPendingById(leaveRequestId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Leave request with ID {leaveRequestId} not found.", notFoundResult.Value);
        }
    }
}