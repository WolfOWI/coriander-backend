// Iné Smith 

using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpLeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public EmpLeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        // Get all leave requests for employees
        [HttpGet("GetAllEmployeeLeaveRequests")]
        public async Task<ActionResult<List<EmpLeaveRequestDTO>>> GetAllEmployeeLeaveRequests()
        {
            var leaveRequests = await _leaveRequestService.GetAllEmployeeLeaveRequests();
            if (leaveRequests == null || leaveRequests.Count == 0)
            {
                return NotFound("No leave requests found.");
            }
            return Ok(leaveRequests);
        }

        // Approve leave request by Id
        [HttpPut("ApproveLeaveRequestById/{leaveRequestId}")]
        public async Task<IActionResult> ApproveLeaveRequestById(int leaveRequestId)
        {
            var result = await _leaveRequestService.ApproveLeaveRequestById(leaveRequestId);
            if (!result)
            {
                return NotFound($"Leave request with ID {leaveRequestId} not found.");
            }
            return NoContent(); // 204 No Content
        }

        // Reject leave request by Id
        [HttpPut("RejectLeaveRequestById/{leaveRequestId}")]
        public async Task<IActionResult> RejectLeaveRequestById(int leaveRequestId)
        {
            var result = await _leaveRequestService.RejectLeaveRequestById(leaveRequestId);
            if (!result)
            {
                return NotFound($"Leave request with ID {leaveRequestId} not found.");
            }
            return NoContent(); // 204 No Content
        }

        // Set leave request to pending by Id
        [HttpPut("SetLeaveRequestToPendingById/{leaveRequestId}")]
        public async Task<IActionResult> SetLeaveRequestToPendingById(int leaveRequestId)
        {
            var result = await _leaveRequestService.SetLeaveRequestToPendingById(leaveRequestId);
            if (!result)
            {
                return NotFound($"Leave request with ID {leaveRequestId} not found.");
            }
            return NoContent(); // 204 No Content
        }
    }
}

