using CoriCore.DTOs;
using CoriCore.Interfaces;
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
        [HttpGet("ByEmployee/{employeeId}")]
        public async Task<IActionResult> GetMeetingsByEmployeeId(int employeeId)
        {
            var meetings = await _meetingService.GetMeetingsByEmployeeId(employeeId);
            return Ok(meetings);
        }

        [HttpGet("ByAdmin/{adminId}")]
        public async Task<IActionResult> GetMeetingsByAdminId(int adminId)
        {
            var meetings = await _meetingService.GetMeetingsByAdminId(adminId);
            return Ok(meetings);
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
        // ========================================
    }


}
