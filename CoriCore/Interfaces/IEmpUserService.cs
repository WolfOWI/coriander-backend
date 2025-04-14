// EmpUser Service Interface
// ========================================

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IEmpUserService
{
    /// <summary>
    /// Get all employee users
    /// </summary>
    /// <returns>The list of employee users</returns>
    Task<List<EmpUserDTO>> GetAllEmpUsers();

    /// <summary>
    /// Get employee user by employee id
    /// </summary>
    /// <param name="id">The ID of the employee user to get</param>
    /// <returns>The employee user details</returns>
    Task<EmpUserDTO> GetEmpUserByEmpId(int id);
    
    /// <summary>
    /// Updates an employee user details
    /// </summary>
    /// <param name="id">The ID of the employee user to update</param>
    /// <param name="updateDto">DTO containing the fields to update</param>
    /// <returns>
    /// Tuple result: (int Code, string Message)
    /// 200 - Successfully updated
    /// 404 - Employee not found
    /// 400 - Invalid data with corresponding error message
    /// </returns>
    Task<(int Code, string Message)> UpdateEmpUserDetailsByIdAsync(int id, EmployeeUpdateDTO updateDto);

}
