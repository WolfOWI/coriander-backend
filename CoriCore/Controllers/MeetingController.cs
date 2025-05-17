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

        [HttpPost("CreateEmployeeRequest")]
        public async Task<IActionResult> CreateEmployeeMeetingRequest([FromBody] MeetingRequestCreateDTO meetingRequestCreateDTO)
        {
            var meetingRequest = await _meetingService.CreateMeetingRequest(meetingRequestCreateDTO);
            return Ok(meetingRequest);
        }
    }
}
