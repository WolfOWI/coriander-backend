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
        [HttpGet("all-by-empId/{employeeId}")]
        public async Task<ActionResult<IEnumerable<GatheringDTO>>> GetAllGatheringsByEmpId(int employeeId)
        {
            var gatherings = await _gatheringService.GetAllGatheringsByEmployeeId(employeeId);
            return Ok(gatherings);
        }

        [HttpGet("upcoming-by-empId/{employeeId}")]
        public async Task<ActionResult<IEnumerable<GatheringDTO>>> GetAllUpcomingGatheringsByEmpId(int employeeId)
        {
            var gatherings = await _gatheringService.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Upcoming");
            return Ok(gatherings);
        }

        [HttpGet("completed-by-empId/{employeeId}")]
        public async Task<ActionResult<IEnumerable<GatheringDTO>>> GetAllCompletedGatheringsByEmpId(int employeeId)
        {
            var gatherings = await _gatheringService.GetAllGatheringsByEmployeeIdAndStatus(employeeId, "Completed");
            return Ok(gatherings);
        }


        [HttpGet("upcoming-by-adminId/{adminId}")]
        public async Task<ActionResult<IEnumerable<GatheringDTO>>> GetAllUpcomingGatheringsByAdminId(int adminId)
        {
            var gatherings = await _gatheringService.GetAllGatheringsByAdminIdAndStatus(adminId, "Upcoming");
            return Ok(gatherings);
        }

        [HttpGet("completed-by-adminId/{adminId}")]
        public async Task<ActionResult<IEnumerable<GatheringDTO>>> GetAllCompletedGatheringsByAdminId(int adminId)
        {
            var gatherings = await _gatheringService.GetAllGatheringsByAdminIdAndStatus(adminId, "Completed");
            return Ok(gatherings);
        }
    }
}
