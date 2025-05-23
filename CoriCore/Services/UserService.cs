// User Service
// ========================================

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
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return 400; // UserId does not exist
            }

            if (user.Role == UserRole.Unassigned)
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

        /// <inheritdoc/>
        public async Task<UserRole> GetUserRoleAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            // If user is not found
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found");
            }

            return user.Role;
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<List<User>> GetUnlinkedUsersAsync()
        {
            var linkedEmployeeIds = await _context.Employees.Select(e => e.UserId).ToListAsync();
            var linkedAdminIds = await _context.Admins.Select(a => a.UserId).ToListAsync();

            var unlinkedUsers = await _context
                .Users.Where(u =>
                    !linkedEmployeeIds.Contains(u.UserId) && !linkedAdminIds.Contains(u.UserId)
                )
                .ToListAsync();

            return unlinkedUsers;
        }

        // Template
        // public Task<int> SetUserRoleAsync(int userId, int userRole)
        // {
        //     throw new NotImplementedException();
        // }
    }
}
