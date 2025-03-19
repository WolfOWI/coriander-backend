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

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

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

        // PUT: api/Employee/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employee
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }


        // Creating an Employee
        // [HttpPost("CreateEmployeeFull")]
        // public async Task<ActionResult<Employee>> CreateEmployeeFull(CreateEmployeeDTO dto)
        // {
        //     // Step 1: Check if inputted user exists
        //     // Step 2: Create the employee
        //     // Step 3: Add the pay cycle
        //     // Step 4: Add Equipment / Create Equipment if specified
        //     // 1. Create the PayCycle
        //     var payCycle = new PayCycle
        //     {
        //         PayCycleName = dto.PayCycleName,
        //         CycleDays = dto.CycleDays
        //     };

        //     _context.PayCycles.Add(payCycle);
        //     await _context.SaveChangesAsync();

        //     // 2. Compute the NextPayday.
        //     // For this example, we simply add CycleDays to the EmployDate.
        //     // You can modify this logic (e.g., first day of next month, end of month, etc.)
        //     DateOnly nextPayday = dto.EmployDate.AddDays(dto.CycleDays);

        //     // 3. Create the new Employee with computed/default values
        //     var employee = new Employee
        //     {
        //         UserId = dto.UserId,
        //         Gender = dto.Gender,
        //         DateOfBirth = dto.DateOfBirth,
        //         PhoneNumber = dto.PhoneNumber,
        //         JobTitle = dto.JobTitle,
        //         Department = dto.Department,
        //         SalaryAmount = dto.SalaryAmount,
        //         PayCycleId = payCycle.PayCycleId,
        //         PayCycle = payCycle,
        //         PastPayday = null, // Default: null
        //         PastPaydayIsPaid = false, // Default: false
        //         NextPayday = nextPayday,
        //         EmployType = dto.EmployType,
        //         EmployDate = dto.EmployDate,
        //         IsSuspended = dto.IsSuspended,
        //         SuspensionEndDate = dto.SuspensionEndDate
        //     };

        //     _context.Employees.Add(employee);
        //     await _context.SaveChangesAsync();

        //     // 4. Create Equipment records if provided
        //     if (dto.Equipment != null && dto.Equipment.Any())
        //     {
        //         foreach (var eqDto in dto.Equipment)
        //         {
        //             var equipment = new Equipment
        //             {
        //                 EmployeeId = employee.EmployeeId,
        //                 EquipmentCatId = eqDto.EquipmentCatId,
        //                 // Assign the current date/time; adjust as needed.
        //                 AssignedDate = DateTime.Now,
        //                 Condition = eqDto.Condition
        //             };
        //             _context.Equipments.Add(equipment);
        //         }
        //         await _context.SaveChangesAsync();
        //     }

        //     return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        // }
    }
}
