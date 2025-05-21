// Auth Service Interface (Login & Register)
// ========================================

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

    Task<(int Code, string Message, bool IsCreated, bool CanSignIn)> RegisterAdminVerifiedAsync(
        RegisterVerifiedDTO dto
    );

    // ============================
    // 2FA - two factor authentication
    // ============================

    // Two Factor Authentication
    Task SendVerificationCodeAsync(RequestEmailVerificationDTO dto);
    Task<bool> VerifyEmailCodeAsync(VerifyEmailCodeDTO dto);
    Task<(int Code, string Message, bool IsCreated, bool CanSignIn)> RegisterVerifiedAsync(
        RegisterVerifiedDTO dto
    );

    // ============================
    // SESSION MANAGEMENT
    // ============================

    // Generate JWT token
    Task<string> GenerateJwt(User user);

    // Revoke a JWT token (invalidate the session)
    Task<bool> RevokeToken(string token);

    // Logout a user (clear session data)
    Task Logout(HttpContext context);

    // Get the current authenticated user
    Task<User?> GetCurrentAuthenticatedUser();

    // Temporary function - remove when website is in production state
    Task<CurrentUserDTO?> GetUserFromRawToken(string token);

    // Check if a user is authenticated
    Task<bool> IsUserAuthenticated();

    // Get current user info after login - frontend
    Task<CurrentUserDTO?> GetCurrentUserDetails(ClaimsPrincipal user);

    // ============================
    // GOOGLE SIGN UP & LOGIN
    // ============================

    // All users - Login with Google (accepts OAuth token)
    Task<(int Code, string Message, string? Token)> LoginWithGoogle(string googleToken, int role);

    // Employees - Register with Google (creates a new user or links existing one)
    Task<bool> RegisterWithGoogle(string googleToken);

    // Admin register with Google
    Task<(int Code, string Message, bool IsCreated, bool CanSignIn)> RegisterAdminWithGoogleAsync(
        string googleToken
    );
}
