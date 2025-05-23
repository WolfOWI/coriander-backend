// Page Service Interface
// ========================================

using System;
using System.Threading.Tasks;
using CoriCore.DTOs.Page_Specific;

namespace CoriCore.Interfaces;

public interface IPageService
{
    /// <summary>
    /// Gets all the information needed for the Admin Employee Details page
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>An AdminEmpDetailsPageDTO containing all necessary information</returns>
    Task<AdminEmpDetailsPageDTO> GetAdminEmpDetailsPageInfo(int employeeId);

    /// <summary>
    /// Gets all the information needed for the Admin Employee Management page (list of employees)
    /// </summary>
    /// <returns>An AdminEmpManagementPageDTO containing all necessary information</returns>
    Task<List<AdminEmpManagePageListItemDTO>> GetAdminEmpManagementPageInfo();

    /// <summary>
    /// Gets all the information needed for the Employee Profile page
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>An EmployeeProfilePageDTO containing all necessary information</returns>
    Task<EmployeeProfilePageDTO> GetEmployeeProfilePageInfo(int employeeId);

    /// <summary>
    /// Get all the information for the Admin Dashboard page
    /// </summary>
    /// <returns>An AdminDashboardPageDTO containing all necessary information</returns>
    Task<AdminDashboardPageDTO> GetAdminDashboardPageInfo(int adminId);

    /// <summary>
    /// Get all the information for the Employee Leave Overview page
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>An EmployeeLeaveOverviewPageDTO containing all necessary information</returns>
    Task<EmployeeLeaveOverviewPageDTO> GetEmployeeLeaveOverviewPageInfo(int employeeId);
} 
