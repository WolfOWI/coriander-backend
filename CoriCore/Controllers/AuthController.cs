using CoriCore.DTOs;
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

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">The user to register</param>
        /// <returns>A message indicating the success or failure of the registration attempt</returns>
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

        /// <summary>
        /// Log in a user using the standard email and password method.
        /// </summary>
        /// <param name="email">The email of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A message indicating the success or failure of the login attempt</returns>
        [HttpPost("login")]
        public async Task<IActionResult> EmailLogin(EmailLoginDTO user)
        {
            string loginResult = await _authService.LoginUser(user.Email, user.Password);

            if (loginResult != "Login successful")
            {
                return BadRequest(loginResult);
            }

            return Ok(loginResult);
        }
        
        
        
        
    }
}
