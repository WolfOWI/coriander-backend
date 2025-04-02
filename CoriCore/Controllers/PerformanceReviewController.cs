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
using static CoriCore.DTOs.PerformaceReviewDTO;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceReviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPerformanceReviewService _PerformanceReviewService;

        public PerformanceReviewController(AppDbContext context, IPerformanceReviewService PerformanceReviewService)
        {
            _context = context;
            _PerformanceReviewService = PerformanceReviewService;
        }

        // GET: api/PerformanceReview
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerformanceReview>>> GetPerformanceReviews()
        {
            return await _context.PerformanceReviews.ToListAsync();
        }

        // GET: api/PerformanceReview/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PerformanceReview>> GetPerformanceReview(int id)
        {
            var performanceReview = await _context.PerformanceReviews.FindAsync(id);

            if (performanceReview == null)
            {
                return NotFound();
            }

            return performanceReview;
        }

        // GET: api/PerformanceReviewByStartDateAdminId
        [HttpGet("GetPrmByStartDateAdminId/{adminId}/{startDate}")]
        public async Task<IActionResult> GetPrmByStartDateAdminId(int adminId, DateTime startDate)
        {
            var reviews = await _PerformanceReviewService.GetPrmByStartDateAdminId(adminId, startDate);

            if (reviews == null || !reviews.Any())
            {
                return NotFound("No Performance Reviews found for the given Admin and Start Date.");
            }

            var reviewDTOs = reviews.Select(pr => new PerformanceReviewDTO
            {
                ReviewId = pr.ReviewId,
                AdminId = pr.AdminId,
                AdminName = pr.Admin.User.FullName, // Fetch Admin Name
                EmployeeId = pr.EmployeeId,
                EmployeeName = pr.Employee.User.FullName, // Fetch Employee Name
                IsOnline = pr.IsOnline,
                MeetLocation = pr.MeetLocation,
                MeetLink = pr.MeetLink,
                StartDate = pr.StartDate,
                EndDate = pr.EndDate,
                Rating = pr.Rating,
                Comment = pr.Comment,
                DocUrl = pr.DocUrl,
                Status = (DTOs.Status)pr.Status
            });

            return Ok(reviewDTOs);
        }

        //Get Performance Review By Employee Id
        [HttpGet("GetPrmByEmpId/{employeeId}")]
        public async Task<IActionResult> GetPrmByEmpId(int employeeId)
        {
            var reviews = await _PerformanceReviewService.GetPrmByEmpId(employeeId);

            if (reviews == null || !reviews.Any())
            {
                return NotFound($"No Performance Reviews found for Employee ID {employeeId}.");
            }

            var reviewDTOs = reviews.Select(pr => new PerformanceReviewDTO
            {
                ReviewId = pr.ReviewId,
                AdminId = pr.AdminId,
                AdminName = pr.Admin.User.FullName, // Fetch Admin Name
                EmployeeId = pr.EmployeeId,
                EmployeeName = pr.Employee.User.FullName, // Fetch Employee Name
                IsOnline = pr.IsOnline,
                MeetLocation = pr.MeetLocation,
                MeetLink = pr.MeetLink,
                StartDate = pr.StartDate,
                EndDate = pr.EndDate,
                Rating = pr.Rating,
                Comment = pr.Comment,
                DocUrl = pr.DocUrl,
                Status = (DTOs.Status)pr.Status
            });

            return Ok(reviewDTOs);
        }

        // PUT: api/PerformanceReview/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerformanceReview(int id, PerformanceReview performanceReview)
        {
            if (id != performanceReview.ReviewId)
            {
                return BadRequest();
            }

            _context.Entry(performanceReview).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformanceReviewExists(id))
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

        // POST: api/PerformanceReview
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PerformanceReview>> PostPerformanceReview(PerformanceReview performanceReview)
        {
            _context.PerformanceReviews.Add(performanceReview);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerformanceReview", new { id = performanceReview.ReviewId }, performanceReview);
        }

        // DELETE: api/PerformanceReview/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerformanceReview(int id)
        {
            var performanceReview = await _context.PerformanceReviews.FindAsync(id);
            if (performanceReview == null)
            {
                return NotFound();
            }

            _context.PerformanceReviews.Remove(performanceReview);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PerformanceReviewExists(int id)
        {
            return _context.PerformanceReviews.Any(e => e.ReviewId == id);
        }
    }
}
