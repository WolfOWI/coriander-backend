// Auth Service Interface (Login & Register)
// ========================================

using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

/// <summary>
/// This interface defines the methods for the authentication service.
/// </summary>
public interface IAuthService
{
    // ============================
    // USER MANAGEMENT
    // ============================
    
    /// <summary>
    /// Register a new user (via email method)
    /// </summary>
    /// <param name="user">The user to register</param>
    /// <returns>The registered user</returns>
    Task<User> RegisterWithEmail(UserEmailRegisterDTO user);

    /// <summary>
    /// Hash a password (using a secure algorithm)
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>The hashed password</returns>
    Task<string> HashPassword(string password);

    /// <summary>
    /// Verify a hashed password
    /// </summary>
    /// <param name="user">The user to verify</param>
    /// <param name="password">The password to verify</param>
    /// <returns>A boolean indicating the success of the operation</returns>
    Task<bool> VerifyPassword(User user, string password);

    /// <summary>
    /// Login a user (via email method)
    /// </summary>
    /// <param name="email">The email of the user</param>
    /// <param name="password">The password of the user</param>
    /// <returns>The logged in user</returns>
    Task<string> LoginWithEmail(string email, string password);

    // ============================
    // SESSION MANAGEMENT
    // ============================
    
    // Revoke a JWT token (invalidate the session)
    Task<bool> RevokeToken(string token);

    // Logout a user (clear session data)
    Task<bool> LogoutUser();

    // Get the current authenticated user
    Task<User?> GetCurrentAuthenticatedUser();

    // Check if a user is authenticated
    Task<bool> IsUserAuthenticated();

    // ============================
    // GOOGLE SIGN UP & LOGIN
    // ============================
    
    // Login with Google (accepts OAuth token)
    Task<string> LoginWithGoogle(string googleToken);

    // Register with Google (creates a new user or links existing one)
    Task<bool> RegisterWithGoogle(string googleToken);
}
