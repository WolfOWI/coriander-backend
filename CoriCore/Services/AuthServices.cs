// Wolf Botha & Ruan Klopper

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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

    // Constructor: Allows this service to interact with the database.
    public AuthServices(AppDbContext context, IUserService userService, IEmailService emailService)
    {
        _context = context;
        _userService = userService;
        _emailService = emailService;
    }
    // ========================================

    // JWT token generator
    // ========================================
    public Task<string> GenerateJwt(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("asjdfhaksljdfhsakjdhfsakjdfhaksldjfhalksjdfhaskjdhfkajdshfklasdfhalsdjkfhasdjkfhksadljhfkjsdhfaskjdhflasdfk2h321b7c3289c120b74c1290b12790.b123789"); // Replace with env var

        var claims = new List<Claim>
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.Role.ToString())
        };

        if (user.Employee != null)
            claims.Add(new Claim("employeeId", user.Employee.EmployeeId.ToString()));

        if (user.Admin != null)
            claims.Add(new Claim("adminId", user.Admin.AdminId.ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);
        return Task.FromResult(jwt);
    }

    // ========================================
    

    // User Registration & Role Assignment
    // ========================================
    // Register a new user (via email method)
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
            Role = user.Role
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
            u.GoogleId == payload.Subject || u.Email == payload.Email);

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
            Role = UserRole.Unassigned // (Changed this from employee to unassigned - Wolf)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return true;
    }

    // Hash a password using BCrypt
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
                IsVerified = false
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

        if (user.CodeGeneratedAt == null || DateTime.UtcNow - user.CodeGeneratedAt > TimeSpan.FromMinutes(10))
            return false;

        user.IsVerified = true;
        user.VerificationCode = null;
        user.CodeGeneratedAt = null;

        await _context.SaveChangesAsync();
        return true;
    }

    // ========================================
    

    // User Login & Authentication
    // ========================================
    // Login a user (with email & password) - whitout jwt
    // public async Task<string> LoginWithEmail(string email, string password)
    // {
    //     // Check if the user exists
    //     User? user = await _userService.GetUserByEmailAsync(email);

    //     // If the user does not exist
    //     if (user == null)
    //     {
    //         throw new Exception($"User with email {email} not found");
    //     }

    //     // Check if the password is valid
    //     bool isPasswordValid = await VerifyPassword(user, password);

    //     // If the password is invalid
    //     if (!isPasswordValid)
    //     {
    //         throw new Exception("Invalid password");
    //     }

    //     // Return a success message
    //     return "Login successful";
    // }
    // Login a user (with email & password) - with JWT
    public async Task<string> LoginWithEmail(string email, string password)
    {
        var user = await _context.Users
            .Include(u => u.Employee)
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

        var user = await _context.Users
            .Include(u => u.Employee)
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
                Role = UserRole.Unassigned
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
        
        if (userfromDb == null) return false;

        return true;
    }
    public Task<bool> RevokeToken(string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LogoutUser()
    {
        throw new NotImplementedException();
    }

    public async Task<CurrentUserDTO?> GetCurrentUserDetails(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim)) return null;

        int userId = int.Parse(userIdClaim);
        var u = await _context.Users
            .Include(u => u.Employee)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (u == null) return null;

        return new CurrentUserDTO
        {
            UserId = u.UserId,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.ToString(),
            ProfilePicture = u.ProfilePicture,
            EmployeeId = u.Employee?.EmployeeId,
            AdminId = u.Admin?.AdminId,
            IsLinked = (u.Role == UserRole.Admin && u.Admin != null) ||
                    (u.Role == UserRole.Employee && u.Employee != null)
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
    // ========================================

}
