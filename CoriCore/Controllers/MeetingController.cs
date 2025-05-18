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
        // EMPLOYEE RELATED 
        // ------------------------
        [HttpGet("GetAllByEmpId/{employeeId}")]
        public async Task<IActionResult> GetAllMeetingsByEmployeeId(int employeeId)
        {
            var meetings = await _meetingService.GetAllMeetingsByEmployeeId(employeeId);
            return Ok(meetings);
        }
        

        [HttpGet("GetUpcomingByEmpId/{employeeId}")]
        public async Task<IActionResult> GetUpcomingMeetingsByEmployeeId(int employeeId)
        {
            var meetings = await _meetingService.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Upcoming);
            return Ok(meetings);
        }

        [HttpGet("GetCompletedByEmpId/{employeeId}")]
        public async Task<IActionResult> GetCompletedMeetingsByEmployeeId(int employeeId)
        {
            var meetings = await _meetingService.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Completed);
            return Ok(meetings);
        }

        [HttpGet("GetRejectedByEmpId/{employeeId}")]
        public async Task<IActionResult> GetRejectedMeetingsByEmployeeId(int employeeId)
        {
            var meetings = await _meetingService.GetMeetingsByEmployeeIdAndStatus(employeeId, MeetStatus.Rejected);
            return Ok(meetings);
        }
        // ------------------------



        // ADMIN RELATED 
        // ------------------------
        [HttpGet("GetAllByAdminId/{adminId}")]
        public async Task<IActionResult> GetAllMeetingsByAdminId(int adminId)
        {
            var meetings = await _meetingService.GetAllMeetingsByAdminId(adminId);
            return Ok(meetings);
        }
        // ------------------------
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
