// Wolf Botha

using System;
using System.Security.Claims;
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

    // Two Factor Authentication
    Task SendVerificationCodeAsync(RequestEmailVerificationDTO dto);
    Task<bool> VerifyEmailCodeAsync(VerifyEmailCodeDTO dto);


    // ============================
    // SESSION MANAGEMENT
    // ============================
    
    // Generate JWT token
    Task<string> GenerateJwt(User user);
    
    // Revoke a JWT token (invalidate the session)
    Task<bool> RevokeToken(string token);

    // Logout a user (clear session data)
    Task<bool> LogoutUser();

    // Get the current authenticated user
    Task<User?> GetCurrentAuthenticatedUser();

    // Check if a user is authenticated
    Task<bool> IsUserAuthenticated();

    // Get current user info after login - frontend
    Task<CurrentUserDTO?> GetCurrentUserDetails(ClaimsPrincipal user);

    // ============================
    // GOOGLE SIGN UP & LOGIN
    // ============================
    
    // Login with Google (accepts OAuth token)
    Task<string> LoginWithGoogle(string googleToken);

    // Register with Google (creates a new user or links existing one)
    Task<bool> RegisterWithGoogle(string googleToken);
}
