using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

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

    public async Task<string> CreateAdmin(AdminDTO adminDTO)
    {
        // Make sure a user does exist (to link to being an admin)
        var existingUser = await _context.Users.FindAsync(adminDTO.UserId);
        if (existingUser == null)
        {
            return "A user with that id could not be found";
        }

        // Check if the user is an employee / already an admin
        var userRole = await _userService.GetUserRoleAsync(adminDTO.UserId);
        if (userRole == UserRole.Employee)
        {
            return "The user is an employee and cannot be promoted to an admin";
            
        } else if (userRole == UserRole.Admin)
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

        // Set the user role to admin
        await _userService.SetUserRoleAsync(adminDTO.UserId, 2); // Set user role to admin

        return "Admin created successfully & user's role was set to admin";
    }
}
