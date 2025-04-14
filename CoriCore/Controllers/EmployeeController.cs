// Employee Controller
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
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(AppDbContext context, IEmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        // POST: api/Employee/setup-user-as-employee
        [HttpPost("setup-user-as-employee")]
        public async Task<IActionResult> SetupUserAsEmployee([FromBody] EmployeeDto employeeDto)
        {
            var result = await _employeeService.RegisterEmployeeAsync(employeeDto);
            return StatusCode(result.Code, new { result.Message });
        }

        // POST: api/Employee/suspension-toggle/{employeeId}
        [HttpPost("suspension-toggle/{employeeId}")]
        public async Task<IActionResult> ToggleEmpSuspension(int employeeId)
        {
            var result = await _employeeService.ToggleEmpSuspensionAsync(employeeId);
            return StatusCode(result.Code, new { result.Message });
        }

        // AUTOGENERATED:
        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }


        // TODO: Remove this endpoint (will form part of EmpUser Deletion)
        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEmployeeByIdAsync(id);
            return StatusCode(result.Code, new { result.Message });
        }


        // GET: api/Employee/status-totals
        /// <summary>
        /// Get the total number of employees, and the totals of each employement status (including suspended)
        /// </summary>
        /// <returns>The total number of employees, and the totals of each employement status</returns>
        [HttpGet("status-totals")]
        public async Task<ActionResult<EmpTotalStatsDTO>> GetEmployeeStatusTotals()
        {
            var result = await _employeeService.GetEmployeeStatusTotals();
            return Ok(result);
        }

        

        // POST: api/Employee
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        // {
        //     _context.Employees.Add(employee);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        // }

        // DELETE: api/Employee/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteEmployee(int id)
        // {
        //     var employee = await _context.Employees.FindAsync(id);
        //     if (employee == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Employees.Remove(employee);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        

        // GET: api/Employee/nextpayday/{payCycle}
        // [HttpGet("nextpayday/{payCycle}")]
        // public async Task<IActionResult> CalculateNextPayday(int payCycle)
        // {
        //     var result = await _employeeService.CalculateNextPayDayAsync(payCycle);
        //     return StatusCode(result.Code, new { result.Message, result.NextPayDay });
        // }
    }

}
