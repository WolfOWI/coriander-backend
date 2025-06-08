// Auth Controller
// ========================================

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


        // ========================================
        // Default registration
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

        // ========================================
        // 🔴 Admins - Production registration & 2 factor authentication "Sign up" : Google & Email
        // ========================================

        // Register admin with Email - after 2FA code is correct
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
            var adminDTO = new AdminDTO { UserId = createdUser.UserId };

            await _adminService.CreateAdmin(adminDTO);

            return Ok("Admin registered successfully");
        }

        // Register admin with Google
        [HttpPost("google-register-admin")]
        public async Task<IActionResult> GoogleRegisterAdmin([FromBody] GoogleRegisterDTO dto)
        {
            var (code, message, isCreated, canSignIn) =
                await _authService.RegisterAdminWithGoogleAsync(dto.IdToken);

            return StatusCode(
                code,
                new
                {
                    message,
                    isCreated,
                    canSignIn,
                }
            );
        }

        [HttpPost("register-admin-verified")]
        public async Task<IActionResult> RegisterAdminVerified([FromForm] RegisterVerifiedDTO dto)
        {
            var (code, message, isCreated, canSignIn) =
                await _authService.RegisterAdminVerifiedAsync(dto);

            return StatusCode(
                code,
                new
                {
                    message,
                    isCreated,
                    canSignIn,
                }
            );
        }

        // ========================================
        // 🔵 Employees - Production registration & 2 factor authentication "Sign up" : Google & Email
        // ========================================

        // Register employee with Google
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

        // Register employee with Email - after 2FA code is correct
        [HttpPost("register-verified")]
        public async Task<IActionResult> RegisterVerified([FromForm] RegisterVerifiedDTO dto)
        {
            var (code, message, isCreated, canSignIn) = await _authService.RegisterVerifiedAsync(
                dto
            );

            return StatusCode(
                code,
                new
                {
                    message,
                    isCreated,
                    canSignIn,
                }
            );
        }

        // ========================================
        // 🟢 All users - 2FA
        // ========================================

        // 2FA - only applicable for email registration
        [HttpPost("request-verification")]
        public async Task<IActionResult> RequestVerification(
            [FromBody] RequestEmailVerificationDTO dto
        )
        {
            await _authService.SendVerificationCodeAsync(dto);
            return Ok("Verification code sent");
        }

        [HttpPost("verify-email-code")]
        public async Task<IActionResult> VerifyEmailCode([FromBody] VerifyEmailCodeDTO dto)
        {
            var result = await _authService.VerifyEmailCodeAsync(dto);
            if (!result)
                return BadRequest("Invalid or expired code");
            return Ok("Email verified successfully");
        }

        // ========================================
        // 🟢 All users - Production authentication "loggin in" : Google & Email
        // ========================================

        // Google login - returns JWT
        // [HttpPost("google-login")]
        // public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
        // {
        //     string token = await _authService.LoginWithGoogle(dto.IdToken);

        //     if (string.IsNullOrEmpty(token))
        //     {
        //         return Unauthorized("Invalid Google login.");
        //     }

        //     Response.Cookies.Append(
        //         "token",
        //         token,
        //         new CookieOptions
        //         {
        //             HttpOnly = false,
        //             Secure = true,
        //             SameSite = SameSiteMode.Lax, // 🔁 change this to test, in production change to .strict
        //             Expires = DateTime.UtcNow.AddDays(7),
        //         }
        //     );

        //     return Ok("Login successful");
        // }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
        {
            var (code, message, token) = await _authService.LoginWithGoogle(dto.IdToken, dto.Role);

            return StatusCode(code, new { token, message });
        }



        // Email login - returns JWT
        [HttpPost("login")]
        public async Task<IActionResult> EmailLogin(EmailLoginDTO user)
        {
            try
            {
                string jwt = await _authService.LoginWithEmail(user.Email, user.Password);

                return Ok(new { token = jwt, message = "Login successful" });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { token = (string?)null, message = ex.Message });
            }
        }


        // ========================================
        // 🟢 All users - JWTs / session management
        // ========================================

        // See if user is logged in and Get current user details when logged in
        // Not supported on Electron localhost
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserDTO>> GetMe()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("No user ID found in claims.");
                return Unauthorized("No user.");
            }

            var user = await _authService.GetCurrentUserDetails(User);
            return Ok(user);
        }


        // Using this to decode the JWT token
        [HttpGet("decode-token")]
        public async Task<IActionResult> DecodeToken([FromQuery] string token)
        {
            var userData = await _authService.GetUserFromRawToken(token);

            if (userData == null)
                return Unauthorized("Invalid token or user does not exist.");

            return Ok(userData);
        }

        // Removes the JWT token if a user signs out
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout(HttpContext);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
