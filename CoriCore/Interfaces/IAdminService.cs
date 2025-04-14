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

}
