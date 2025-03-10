using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoriCore.Data;
using CoriCore.Models;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipmentCategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EquipmentCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentCategory>>> GetEquipmentCategories()
        {
            return await _context.EquipmentCategories.ToListAsync();
        }

        // GET: api/EquipmentCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentCategory>> GetEquipmentCategory(int id)
        {
            var equipmentCategory = await _context.EquipmentCategories.FindAsync(id);

            if (equipmentCategory == null)
            {
                return NotFound();
            }

            return equipmentCategory;
        }

        // PUT: api/EquipmentCategory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipmentCategory(int id, EquipmentCategory equipmentCategory)
        {
            if (id != equipmentCategory.EquipmentCatId)
            {
                return BadRequest();
            }

            _context.Entry(equipmentCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentCategoryExists(id))
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

        // POST: api/EquipmentCategory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipmentCategory>> PostEquipmentCategory(EquipmentCategory equipmentCategory)
        {
            _context.EquipmentCategories.Add(equipmentCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipmentCategory", new { id = equipmentCategory.EquipmentCatId }, equipmentCategory);
        }

        // DELETE: api/EquipmentCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipmentCategory(int id)
        {
            var equipmentCategory = await _context.EquipmentCategories.FindAsync(id);
            if (equipmentCategory == null)
            {
                return NotFound();
            }

            _context.EquipmentCategories.Remove(equipmentCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipmentCategoryExists(int id)
        {
            return _context.EquipmentCategories.Any(e => e.EquipmentCatId == id);
        }
    }
}
