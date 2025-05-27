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
            // First, make sure the user actually exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                // 400 = BadRequest ⇒ userId invalid
                return 400;
            }

            // If there’s already an Employee record for this user…
            bool hasEmployee = await _context
                .Employees
                .AnyAsync(e => e.UserId == userId);

            // …or an Admin record…
            bool hasAdmin = await _context
                .Admins
                .AnyAsync(a => a.UserId == userId);

            if (hasEmployee || hasAdmin)
            {
                // 400 = BadRequest ⇒ already setup
                return 400;
            }

            // 201 = Created ⇒ available to register as employee
            return 201;
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
