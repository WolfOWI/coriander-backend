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
    public class EquipmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(AppDbContext context, IEquipmentService equipmentService)
        {
            _context = context;
            _equipmentService = equipmentService;
        }

        // GET: api/Equipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()
        {
            return await _context.Equipments.ToListAsync();
        }

        // GET: api/Equipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetEquipment(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            return equipment;
        }

        // PUT: api/Equipment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutEquipment(int id, Equipment equipment)
        // {
        //     if (id != equipment.EquipmentId)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(equipment).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!EquipmentExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: api/Equipment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<Equipment>> PostEquipment(Equipment equipment)
        // {
        //     _context.Equipments.Add(equipment);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetEquipment", new { id = equipment.EquipmentId }, equipment);
        // }

        // DELETE: api/Equipment/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteEquipment(int id)
        // {
        //     var equipment = await _context.Equipments.FindAsync(id);
        //     if (equipment == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Equipments.Remove(equipment);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool EquipmentExists(int id)
        {
            return _context.Equipments.Any(e => e.EquipmentId == id);
        }

        /// <summary>
        /// Gets all equipment assigned to a specific employee
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <returns>A list of equipment assigned to the employee</returns>
        [HttpGet("by-empId/{employeeId}")]
        public async Task<ActionResult<List<EquipmentDTO>>> GetEquipmentByEmployeeId(int employeeId)
        {
            var equipment = await _equipmentService.GetEquipmentByEmployeeId(employeeId);
            return Ok(equipment);
        }
    }
}
