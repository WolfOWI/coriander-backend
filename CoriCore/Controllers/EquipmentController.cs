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

        // Assign equipment to employee
        [HttpPost("assign-equipment")]
        public async Task<IActionResult> AssignEquipmentToEmployee(int employeeId, List<int> equipmentIds)
        {
            var result = await _equipmentService.AssignEquipmentAsync(employeeId, equipmentIds);
            return StatusCode(result.Code, result.Message);
        }

        // Unlink equipment from employee
        [HttpPut("unlink/{equipmentId}")]
        public async Task<IActionResult> UnlinkEquipmentFromEmployee(int equipmentId)
        {
            var result = await _equipmentService.UnlinkEquipmentFromEmployee(equipmentId);
            return StatusCode(result.Code, result.Message);
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

        

    }
}
