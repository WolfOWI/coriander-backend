// Wolf Botha

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
    
    // Register a new user (via email method)
    Task<User> RegisterWithEmail(UserEmailRegisterDTO user);

    // Hash a password (using a secure algorithm)
    Task<string> HashPassword(string password);

    // Verify a hashed password
    Task<bool> VerifyPassword(User user, string password);

    // Login a user (via email method)
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
