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
        private readonly IAdminService _adminService;
        public AuthController(IAuthService authService, IAdminService adminService)
        {
            _authService = authService;
            _adminService = adminService;
        }
        // ========================================

        /// <summary>
        /// Registers a new user (unassigned role).
        /// </summary>
        /// <param name="user">The user to register</param>
        /// <returns>A message indicating the success or failure of the registration attempt</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserEmailRegisterDTO user)
        {
            User createdUser = await _authService.RegisterWithEmail(user);

            if (createdUser == null)
            {
                return BadRequest("User already exists");
            }

            return Ok("User registered successfully");
        }

        /// <summary>
        /// Registers a new admin user. Creating both a user (with admin role) and a linked admin.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(UserEmailRegisterDTO user)
        {
            // Set the role to Admin before registration
            user.Role = UserRole.Admin;

            // USER CREATION
            // ========================================
            User createdUser = await _authService.RegisterWithEmail(user);

            if (createdUser == null)
            {
                return BadRequest("User was not created");
            }

            // ADMIN CREATION (ASSIGNMENT TO USER)
            // ========================================
            // Create an AdminDTO from the created user
            var adminDTO = new AdminDTO
            {
                UserId = createdUser.UserId
            };

            await _adminService.CreateAdmin(adminDTO);

            return Ok("Admin registered successfully");
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
            string loginResult = await _authService.LoginWithEmail(user.Email, user.Password);

            if (loginResult != "Login successful")
            {
                return BadRequest(loginResult);
            }

            return Ok(loginResult);
        }
        
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
        {
            string token = await _authService.LoginWithGoogle(dto.IdToken);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid Google login.");
            }

            return Ok(new { token });
        }

        [HttpPost("google-register")]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleRegisterDTO dto)
        {
            bool isRegistered = await _authService.RegisterWithGoogle(dto.IdToken);

            if (!isRegistered)
            {
                return BadRequest("User already exists or registration failed.");
            }

            return Ok("Google registration successful.");
        }
    }
}
