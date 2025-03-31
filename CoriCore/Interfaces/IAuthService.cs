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
    Task<bool> RegisterWithEmail(UserEmailRegisterDTO user);

    // Hash a password (using a secure algorithm)
    Task<string> HashPassword(string password);

    // Verify a hashed password
    Task<bool> VerifyPassword(User user, string password);

    // Login a user (via email method)
    Task<string> LoginWithEmail(string email, string password);

    // Check if email exists
    // Returns user data or null (if user doesn't exist)
    Task<User?> EmailExists(string email);

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
    // ROLE MANAGEMENT
    // ============================
    
    // Get the role of the current user
    Task<UserRole?> GetUserRole(int userId);

    // Assign a role to a user (0 = Unassigned, 1 = Employee, 2 = Admin)
    Task<bool> AssignRole(int userId, UserRole role);

    // ============================
    // GOOGLE SIGN UP & LOGIN
    // ============================
    
    // Login with Google (accepts OAuth token)
    Task<string> LoginWithGoogle(string googleToken);

    // Register with Google (creates a new user or links existing one)
    Task<bool> RegisterWithGoogle(string googleToken);
}
