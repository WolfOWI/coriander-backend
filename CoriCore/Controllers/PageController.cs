// Page Controller
// ========================================

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

        /// <summary>
        /// Gets all the information needed for the Admin Employee Management page
        /// </summary>
        /// <returns>A list of AdminEmpManagePageListItemDTO containing all necessary information</returns>
        [HttpGet("admin-emp-management")]
        public async Task<ActionResult<List<AdminEmpManagePageListItemDTO>>> GetAdminEmpManagementPage()
        {
            try
            {
                var pageInfo = await _pageService.GetAdminEmpManagementPageInfo();
                return Ok(pageInfo);
            }
            catch (Exception ex)
            {
                return NotFound($"Error retrieving admin employee management page: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Gets all the information needed for the Employee Profile page
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <returns>An EmployeeProfilePageDTO containing all necessary information</returns>
        [HttpGet("employee-profile/{employeeId}")]
        public async Task<ActionResult<EmployeeProfilePageDTO>> GetEmployeeProfilePage(int employeeId)
        {
            try
            {
                var pageInfo = await _pageService.GetEmployeeProfilePageInfo(employeeId);
                return Ok(pageInfo);
            }
            catch (Exception ex)
            {
                return NotFound($"Error retrieving employee profile page: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all the information needed for the Admin Dashboard page
        /// </summary>
        /// <returns>An AdminDashboardPageDTO containing all necessary information</returns>
        [HttpGet("admin-dashboard/{adminId}")]
        public async Task<ActionResult<AdminDashboardPageDTO>> GetAdminDashboardPage(int adminId)
        {
            try
            {
                var pageInfo = await _pageService.GetAdminDashboardPageInfo(adminId);
                return Ok(pageInfo);
            }
            catch (Exception ex)
            {
                return NotFound($"Error retrieving admin dashboard page: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all the information needed for the Employee Leave Overview page
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <returns>An EmployeeLeaveOverviewPageDTO containing all necessary information</returns>
        [HttpGet("employee-leave-overview/{employeeId}")]   
        public async Task<ActionResult<EmployeeLeaveOverviewPageDTO>> GetEmployeeLeaveOverviewPage(int employeeId)
        {
            try
            {
                var pageInfo = await _pageService.GetEmployeeLeaveOverviewPageInfo(employeeId);
                return Ok(pageInfo);
            }
            catch (Exception ex)
            {
                return NotFound($"Error retrieving employee leave overview page: {ex.Message}");
            }
        }
    }
} 