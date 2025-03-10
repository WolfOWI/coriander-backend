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
    public class PayCycleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PayCycleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PayCycle
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayCycle>>> GetPayCycles()
        {
            return await _context.PayCycles.ToListAsync();
        }

        // GET: api/PayCycle/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PayCycle>> GetPayCycle(int id)
        {
            var payCycle = await _context.PayCycles.FindAsync(id);

            if (payCycle == null)
            {
                return NotFound();
            }

            return payCycle;
        }

        // PUT: api/PayCycle/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayCycle(int id, PayCycle payCycle)
        {
            if (id != payCycle.PayCycleId)
            {
                return BadRequest();
            }

            _context.Entry(payCycle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayCycleExists(id))
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

        // POST: api/PayCycle
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PayCycle>> PostPayCycle(PayCycle payCycle)
        {
            _context.PayCycles.Add(payCycle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayCycle", new { id = payCycle.PayCycleId }, payCycle);
        }

        // DELETE: api/PayCycle/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayCycle(int id)
        {
            var payCycle = await _context.PayCycles.FindAsync(id);
            if (payCycle == null)
            {
                return NotFound();
            }

            _context.PayCycles.Remove(payCycle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayCycleExists(int id)
        {
            return _context.PayCycles.Any(e => e.PayCycleId == id);
        }
    }
}
