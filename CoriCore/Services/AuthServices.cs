// Wolf Botha & Ruan Klopper

using System;
using CoriCore.Data;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

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
    // Register a new user
    public async Task<bool> RegisterUser(User user)
    {
        User? doesUserExist = await EmailExists(user.Email);

        // If the user already exists, return false
        if (doesUserExist != null)
        {
            return false;
        }

        // Hash the password
        user.Password = await HashPassword(user.Password);

        // Adding the user to the database
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return true;
        
    }

    public Task<bool> RegisterWithGoogle(string googleToken)
    {
        throw new NotImplementedException();
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
    public async Task<string> LoginUser(string email, string password)
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

    public Task<string> LoginWithGoogle(string googleToken)
    {
        throw new NotImplementedException();
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
