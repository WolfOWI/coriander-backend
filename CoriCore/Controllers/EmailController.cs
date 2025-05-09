// Email Controller
// ========================================

using System;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestEmail([FromBody] EmailTestRequest request)
        {
            await _emailService.SendEmailAsync(
                request.RecipientEmail,
                request.Subject,
                request.MessageTitle,
                request.MessageBody
            );

            return Ok("Test email sent successfully.");
        }

        public class EmailTestRequest
        {
            public required string RecipientEmail { get; set; }
            public required string Subject { get; set; }
            public required string MessageTitle { get; set; }
            public required string MessageBody { get; set; }
        }
    }
}
