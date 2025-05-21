// Leave Balance Controller
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

    }
}
