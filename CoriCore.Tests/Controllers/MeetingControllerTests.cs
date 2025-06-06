using System;
using CoriCore.Controllers;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class MeetingControllerTests
{
    private readonly MeetingController _controller;
    private readonly Mock<IMeetingService> _mockMeetingService;

    public MeetingControllerTests()
    {
        _mockMeetingService = new Mock<IMeetingService>();
        _controller = new MeetingController(_mockMeetingService.Object);
    }

    [Fact]
    public async Task GetAllRequestsByEmpId_ReturnsOkWithCombinedRequests()
    {
        // Arrange
        var employeeId = 1;
        var pendingRequests = new List<MeetingDTO>
        {
            new MeetingDTO { MeetingId = 1, Purpose = "Pending Meeting 1" }
        };
        var rejectedRequests = new List<MeetingDTO>
        {
            new MeetingDTO { MeetingId = 2, Purpose = "Rejected Meeting 1" }
        };

        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested))
            .ReturnsAsync(pendingRequests);
        _mockMeetingService.Setup(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected))
            .ReturnsAsync(rejectedRequests);

        // Act
        var result = await _controller.GetAllRequestsByEmpId(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequests = Assert.IsAssignableFrom<IEnumerable<MeetingDTO>>(okResult.Value);
        Assert.Equal(2, returnedRequests.Count());
        _mockMeetingService.Verify(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested), Times.Once);
        _mockMeetingService.Verify(x => x.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected), Times.Once);
    }

    [Fact]
    public async Task GetAllUpcomingByAdminId_ReturnsOkWithUpcomingMeetings()
    {
        // Arrange
        var adminId = 1;
        var upcomingMeetings = new List<MeetingDTO>
        {
            new MeetingDTO { MeetingId = 1, Purpose = "Upcoming Meeting 1" },
            new MeetingDTO { MeetingId = 2, Purpose = "Upcoming Meeting 2" }
        };

        _mockMeetingService.Setup(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming))
            .ReturnsAsync(upcomingMeetings);

        // Act
        var result = await _controller.GetAllUpcomingByAdminId(adminId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeetings = Assert.IsAssignableFrom<IEnumerable<MeetingDTO>>(okResult.Value);
        Assert.Equal(2, returnedMeetings.Count());
        _mockMeetingService.Verify(x => x.GetMeetingsByAdminIdAndStatus(adminId, MeetStatus.Upcoming), Times.Once);
    }

    [Fact]
    public async Task GetAllPendingRequestsByAdminId_ReturnsOkWithPendingRequests()
    {
        // Arrange
        var adminId = 1;
        var pendingRequests = new List<MeetingRequestDTO>
        {
            new MeetingRequestDTO { MeetingId = 1, Purpose = "Pending Request 1" }
        };

        _mockMeetingService.Setup(x => x.GetAllPendingRequestsByAdminId(adminId))
            .ReturnsAsync(pendingRequests);

        // Act
        var result = await _controller.GetAllPendingRequestsByAdminId(adminId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequests = Assert.IsAssignableFrom<IEnumerable<MeetingRequestDTO>>(okResult.Value);
        Assert.Single(returnedRequests);
        _mockMeetingService.Verify(x => x.GetAllPendingRequestsByAdminId(adminId), Times.Once);
    }

    [Fact]
    public async Task CreateEmployeeMeetingRequest_ReturnsOkWithCreatedRequest()
    {
        // Arrange
        var createDto = new MeetingRequestCreateDTO
        {
            Purpose = "New Meeting Request",
            AdminId = 1,
            EmployeeId = 1
        };
        var createdRequest = new MeetingRequestCreateDTO { Purpose = "New Meeting Request" };

        _mockMeetingService.Setup(x => x.CreateMeetingRequest(createDto))
            .ReturnsAsync(createdRequest);

        // Act
        var result = await _controller.CreateEmployeeMeetingRequest(createDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequest = Assert.IsType<MeetingRequestCreateDTO>(okResult.Value);
        Assert.Equal(createdRequest.Purpose, returnedRequest.Purpose);
        _mockMeetingService.Verify(x => x.CreateMeetingRequest(createDto), Times.Once);
    }

    [Fact]
    public async Task ConfirmAndScheduleMeetingRequest_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;
        var updateDto = new MeetingUpdateDTO
        {
            StartDate = DateTime.Now.AddDays(1),
            IsOnline = true
        };

        _mockMeetingService.Setup(x => x.ConfirmAndUpdateMeetingRequest(meetingId, updateDto))
            .ReturnsAsync((200, "Meeting confirmed successfully"));

        // Act
        var result = await _controller.ConfirmAndScheduleMeetingRequest(meetingId, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        Assert.Equal("Meeting confirmed successfully", statusCodeResult.Value);
        _mockMeetingService.Verify(x => x.ConfirmAndUpdateMeetingRequest(meetingId, updateDto), Times.Once);
    }

    [Fact]
    public async Task UpdateMeeting_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;
        var updateDto = new MeetingUpdateDTO
        {
            MeetLocation = "Updated Meeting Location"
        };

        _mockMeetingService.Setup(x => x.UpdateMeeting(meetingId, updateDto))
            .ReturnsAsync((200, "Meeting updated successfully"));

        // Act
        var result = await _controller.UpdateMeeting(meetingId, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockMeetingService.Verify(x => x.UpdateMeeting(meetingId, updateDto), Times.Once);
    }

    [Fact]
    public async Task UpdateMeetingRequest_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;
        var updateDto = new MeetingRequestUpdateDTO
        {
            Purpose = "Updated Request Title"
        };

        _mockMeetingService.Setup(x => x.UpdateMeetingRequest(meetingId, updateDto))
            .ReturnsAsync((200, "Meeting request updated successfully"));

        // Act
        var result = await _controller.UpdateMeetingRequest(meetingId, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockMeetingService.Verify(x => x.UpdateMeetingRequest(meetingId, updateDto), Times.Once);
    }

    [Fact]
    public async Task RejectMeetingRequest_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;

        _mockMeetingService.Setup(x => x.RejectMeetingRequest(meetingId))
            .ReturnsAsync((200, "Meeting request rejected"));

        // Act
        var result = await _controller.RejectMeetingRequest(meetingId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockMeetingService.Verify(x => x.RejectMeetingRequest(meetingId), Times.Once);
    }

    [Fact]
    public async Task MarkMeetingAsCompleted_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;

        _mockMeetingService.Setup(x => x.MarkMeetingAsCompleted(meetingId))
            .ReturnsAsync((200, "Meeting marked as completed"));

        // Act
        var result = await _controller.MarkMeetingAsCompleted(meetingId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockMeetingService.Verify(x => x.MarkMeetingAsCompleted(meetingId), Times.Once);
    }

    [Fact]
    public async Task MarkMeetingAsUpcoming_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;

        _mockMeetingService.Setup(x => x.MarkMeetingAsUpcoming(meetingId))
            .ReturnsAsync((200, "Meeting marked as upcoming"));

        // Act
        var result = await _controller.MarkMeetingAsUpcoming(meetingId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockMeetingService.Verify(x => x.MarkMeetingAsUpcoming(meetingId), Times.Once);
    }

    [Fact]
    public async Task DeleteMeeting_ReturnsStatusCode_FromService()
    {
        // Arrange
        var meetingId = 1;

        _mockMeetingService.Setup(x => x.DeleteMeeting(meetingId))
            .ReturnsAsync((200, "Meeting deleted successfully"));

        // Act
        var result = await _controller.DeleteMeeting(meetingId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockMeetingService.Verify(x => x.DeleteMeeting(meetingId), Times.Once);
    }
}