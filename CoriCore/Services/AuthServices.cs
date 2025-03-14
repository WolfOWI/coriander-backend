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
    /// <summary>
    /// Registers a new user in the database
    /// </summary>
    /// <param name="user">The user to register</param>
    /// <returns>True if the user was registered successfully, otherwise false</returns>
    public async Task<bool> RegisterUser(User user)
    {
        User? doesUserExist = await EmailExists(user.Email);

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

    /// <summary>
    /// Hashes a password using a secure algorithm
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>The hashed password</returns>
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
    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="email">The email of the user to login</param>
    /// <param name="password">The password of the user to login</param>
    /// <returns>A message indicating the success or failure of the login attempt</returns>
    public async Task<string> LoginUser(string email, string password)
    {
        User? user = await EmailExists(email);

        if (user == null)
        {
            return "User not found";
        }

        // Check if the password is valid
        bool isPasswordValid = await VerifyPassword(user, password);

        if (!isPasswordValid)
        {
            return "Invalid password";
        }

        return "Login successful";
    }

    public Task<string> LoginWithGoogle(string googleToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Verifies a password against a hashed password
    /// </summary>
    /// <param name="user">The user to verify the password for</param>
    /// <param name="password">The password to verify</param>
    /// <returns>True if the password is valid, otherwise false</returns>
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
    /// <summary>
    /// Checks if a user with the given email exists in the database
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <returns>The user if they exist, otherwise null</returns>
    public async Task<User?> EmailExists(string email)
    {
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
