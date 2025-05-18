// Auth Services
// ========================================
// Wolf Botha & Ruan Klopper

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoriCore.Services;

/// <summary>
/// Handles all of the functionality to authenticate a user
/// </summary>
public class AuthServices : IAuthService
{
    // DEPENDENCY INJECTION
    // ========================================
    private readonly AppDbContext _context;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IImageService _imageService;

    // Constructor: Allows this service to interact with the database.
    public AuthServices(
        AppDbContext context,
        IUserService userService,
        IEmailService emailService,
        IImageService imageService
    )
    {
        _context = context;
        _userService = userService;
        _emailService = emailService;
        _imageService = imageService;
    }

    // ========================================

    // JWT token generator
    // ========================================
    public Task<string> GenerateJwt(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(
            "asjdfhaksljdfhsakjdhfsakjdfhaksldjfhalksjdfhaskjdhfkajdshfklasdfhalsdjkfhasdjkfhksadljhfkjsdhfaskjdhflasdfk2h321b7c3289c120b74c1290b12790.b123789"
        ); // Replace with env var

        var claims = new List<Claim>
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.Role.ToString()),
        };

        if (user.Employee != null)
            claims.Add(new Claim("employeeId", user.Employee.EmployeeId.ToString()));

        if (user.Admin != null)
            claims.Add(new Claim("adminId", user.Admin.AdminId.ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);
        return Task.FromResult(jwt);
    }

    // ========================================


    // User Registration & Role Assignment
    // ========================================
    /// <inheritdoc/>
    public async Task<User> RegisterWithEmail(UserEmailRegisterDTO user)
    {
        // Check if the user already exists
        if (await userExistsByEmail(user.Email))
        {
            throw new Exception($"User with email {user.Email} already exists");
        }

        // Hash the password
        string hashedPassword = await HashPassword(user.Password);

        // Create new user from DTO
        var newUser = new User
        {
            FullName = user.FullName,
            Email = user.Email,
            Password = hashedPassword,
            ProfilePicture = user.ProfilePicture,
            Role = user.Role,
        };

        // Adding the user to the database
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    // Register a new user (via Google method)
    public async Task<bool> RegisterWithGoogle(string googleToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);

        // Check if user already exists by Google ID or email
        var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
            u.GoogleId == payload.Subject || u.Email == payload.Email
        );

        if (existingUser != null)
        {
            return false; // Already registered
        }

        var user = new User
        {
            FullName = payload.Name,
            Email = payload.Email,
            GoogleId = payload.Subject,
            ProfilePicture = payload.Picture,
            Role = UserRole.Unassigned, // (Changed this from employee to unassigned - Wolf)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        await _emailService.SendAccountPendingEmail(user.Email, user.FullName);

        return true;
    }

    // Register a new admin (via Google method)
    public async Task<(
        int Code,
        string Message,
        bool IsCreated,
        bool CanSignIn
    )> RegisterAdminWithGoogleAsync(string googleToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);

        // Check if user already exists
        var existing = await _context
            .Users.Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.Email == payload.Email || u.GoogleId == payload.Subject);

        if (existing != null)
        {
            if (existing.IsVerified && existing.Role == UserRole.Admin && existing.Admin != null)
                return (409, "Admin account already exists. Try logging in.", false, true);

            return (
                409,
                "Email already registered under a different role or not verified.",
                false,
                false
            );
        }

        // Create and verify user
        var user = new User
        {
            FullName = payload.Name,
            Email = payload.Email,
            GoogleId = payload.Subject,
            ProfilePicture = payload.Picture,
            Role = UserRole.Admin,
            IsVerified = true,
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Link admin
        var admin = new Admin { UserId = user.UserId };

        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        return (200, "Admin account created and linked via Google.", true, true);
    }

    // Register a new admin (via Email method) & 2FA
    public async Task<(
        int Code,
        string Message,
        bool IsCreated,
        bool CanSignIn
    )> RegisterAdminVerifiedAsync(RegisterVerifiedDTO dto)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existing == null)
            return (
                404,
                "Email not found. Please request a verification code first.",
                false,
                false
            );

        if (existing.IsVerified)
            return (409, "Email already verified. Try logging in instead.", false, true);

        if (existing.VerificationCode != dto.Code)
            return (401, "Invalid verification code.", false, false);

        if (
            existing.CodeGeneratedAt == null
            || DateTime.UtcNow - existing.CodeGeneratedAt > TimeSpan.FromMinutes(10)
        )
            return (410, "Verification code expired. Request a new one.", false, false);

        // ✅ Update the user
        existing.FullName = dto.FullName;
        existing.Password = await HashPassword(dto.Password);
        existing.Role = UserRole.Admin;
        existing.IsVerified = true;
        existing.VerificationCode = null;
        existing.CodeGeneratedAt = null;

        if (dto.ProfileImage != null)
        {
            var imagePath = await _imageService.UploadImageAsync(dto.ProfileImage);
            existing.ProfilePicture = imagePath;
        }

        await _context.SaveChangesAsync();

        // ✅ Create and assign Admin entity
        var admin = new Admin { UserId = existing.UserId };
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        return (200, "Admin account created and linked successfully.", true, true);
    }

    /// <inheritdoc/>
    public Task<string> HashPassword(string password)
    {
        string HashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 13);
        return Task.FromResult(HashedPassword);
    }

    // ========================================

    // Two factor authentication
    // ========================================
    // Send and generate a verification code for user
    public async Task SendVerificationCodeAsync(RequestEmailVerificationDTO dto)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existing != null && existing.IsVerified)
            throw new Exception("Email already verified and registered.");

        var code = new Random().Next(100000, 999999).ToString();

        if (existing == null)
        {
            existing = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                IsVerified = false,
            };
            await _context.Users.AddAsync(existing);
        }

        existing.VerificationCode = code;
        existing.CodeGeneratedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _emailService.SendVerificationCodeEmail(dto.Email, code, dto.FullName);
    }

    // Verify code send via email
    public async Task<bool> VerifyEmailCodeAsync(VerifyEmailCodeDTO dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || user.VerificationCode != dto.Code)
            return false;

        if (
            user.CodeGeneratedAt == null
            || DateTime.UtcNow - user.CodeGeneratedAt > TimeSpan.FromMinutes(10)
        )
            return false;

        user.IsVerified = true;
        user.VerificationCode = null;
        user.CodeGeneratedAt = null;

        await _context.SaveChangesAsync();
        return true;
    }

    // Function to do a full user registration after the 2FA has been sent
    // It will register user
    public async Task<(
        int Code,
        string Message,
        bool IsCreated,
        bool CanSignIn
    )> RegisterVerifiedAsync(RegisterVerifiedDTO dto)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existing == null)
        {
            return (
                404,
                "Email not found. Please request a verification code first.",
                false,
                false
            );
        }

        if (existing.IsVerified)
        {
            return (409, "Email already verified. Try logging in instead.", false, true);
        }

        if (existing.VerificationCode != dto.Code)
        {
            return (401, "Invalid verification code.", false, false);
        }

        if (
            existing.CodeGeneratedAt == null
            || DateTime.UtcNow - existing.CodeGeneratedAt > TimeSpan.FromMinutes(10)
        )
        {
            return (410, "Verification code expired. Request a new one.", false, false);
        }

        existing.FullName = dto.FullName;
        existing.Password = await HashPassword(dto.Password);
        existing.Role = dto.Role;
        existing.IsVerified = true;
        existing.VerificationCode = null;
        existing.CodeGeneratedAt = null;

        if (dto.ProfileImage != null)
        {
            var imagePath = await _imageService.UploadImageAsync(dto.ProfileImage);
            existing.ProfilePicture = imagePath;
        }

        await _context.SaveChangesAsync();
        await _emailService.SendAccountPendingEmail(existing.Email, existing.FullName);

        return (200, "Account created successfully.", true, true);
    }

    // ========================================


    // User Login & Authentication
    // ========================================
    /// <inheritdoc/>
    public async Task<string> LoginWithEmail(string email, string password)
    {
        var user = await _context
            .Users.Include(u => u.Employee)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new Exception($"User with email {email} not found");

        if (!await VerifyPassword(user, password))
            throw new Exception("Invalid password");

        return await GenerateJwt(user);
    }

    public async Task<string> LoginWithGoogle(string googleToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);

        var user = await _context
            .Users.Include(u => u.Employee)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject || u.Email == payload.Email);

        if (user == null)
        {
            user = new User
            {
                FullName = payload.Name,
                Email = payload.Email,
                GoogleId = payload.Subject,
                ProfilePicture = payload.Picture,
                Role = UserRole.Unassigned,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        return await GenerateJwt(user);
    }

    // Verify a User's password against a hashed password
    public Task<bool> VerifyPassword(User user, string password)
    {
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        return Task.FromResult(isPasswordValid);
    }

    // ========================================


    // Session Management
    // ========================================
    // Check if a user with the given email exists in the database
    public async Task<bool> userExistsByEmail(string email)
    {
        // Get the user from the database
        User? userfromDb = await _userService.GetUserByEmailAsync(email);

        if (userfromDb == null)
            return false;

        return true;
    }

    public Task<bool> RevokeToken(string token)
    {
        throw new NotImplementedException();
    }

    public Task Logout(HttpContext context)
    {
        context.Response.Cookies.Delete("token");
        return Task.CompletedTask;
    }

    public async Task<CurrentUserDTO?> GetCurrentUserDetails(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return null;

        int userId = int.Parse(userIdClaim);
        var u = await _context
            .Users.Include(u => u.Employee)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (u == null)
            return null;

        return new CurrentUserDTO
        {
            UserId = u.UserId,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.ToString(),
            ProfilePicture = u.ProfilePicture,
            IsVerified = u.IsVerified,
            EmployeeId = u.Employee?.EmployeeId,
            AdminId = u.Admin?.AdminId,
            IsLinked =
                (u.Role == UserRole.Admin && u.Admin != null)
                || (u.Role == UserRole.Employee && u.Employee != null),
        };
    }

    // ========================================


    // Helper Methods
    // ========================================
    public Task<User?> GetCurrentAuthenticatedUser()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUserAuthenticated()
    {
        throw new NotImplementedException();
    }

    public async Task<CurrentUserDTO?> GetUserFromRawToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim == null)
                return null;

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return null;

            var user = await _context
                .Users.Include(u => u.Employee)
                .Include(u => u.Admin)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            return new CurrentUserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                ProfilePicture = user.ProfilePicture,
                IsVerified = user.IsVerified,
                EmployeeId = user.Employee?.EmployeeId,
                AdminId = user.Admin?.AdminId,
                IsLinked =
                    (user.Role == UserRole.Admin && user.Admin != null)
                    || (user.Role == UserRole.Employee && user.Employee != null),
            };
        }
        catch
        {
            return null;
        }
    }

    // ========================================
}
