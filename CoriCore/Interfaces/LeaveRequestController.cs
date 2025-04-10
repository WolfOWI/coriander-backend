// Iné Smith

using System;
using CoriCore.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace CoriCore.Interfaces
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly IApplyForLeaveService _applyForLeaveService;

        public LeaveRequestController(IApplyForLeaveService applyForLeaveService)
        {
            _applyForLeaveService = applyForLeaveService;
        }

        // POST: api/LeaveRequest/SubmitLeaveRequest
        [HttpPost("SubmitLeaveRequest")]
        public async Task<ActionResult<ApplyForLeaveDTO>> SubmitLeaveRequest([FromBody] ApplyForLeaveDTO leaveRequest)
        {
            // Check if the leaveRequest is null or invalid
            if (leaveRequest == null)
            {
                return BadRequest("Leave request cannot be null");
            }

            // Call the service to submit the leave request and get confirmation or the created leave request
            var result = await _applyForLeaveService.ApplyForLeave(leaveRequest);

            // Return a created response with the leave request details
            return CreatedAtAction(nameof(SubmitLeaveRequest), new { id = result.LeaveRequestId }, result);
        }
    }
}