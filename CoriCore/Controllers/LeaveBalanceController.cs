using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.DTOs;
using CoriCore.Interfaces;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILeaveBalanceService _leaveBalanceService;
        public LeaveBalanceController(AppDbContext context, ILeaveBalanceService leaveBalanceService)
        {
            _context = context;
            _leaveBalanceService = leaveBalanceService;
        }

        /// <summary>
        /// Get all leave balances (with their types) by employee id
        /// </summary>
        /// <param name="employeeId">The id of the employee</param>
        /// <returns>A list of leave balances</returns>
        // GET: api/LeaveBalance/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<List<LeaveBalanceDTO>>> GetLeaveBalancesByEmployeeId(int employeeId)
        {
            var leaveBalances = await _leaveBalanceService.GetAllLeaveBalancesByEmployeeId(employeeId);
            return Ok(leaveBalances);
        }

        // AUTO GENERATED ENDPOINTS BELOW:

        // GET: api/LeaveBalance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveBalance>>> GetLeaveBalances()
        {
            return await _context.LeaveBalances.ToListAsync();
        }

        // GET: api/LeaveBalance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveBalance>> GetLeaveBalance(int id)
        {
            var leaveBalance = await _context.LeaveBalances.FindAsync(id);

            if (leaveBalance == null)
            {
                return NotFound();
            }

            return leaveBalance;
        }

        // PUT: api/LeaveBalance/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveBalance(int id, LeaveBalance leaveBalance)
        {
            if (id != leaveBalance.LeaveBalanceId)
            {
                return BadRequest();
            }

            _context.Entry(leaveBalance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveBalanceExists(id))
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

        // POST: api/LeaveBalance
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LeaveBalance>> PostLeaveBalance(LeaveBalance leaveBalance)
        {
            _context.LeaveBalances.Add(leaveBalance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaveBalance", new { id = leaveBalance.LeaveBalanceId }, leaveBalance);
        }

        // DELETE: api/LeaveBalance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveBalance(int id)
        {
            var leaveBalance = await _context.LeaveBalances.FindAsync(id);
            if (leaveBalance == null)
            {
                return NotFound();
            }

            _context.LeaveBalances.Remove(leaveBalance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveBalanceExists(int id)
        {
            return _context.LeaveBalances.Any(e => e.LeaveBalanceId == id);
        }
    }
}
