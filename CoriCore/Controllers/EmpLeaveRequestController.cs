// Iné Smith 

using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpLeaveRequestController : ControllerBase
    {
        private readonly IEmpLeaveRequestService _leaveRequestService;

        public EmpLeaveRequestController(IEmpLeaveRequestService leaveRequestService)
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

        // Get pending leave requests
        [HttpGet("GetAllPending")]
        public async Task<ActionResult<List<EmpLeaveRequestDTO>>> GetPendingLeaveRequests()
        {
            var pendingLeaveRequests = await _leaveRequestService.GetPendingLeaveRequests();
            return Ok(pendingLeaveRequests);
        }

        // Get approved leave requests
        [HttpGet("GetAllApproved")]
        public async Task<ActionResult<List<EmpLeaveRequestDTO>>> GetApprovedLeaveRequests()
        {
            var approvedLeaveRequests = await _leaveRequestService.GetApprovedLeaveRequests();
            return Ok(approvedLeaveRequests);
        }

        // Get rejected leave requests
        [HttpGet("GetAllRejected")]
        public async Task<ActionResult<List<EmpLeaveRequestDTO>>> GetRejectedLeaveRequests()
        {
            var rejectedLeaveRequests = await _leaveRequestService.GetRejectedLeaveRequests();
            return Ok(rejectedLeaveRequests);
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

