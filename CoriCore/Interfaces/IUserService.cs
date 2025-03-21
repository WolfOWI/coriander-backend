using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Validation step for RegisterEmployee/Admin.
        /// Check whether the inputted UserId already belongs to another Employee/Admin
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Boolean, true: UserId already assigned, false: UserId not assigned </returns>
        Task<bool> EmployeeAdminExists(int UserId);

        /// <summary>
        /// Sets the role of a user (0 = Unassigned, 1 = Employee, 2 = Admin)
        /// </summary>
        /// <param name="userId">UserId to update</param>
        /// <param name="role">Role to assign</param>
        /// <returns>True if update is successful, False otherwise</returns>
        Task<bool> SetUserRole(int userId, UserRole role);

        // Frontend display to select an unlinked user
        // Task<bool> GetAllUnlinkedUsers(int id);
    }
}