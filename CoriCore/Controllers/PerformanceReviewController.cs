// Performance Review Controller
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
using CoriCore.Interfaces;
using CoriCore.DTOs;
using CoriCore.Services;

namespace CoriCore.Controllers
{   
    //Makes it a RESTful API controller
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

        //------------------------------------------------------

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
                Status = pr.Status
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
                Status = pr.Status
            });

            return Ok(reviewDTOs);
        }

        // Get EmpUserRatingMetrics
        [HttpGet("EmpUserRatingMetrics")]
        public async Task<IActionResult> GetAllEmpUserRatingMetrics()
        {
            var metrics = await _PerformanceReviewService.GetAllEmpUserRatingMetrics();
            return Ok(metrics);
        }

        // Get random EmpUserRatingMetrics
        [HttpGet("RandomEmpUserRatingMetrics/{numberOfEmps}")]
        public async Task<IActionResult> GetRandomEmpUserRatingMetricsByNum(int numberOfEmps)
        {
            try
            {
                var metrics = await _PerformanceReviewService.GetRandomEmpUserRatingMetricsByNum(numberOfEmps);
                return Ok(metrics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get EmpUserRatingMetrics by Employee ID
        [HttpGet("EmpUserRatingMetrics/{employeeId}")]
        public async Task<IActionResult> GetEmpUserRatingMetricsByEmpId(int employeeId)
        {
            var metrics = await _PerformanceReviewService.GetEmpUserRatingMetricsByEmpId(employeeId);
            
            if (metrics == null)
            {
                return NotFound($"No rating metrics found for Employee ID {employeeId}.");
            }

            return Ok(metrics);
        }

        //POST Performance Review
        //Using the PostPerformanceReviewDTO and setting status to 1 (Upcoming)
        [HttpPost("CreatePerformanceReview")]
        public async Task<IActionResult> CreatePerformanceReview([FromBody] PostPerformanceReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
            {
                return BadRequest("Invalid performance review data.");
            }

            // Map the DTO to the PerformanceReview entity
            var performanceReview = new PerformanceReview
            {
                AdminId = reviewDTO.AdminId,
                EmployeeId = reviewDTO.EmployeeId,
                IsOnline = reviewDTO.IsOnline,
                MeetLocation = reviewDTO.MeetLocation,
                MeetLink = reviewDTO.MeetLink,
                StartDate = reviewDTO.StartDate,
                EndDate = reviewDTO.EndDate,
                Rating = reviewDTO.Rating,
                Comment = reviewDTO.Comment,
                DocUrl = reviewDTO.DocUrl,
                Status = ReviewStatus.Upcoming // Set status to Upcoming (1)
            };

            // Create the performance review in the database
            await _PerformanceReviewService.CreatePerformanceReview(performanceReview);

            return CreatedAtAction(nameof(GetPerformanceReview), new { id = performanceReview.ReviewId }, performanceReview);
        }


        //Update Performance Review using PostPerformanceReviewDTO
        [HttpPut("UpdatePerformanceReview/{id}")]
        public async Task<IActionResult> UpdatePerformanceReview(int id, [FromBody] PostPerformanceReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
            {
                return BadRequest("Invalid performance review data.");
            }

            // Map the DTO to the PerformanceReview entity
            var performanceReview = new PerformanceReview
            {
                ReviewId = id,
                AdminId = reviewDTO.AdminId,
                EmployeeId = reviewDTO.EmployeeId,
                IsOnline = reviewDTO.IsOnline,
                MeetLocation = reviewDTO.MeetLocation,
                MeetLink = reviewDTO.MeetLink,
                StartDate = reviewDTO.StartDate,
                EndDate = reviewDTO.EndDate,
                Rating = reviewDTO.Rating,
                Comment = reviewDTO.Comment,
                DocUrl = reviewDTO.DocUrl,
                Status = ReviewStatus.Upcoming // Set status to Upcoming (1) or keep the existing status
            };

            // Update the performance review in the database
            await _PerformanceReviewService.UpdatePerformanceReview(id, performanceReview);

            return NoContent();
        }
        

        //Get All the upcoming Performance Reviews with status 1 (Upcoming)
        // and returns it as a list of PerformanceReviewDTO objects.
        [HttpGet("GetAllUpcomingPrm")]
        public async Task<IActionResult> GetAllUpcomingPrm()
        {
            var reviews = await _PerformanceReviewService.GetAllUpcomingPrm() as IEnumerable<PerformanceReview>;

            if (reviews == null || !reviews.Any())
            {
                return NotFound("No upcoming performance reviews found.");
            }

            var reviewDTOs = reviews.Select(review => new PerformanceReviewDTO
            {
                ReviewId = review.ReviewId,
                AdminId = review.AdminId,
                AdminName = review.Admin.User.FullName, // Fetch Admin Name
                EmployeeId = review.EmployeeId,
                EmployeeName = review.Employee.User.FullName, // Fetch Employee Name
                IsOnline = review.IsOnline,
                MeetLocation = review.MeetLocation,
                MeetLink = review.MeetLink,
                StartDate = review.StartDate,
                EndDate = review.EndDate,
                Rating = review.Rating,
                Comment = review.Comment,
                DocUrl = review.DocUrl,
                Status = review.Status
            });

            return Ok(reviewDTOs);
        }

        [HttpGet("top-rated")]
        public async Task<ActionResult<List<EmpUserRatingMetricsDTO>>> GetTopRatedEmployees()
        {
            var topRatedEmployees = await _PerformanceReviewService.GetTopRatedEmployees();
            if (topRatedEmployees == null || !topRatedEmployees.Any())
            {
                return NotFound("No top-rated employees found.");
            }
            return Ok(topRatedEmployees);
        }
        
        [HttpPut("update-status/{id}")]
        public async Task<ActionResult<PerformanceReview>> UpdateReviewStatus(int id, [FromQuery] ReviewStatus status)
        {
            try
            {
                var updatedReview = await _PerformanceReviewService.UpdateReviewStatus(id, status);
                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        

        [HttpDelete("DeletePerformanceReview/{id}")]
        public async Task<IActionResult> DeletePerformanceReview(int id)
        {
            var deleted = await _PerformanceReviewService.DeletePerformanceReview(id);
            if (!deleted)
            {
                return NotFound($"Successful - Performance review with ID {id} not found.");
            }

            return NoContent();
        }

        [HttpDelete("DeletePrmByEmpId/{employeeId}")]
        public async Task<IActionResult> DeletePrmByEmpId(int employeeId)
        {
            var deleted = await _PerformanceReviewService.DeletePrmByEmpId(employeeId);
            if (!deleted)
            {
                return NotFound($"No Performance reviews for Employee ID {employeeId} not found.");
            }

            return NoContent();
        }




        // AUTOGENERATED: -------------------------------------
        // PUT: api/PerformanceReview/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutPerformanceReview(int id, PerformanceReview performanceReview)
        // {
        //     if (id != performanceReview.ReviewId)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(performanceReview).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!PerformanceReviewExists(id))
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

        // POST: api/PerformanceReview
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<PerformanceReview>> PostPerformanceReview(PerformanceReview performanceReview)
        // {
        //     _context.PerformanceReviews.Add(performanceReview);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetPerformanceReview", new { id = performanceReview.ReviewId }, performanceReview);
        // }

        // DELETE: api/PerformanceReview/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeletePerformanceReview(int id)
        // {
        //     var performanceReview = await _context.PerformanceReviews.FindAsync(id);
        //     if (performanceReview == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.PerformanceReviews.Remove(performanceReview);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool PerformanceReviewExists(int id)
        {
            return _context.PerformanceReviews.Any(e => e.ReviewId == id);
        }
    }
}
