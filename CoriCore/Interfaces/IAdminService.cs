// Admin Service Interface
// ========================================

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IAdminService
{
    /// <summary>
    /// Create a new admin
    /// </summary>
    /// <param name="adminDTO">The admin DTO</param>
    /// <returns>A string indicating the success of the operation</returns>
    Task<string> CreateAdmin(AdminDTO adminDTO);

    /// <summary>
    /// Get an admin user by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>The AdminUserDTO if found, otherwise null</returns>
    Task<AdminUserDTO?> GetAdminUserByAdminId(int userId);

}
