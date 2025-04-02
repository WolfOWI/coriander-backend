using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces
{
    /// <summary>
    /// Interface for handling user-related services, especially regarding roles and existence checks.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Checks whether a given UserId is already registered as an Employee or Admin.
        /// </summary>
        /// <param name="userId">Unique identifier for the user.</param>
        /// <returns>
        /// - 201: "User available for registration".
        /// - 400: "User with {userId} already registered".
        /// </returns>
        Task<int> EmployeeAdminExistsAsync(int userId);

        /// <summary>
        /// Sets the role of a user in the system.
        /// </summary>
        /// <param name="userId">Unique identifier for the user.</param>
        /// <param name="userRole">Role to be assigned (0 = Unassigned, 1 = Employee, 2 = Admin).</param>
        /// <returns>
        /// - 201: "Successfully set user role".
        /// - 400: "UserId does not exist".
        /// </returns>
        Task<int> SetUserRoleAsync(int userId, int userRole);

        /// <summary>
        /// Check the role of a user.
        /// </summary>
        /// <param name="userId">Unique identifier for the user.</param>
        /// <returns>
        /// </returns>
        Task<UserRole> GetUserRoleAsync(int userId);


        /// Get user by email
        /// </summary>
        /// <param name="email">Email of the user.</param>
        /// <returns>
        /// </returns>
        Task<User?> GetUserByEmailAsync(string email);
    }
}