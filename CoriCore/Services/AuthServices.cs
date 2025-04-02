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
    // Inject the database context into the service
    private readonly AppDbContext _context;

    // Constructor: Allows this service to interact with the database.
    public AuthServices(AppDbContext context)
    {
        _context = context;
    }
    // ========================================
    

    // User Registration & Role Assignment
    // ========================================
    // Register a new user (via email method)
    public async Task<bool> RegisterWithEmail(UserEmailRegisterDTO user)
    
    {
        User? doesUserExist = await EmailExists(user.Email);

        // If the user already exists, return false
        if (doesUserExist != null)
        {
            return false;
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

        return true;
    }

    // Register a new admin-user (via email method)
    public async Task<bool> RegisterAdminWithEmail(AdminUserEmailRegisterDTO user)
    {
        User? doesUserExist = await EmailExists(user.Email);

        // If the user already exists, return false
        if (doesUserExist != null)
        {
            return false;
        }

        // Hash the password
        string hashedPassword = await HashPassword(user.Password);

        // Create new admin with DTO (include User model & admin model)
        var newAdmin = new AdminUserEmailRegisterDTO
        {
            FullName = user.FullName,
            Email = user.Email,
            Password = hashedPassword,
            ProfilePicture = user.ProfilePicture,
            Role = user.Role,
            AdminId = user.AdminId
        };

        return true;
    }

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

    public Task<bool> AssignRole(int userId, UserRole role)
    {
        throw new NotImplementedException();
    }
    
    public Task<UserRole?> GetUserRole(int userId)
    {
        throw new NotImplementedException();
    }
    // ========================================
    

    // User Login & Authentication
    // ========================================
    // Login a user (with email & password)
    public async Task<string> LoginWithEmail(string email, string password)
    {
        // Check if the user exists
        User? user = await EmailExists(email);

        // If the user does not exist
        if (user == null)
        {
            return "User not found";
        }

        // Check if the password is valid
        bool isPasswordValid = await VerifyPassword(user, password);

        // If the password is invalid
        if (!isPasswordValid)
        {
            return "Invalid password";
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
    // Check if a user with the given email exists in the database
    public async Task<User?> EmailExists(string email)
    {
        // Get the user from the database
        User? userfromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        return userfromDb;
        // If the user exists, return the user
        // If the user does not exist, return null
    }

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
