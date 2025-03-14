using System;
using CoriCore.Data;
using CoriCore.Interfaces;
using CoriCore.Models;
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
    
    public Task<bool> AssignRole(int userId, UserRole role)
    {
        throw new NotImplementedException();
    }

    public Task<User?> EmailExists(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetCurrentAuthenticatedUser()
    {
        throw new NotImplementedException();
    }

    public Task<UserRole?> GetUserRole(int userId)
    {
        throw new NotImplementedException();
    }

    public string HashPassword(string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUserAuthenticated()
    {
        throw new NotImplementedException();
    }

    public Task<string> LoginUser(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<string> LoginWithGoogle(string googleToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LogoutUser()
    {
        throw new NotImplementedException();
    }

    public Task<bool> RegisterUser(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RegisterWithGoogle(string googleToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RevokeToken(string token)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        throw new NotImplementedException();
    }
}
