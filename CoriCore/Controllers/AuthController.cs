using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        // Dependency Injection
        // ========================================
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        // ========================================

        // Register a new user
        // ========================================
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            bool isRegistered = await _authService.RegisterUser(user);

            if (!isRegistered)
            {
                return BadRequest("User already exists");
            }

            return Ok("User registered successfully");
        }
        // ========================================
        
        
    }
}
