// Leave Request Controller
// ========================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Interfaces;
using CoriCore.DTOs;

namespace CoriCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IApplyForLeaveService _applyForLeaveService;

        public LeaveRequestController(AppDbContext context, ILeaveRequestService leaveRequestService, IApplyForLeaveService applyForLeaveService)
        {
            _leaveRequestService = leaveRequestService;
            _context = context;
            _applyForLeaveService = applyForLeaveService;
        }

        // GET: api/LeaveRequest/EmployeeId
        [HttpGet("EmployeeId/{employeeId}")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetLeaveRequestsByEmployeeId(int employeeId) // IEmumerable = List
        {
            var leaveRequests = await _leaveRequestService.GetLeaveRequestsByEmployeeId(employeeId);

            if (leaveRequests == null || !leaveRequests.Any())
            {
                return NotFound();
            }

            return Ok(leaveRequests);
        }


        // GET: api/LeaveRequest
        //Display all leave requests using the leaveRequestDTO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDTO>>> GetAllLeaveRequests()
        {
            var leaveRequests = await _leaveRequestService.GetAllLeaveRequests();

            if (leaveRequests == null || !leaveRequests.Any())
            {
                return NotFound();
            }

            return Ok(leaveRequests);
        }

        // GET: api/LeaveRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return leaveRequest;
        }

        // PUT: api/LeaveRequest/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveRequestId)
            {
                return BadRequest();
            }

            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LeaveRequest
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaveRequest", new { id = leaveRequest.LeaveRequestId }, leaveRequest);
        }

        // POST: api/LeaveRequest/SubmitLeaveRequest
        [HttpPost("SubmitLeaveRequest")]
        public async Task<ActionResult<LeaveRequestDTO>> SubmitLeaveRequest([FromBody] ApplyForLeaveDTO leaveRequest)
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

        // DELETE: api/LeaveRequest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.LeaveRequestId == id);
        }
    }
}
