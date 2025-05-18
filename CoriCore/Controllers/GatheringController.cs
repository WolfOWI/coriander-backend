using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoriCore.Interfaces;
using CoriCore.DTOs;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatheringController : ControllerBase
    {
        private readonly IGatheringService _gatheringService;

        public GatheringController(IGatheringService gatheringService)
        {
            _gatheringService = gatheringService;
        }

        /// <summary>
        /// Get all meetings and performance reviews for an employee, sorted by start date
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <returns>List of gatherings sorted by start date</returns>
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<GatheringDTO>>> GetAllGatheringsByEmployeeId(int employeeId)
        {
            var gatherings = await _gatheringService.GetAllGatheringsByEmployeeId(employeeId);
            return Ok(gatherings);
        }

       
    }
}
