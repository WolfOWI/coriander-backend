// Auth Services
// ========================================
// Wolf Botha & Ruan Klopper

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;

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

    // Constructor: Allows this service to interact with the database.
    public AuthServices(AppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
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
    

    // User Login & Authentication
    // ========================================
    // Login a user (with email & password)
    public async Task<string> LoginWithEmail(string email, string password)
    {
        // Check if the user exists
        User? user = await _userService.GetUserByEmailAsync(email);

        // If the user does not exist
        if (user == null)
        {
            throw new Exception($"User with email {email} not found");
        }

        // Check if the password is valid
        bool isPasswordValid = await VerifyPassword(user, password);

        // If the password is invalid
        if (!isPasswordValid)
        {
            throw new Exception("Invalid password");
        }

        // Return a success message
        return "Login successful";
    }

    public async Task<string> LoginWithGoogle(string googleToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject || u.Email == payload.Email);

        if (user == null)
        {
            user = new User
            {
                FullName = payload.Name,
                Email = payload.Email,
                GoogleId = payload.Subject,
                ProfilePicture = payload.Picture,
                Role = UserRole.Unassigned // (Changed this from employee to unassigned - Wolf)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // TODO: Replace with your actual JWT generation logic
        return $"JWT_FOR_{user.Email}";
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
