using Microsoft.AspNetCore.Mvc;
using CoriCore.Interfaces;
using CoriCore.Models;
using CoriCore.DTOs;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authorization;

namespace CoriCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoogleMeetController : ControllerBase
    {
        private readonly IGMeetTokenService _tokenService;
        private readonly IGoogleMeetService _googleMeetService;

        public GoogleMeetController(IGMeetTokenService tokenService, IGoogleMeetService googleMeetService)
        {
            _tokenService = tokenService;
            _googleMeetService = googleMeetService;
        }

        [HttpGet("gmeet-token")]
        public async Task<IActionResult> GetAccessToken()
        {
            var adminId = User.FindFirst("adminId")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized("User is not an admin");

            var token = await _tokenService.GetAccessTokenAsync(int.Parse(adminId));
            return Ok(token);
        }

        [HttpPost("create-event")]
        public async Task<IActionResult> CreateEvent(CreateGMeetEventDTO createGMeetEventDTO)
        {
            var adminId = User.FindFirst("adminId")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized("User is not an admin");

            var response = await _googleMeetService.CreateEventAsync(createGMeetEventDTO, int.Parse(adminId));
            return Ok(response);
        }

        [HttpGet("get-event/{eventId}")]
        public async Task<IActionResult> GetEvent(string eventId)
        {
            var adminId = User.FindFirst("adminId")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized("User is not an admin");

            var response = await _googleMeetService.GetEventByIdAsync(eventId, int.Parse(adminId));
            return Ok(response);
        }

        [HttpPut("update-event/{eventId}")]
        public async Task<IActionResult> UpdateEvent(string eventId, UpdateGMeetEventDTO updateGMeetEventDTO)
        {
            var adminId = User.FindFirst("adminId")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized("User is not an admin");

            var response = await _googleMeetService.UpdateEventAsync(eventId, updateGMeetEventDTO, int.Parse(adminId));
            return Ok(response);
        }

        [HttpDelete("delete-event/{eventId}")]
        public async Task<IActionResult> DeleteEvent(string eventId)
        {
            var adminId = User.FindFirst("adminId")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized("User is not an admin");

            await _googleMeetService.DeleteEventAsync(eventId, int.Parse(adminId));
            return Ok();
        }
    }
} 