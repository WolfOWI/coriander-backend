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
    /// Get all admins
    /// </summary>
    /// <returns>A list of all admins in AdminUserDTO format</returns>
    Task<List<AdminUserDTO>> GetAllAdmins();

    /// <summary>
    /// Get an admin by admin ID
    /// </summary>
    /// <param name="adminId">The admin ID</param>
    Task<AdminUserDTO?> GetAdminUserByAdminId(int adminId);
    

}
