using System;
using CoriCore.Data;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> EmployeeAdminExistsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .Include(u => u.Admin)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || (user.Employee == null && user.Admin == null))
            {
                return 201; // User available for registration
            }

            return 400; // User with userId already registered as Employee or Admin
        }

        /// <inheritdoc />
        public async Task<int> SetUserRoleAsync(int userId, int userRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return 400; // UserId does not exist
            }

            if (!Enum.IsDefined(typeof(UserRole), userRole))
            {
                return 400; // Invalid role enum
            }

            user.Role = (UserRole)userRole;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return 201; // Successfully set user role
        }

        // Template
        // public Task<int> SetUserRoleAsync(int userId, int userRole)
        // {
        //     throw new NotImplementedException();
        // }
    }
}