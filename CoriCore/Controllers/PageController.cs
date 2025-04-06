using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoriCore.DTOs.Page_Specific;
using CoriCore.Interfaces;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// Gets all the information needed for the Admin Employee Details page
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <returns>An AdminEmpDetailsPageDTO containing all necessary information</returns>
        [HttpGet("admin-emp-details/{employeeId}")]
        public async Task<ActionResult<AdminEmpDetailsPageDTO>> GetAdminEmpDetailsPage(int employeeId)
        {
            try
            {
                var pageInfo = await _pageService.GetAdminEmpDetailsPageInfo(employeeId);
                return Ok(pageInfo);
            }
            catch (Exception ex)
            {
                return NotFound($"Error retrieving admin employee details: {ex.Message}");
            }
        }
    }
} 