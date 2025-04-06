using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpUserController : ControllerBase
    {

        // Dependency Injection
        private readonly AppDbContext _context;
        private readonly IEmpUserService _empUserService;

        public EmpUserController(AppDbContext context, IEmpUserService empUserService)
        {
            _context = context;
            _empUserService = empUserService;
        }

        /// <summary>
        /// Get all user employees and their information
        /// </summary>
        /// <returns>List of EmpUserDTO</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpUserDTO>>> GetAllEmpUsers()
        {
            var empUsers = await _empUserService.GetAllEmpUsers();
            return Ok(empUsers);
        }

        /// <summary>
        /// Get an user employee by their ID
        /// </summary>
        /// <param name="id">The ID of the user employee to get</param>
        /// <returns>An EmpUserDTO</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpUserDTO>> GetEmpUserById(int id)
        {
            try
            {
                var empUser = await _empUserService.GetEmpUserByEmpId(id);
                return Ok(empUser);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/EmpUser/edit-by-id/{id}
        [HttpPut("edit-by-id/{id}")]
        public async Task<IActionResult> EditEmpUserDetailsById(int id, [FromBody] EmployeeUpdateDTO updateDto)
        {
            var result = await _empUserService.UpdateEmpUserDetailsByIdAsync(id, updateDto);
            return StatusCode(result.Code, new { result.Message });
        }
    }
}
