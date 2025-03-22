using CoriCore.Data;
using CoriCore.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpUserController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all user employees and their information
        /// </summary>
        /// <returns>List of EmpUserDTO</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpUserDTO>>> GetAllEmpUser()
        {
            var empUsers = await _context.Employees
                .Include(e => e.User)
                .Select(e => new EmpUserDTO
                {
                    // User Information
                    UserId = e.UserId,
                    FullName = e.User.FullName,
                    Email = e.User.Email,
                    ProfilePicture = e.User.ProfilePicture,
                    Role = e.User.Role,

                    // Employee Information
                    EmployeeId = e.EmployeeId,
                    Gender = e.Gender,
                    DateOfBirth = e.DateOfBirth,
                    PhoneNumber = e.PhoneNumber,
                    JobTitle = e.JobTitle,
                    Department = e.Department,
                    SalaryAmount = e.SalaryAmount,
                    PayCycle = e.PayCycle,
                    PastPayday = e.PastPayday,
                    PastPaydayIsPaid = e.PastPaydayIsPaid,
                    NextPayday = e.NextPayday,
                    EmployType = e.EmployType,
                    EmployDate = e.EmployDate,
                    IsSuspended = e.IsSuspended
                })
                .ToListAsync();

            return Ok(empUsers);
        }
    }
}
