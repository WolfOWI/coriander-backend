using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Authorization;
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
        // [HttpPost("login")]
        // public async Task<IActionResult> EmailLogin(EmailLoginDTO user)
        // {
        //     string loginResult = await _authService.LoginWithEmail(user.Email, user.Password);

        //     if (loginResult != "Login successful")
        //     {
        //         return BadRequest(loginResult);
        //     }

        //     return Ok(loginResult);
        // }

        // Regular email & password login with JWT
        [HttpPost("login")]
        public async Task<IActionResult> EmailLogin(EmailLoginDTO user)
        {
            string jwt = await _authService.LoginWithEmail(user.Email, user.Password);

            Response.Cookies.Append("token", jwt, new CookieOptions
            {
                HttpOnly = false, // change to true when website is in production or when working with the frontend
                Secure = false, // same like above
                SameSite = SameSiteMode.Lax, // 🔁 change this to test, in production change to .strict
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok("Login successful");
        }
        
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
        {
            string token = await _authService.LoginWithGoogle(dto.IdToken);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid Google login.");
            }

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax, // 🔁 change this to test, in production change to .strict
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok("Login successful");
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

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userData = await _authService.GetCurrentUserDetails(User);

            if (userData == null)
                return Unauthorized("Invalid user session");

            return Ok(userData);
        }

        // Two-Step verification end points:
        [HttpPost("request-verification")]
        public async Task<IActionResult> RequestVerification([FromBody] RequestEmailVerificationDTO dto)
        {
            await _authService.SendVerificationCodeAsync(dto);
            return Ok("Verification code sent");
        }

        [HttpPost("verify-email-code")]
        public async Task<IActionResult> VerifyEmailCode([FromBody] VerifyEmailCodeDTO dto)
        {
            var result = await _authService.VerifyEmailCodeAsync(dto);
            if (!result) return BadRequest("Invalid or expired code");
            return Ok("Email verified successfully");
        }

        [HttpPost("register-verified")]
        public async Task<IActionResult> RegisterVerified([FromBody] RegisterVerifiedDTO dto)
        {
            var (code, message, isCreated, canSignIn) = await _authService.RegisterVerifiedAsync(dto);

            return StatusCode(code, new
            {
                message,
                isCreated,
                canSignIn
            });
        }

    }
}
