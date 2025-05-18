using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;

        public MeetingController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        // GET
        // ========================================
        // Get all rejected & requested meetings (requests) for an employee
        [HttpGet("GetAllRequestsByEmpId/{employeeId}")]
        public async Task<IActionResult> GetAllRequestsByEmpId(int employeeId)
        {
            var pendingMeetingRequests = await _meetingService.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Requested);
            var rejectedMeetingRequests = await _meetingService.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected);

            // Combine the two lists
            var allMeetingRequests = rejectedMeetingRequests.Concat(pendingMeetingRequests);

            // Sort by requestedAt descending
            allMeetingRequests = allMeetingRequests.OrderByDescending(m => m.RequestedAt);

            // Return the combined list
            return Ok(allMeetingRequests);
        }

        // ========================================

        // CREATE
        // ========================================
        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateEmployeeMeetingRequest([FromBody] MeetingRequestCreateDTO meetingRequestCreateDTO)
        {
            var meetingRequest = await _meetingService.CreateMeetingRequest(meetingRequestCreateDTO);
            return Ok(meetingRequest);
        }
        // ========================================

        // UPDATE
        // ========================================
        [HttpPut("ConfirmAndSchedule/{meetingId}")]
        public async Task<IActionResult> ConfirmAndScheduleMeetingRequest(int meetingId, [FromBody] MeetingConfirmDTO meetingConfirmDTO)
        {
            var (code, message) = await _meetingService.ConfirmAndUpdateMeetingRequest(meetingId, meetingConfirmDTO);
            return StatusCode(code, message);
        }

        [HttpPut("Reject/{meetingId}")]
        public async Task<IActionResult> RejectMeetingRequest(int meetingId)
        {
            var (code, message) = await _meetingService.RejectMeetingRequest(meetingId);
            return StatusCode(code, message);
        }

        [HttpPut("MarkAsCompleted/{meetingId}")]
        public async Task<IActionResult> MarkMeetingAsCompleted(int meetingId)
        {
            var (code, message) = await _meetingService.MarkMeetingAsCompleted(meetingId);
            return StatusCode(code, message);
        }
        // ========================================
    }


}
