// Admin Service
// ========================================

using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;

    public AdminService(AppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <inheritdoc/>
    public async Task<string> CreateAdmin(AdminDTO adminDTO)
    {
        // Make sure a user does exist (to link to being an admin)
        var existingUser = await _context.Users.FindAsync(adminDTO.UserId);
        if (existingUser == null)
        {
            return "A user with that id could not be found";
        }

        // Check if the user is already an admin
        var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.UserId == adminDTO.UserId);
        if (existingAdmin != null)
        {
            return "The user is already an admin";
        }

        // Map the DTO to the Admin model
        var admin = new Admin
        {
            UserId = adminDTO.UserId,
        };

        // Create the admin
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        return "Admin created successfully";
    }

    public async Task<AdminUserDTO?> GetAdminUserByAdminId(int adminId)
    {
        var admin = await _context.Admins
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AdminId == adminId);

        if (admin == null || admin.User == null)
            return null;

        return new AdminUserDTO
        {
            AdminId = admin.AdminId,
            UserId = admin.UserId,
            Email = admin.User.Email,
            FullName = admin.User.FullName
        };
    }
}
