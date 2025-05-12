using Microsoft.AspNetCore.Mvc;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CoriCore.Models;

namespace CoriCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GMeetAuthController : ControllerBase
    {
        private readonly GoogleMeetOptions _options;
        private readonly IGMeetTokenService _tokenService;

        public GMeetAuthController(IOptions<GoogleMeetOptions> options, IGMeetTokenService tokenService)
        {
            _options = options.Value;
            _tokenService = tokenService;
        }

        [HttpGet("gmeet-auth")]
        public IActionResult Authorise()
        {
            var adminId = User.FindFirst("adminId")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized("User is not an admin");

            var url = "https://accounts.google.com/o/oauth2/v2/auth?" +
                        $"scope={_options.Scope}" +
                        $"&access_type=offline" +
                        $"&response_type=code" +
                        $"&state={adminId}" +
                        $"&redirect_uri={_options.RedirectUrl}" +
                        $"&client_id={_options.ClientId}";
            
            return Ok(new { authUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            if (!int.TryParse(state, out int adminId))
                return BadRequest("Invalid state parameter");

            var token = await _tokenService.GetTokenAsync(code, adminId);
            return Ok(new { message = "Google Meet authorization successful", token });
        }
    }
} 