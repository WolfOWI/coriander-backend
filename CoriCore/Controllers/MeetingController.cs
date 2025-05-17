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

        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateEmployeeMeetingRequest([FromBody] MeetingRequestCreateDTO meetingRequestCreateDTO)
        {
            var meetingRequest = await _meetingService.CreateMeetingRequest(meetingRequestCreateDTO);
            return Ok(meetingRequest);
        }

        [HttpPut("ConfirmAndUpdate/{meetingId}")]
        public async Task<IActionResult> ConfirmAndUpdateMeetingRequest(int meetingId, [FromBody] MeetingConfirmDTO meetingConfirmDTO)
        {
            var (code, message) = await _meetingService.ConfirmAndUpdateMeetingRequest(meetingId, meetingConfirmDTO);
            return StatusCode(code, message);
        }
    }


}
