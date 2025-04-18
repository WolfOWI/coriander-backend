// Equipment Controller
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
    public class EquipmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(AppDbContext context, IEquipmentService equipmentService)
        {
            _context = context;
            _equipmentService = equipmentService;
        }

        /// <summary>
        /// Gets all equipment items (both assigned and unassigned) with additional info about user/employee (if assigned)
        /// </summary>
        /// <returns>A list of equipment DTOs</returns>
        [HttpGet]
        public async Task<ActionResult<List<EmpEquipItemDTO>>> GetAllEquipItems()
        {   
            var assignedItems = await _equipmentService.GetAllAssignedEquipItems();
            var unassignedItems = await _equipmentService.GetAllUnassignedEquipItems();

            // Turn unassignedItems into a list of EmpEquipItemDTO
            var unassignedItemsList = unassignedItems.Select(item => new EmpEquipItemDTO
            {
                Equipment = item,
                FullName = null,
                ProfilePicture = null,
                NumberOfItems = null
            }).ToList();

            // Add assignedItems and unassignedItemsList to allItem
            var allItems = new List<EmpEquipItemDTO>();
            allItems.AddRange(unassignedItemsList);
            allItems.AddRange(assignedItems);

            return Ok(allItems);
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

        // GET: api/Equipment
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()
        // {
        //     return await _context.Equipments.ToListAsync();
        // }

        // GET: api/Equipment/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<Equipment>> GetEquipment(int id)
        // {
        //     var equipment = await _context.Equipments.FindAsync(id);

        //     if (equipment == null)
        //     {
        //         return NotFound();
        //     }

        //     return equipment;
        // }

        // POST
        [HttpPost("CreateEquipmentItems")]
        public async Task<ActionResult<IEnumerable<EquipmentDTO>>> CreateEquipmentItems([FromBody] List<CreateEquipmentDTO> equipmentDtos)
        {
            if (equipmentDtos == null || !equipmentDtos.Any())
            {
                return BadRequest("No equipment provided.");
            }

            // Call service to create the equipment items
            var equipmentItems = await _equipmentService.CreateEquipmentItemsAsync(equipmentDtos);

            // Load the equipment categories for the created items
            var equipmentItemsWithCategories = await _context.Equipments
            .Include(e => e.EquipmentCategory) // Include the equipment category
            .Where(e => equipmentItems.Select(ei => ei.EquipmentId).Contains(e.EquipmentId)) // Filter by the created items
            .ToListAsync();

            // Map back to EquipmentDTO to return the created items
            var equipmentDtosCreated = equipmentItemsWithCategories.Select(e => new EquipmentDTO
            {
                EquipmentId = e.EquipmentId,
                EmployeeId = e.EmployeeId ?? 0,
                EquipmentCatId = e.EquipmentCatId,
                EquipmentCategoryName = e.EquipmentCategory.EquipmentCatName, // Fixed property name
                EquipmentName = e.EquipmentName,
                AssignedDate = e.AssignedDate,
                Condition = e.Condition
            }).ToList();

            // Return the created equipment items as a response
            return Ok(new
            {
                Count = equipmentDtosCreated.Count,
                Data = equipmentDtosCreated
            });

        }

        // PUT: api/Equipment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEquipmentItem(int id, [FromBody] EquipmentDTO equipmentDto)
        {
            if (id != equipmentDto.EquipmentId)
            {
                return BadRequest("Equipment ID mismatch.");
            }

            // Call service to edit the equipment item
            var updatedEquipment = await _equipmentService.EditEquipmentItemAsync(id, equipmentDto);

            if (updatedEquipment == null)
            {
                return NotFound("Equipment not found.");
            }

            return Ok(updatedEquipment);
        }

        //Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipmentItem(int id)
        {
            var deleted = await _equipmentService.DeleteEquipmentItemAsync(id);

            if (!deleted)
            {
                return NotFound("Equipment not found.");
            }

            return NoContent();
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


    }
}
