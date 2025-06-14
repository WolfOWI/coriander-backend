// Page Service (For pages that need to display multiple types of data)
// ========================================

using System;
using System.Threading.Tasks;
using System.Linq;
using CoriCore.DTOs.Page_Specific;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;

namespace CoriCore.Services;
 
public class PageService : IPageService
{
    private readonly IEmpUserService _empUserService;
    private readonly IEquipmentService _equipmentService;
    private readonly ILeaveBalanceService _leaveBalanceService;
    private readonly IPerformanceReviewService _performanceReviewService;
    private readonly ILeaveRequestService _leaveRequestService;
    private readonly IEmployeeService _employeeService;
    private readonly IAdminService _adminService;
    private readonly IGatheringService _gatheringService;
    private readonly IEmpLeaveRequestService _EmpLeaveRequestService;

    public PageService(
        IEmpUserService empUserService,
        IEquipmentService equipmentService,
        ILeaveBalanceService leaveBalanceService,
        IPerformanceReviewService performanceReviewService,
        ILeaveRequestService leaveRequestService,
        IEmployeeService employeeService,
        IAdminService adminService,
        IGatheringService gatheringService)
    {
        _empUserService = empUserService;
        _equipmentService = equipmentService;
        _leaveBalanceService = leaveBalanceService;
        _performanceReviewService = performanceReviewService;
        _leaveRequestService = leaveRequestService;
        _employeeService = employeeService;
        _adminService = adminService;
        _gatheringService = gatheringService;
    }

    public async Task<AdminDashboardPageDTO> GetAdminDashboardPageInfo(int adminId)
    {
        var adminUser = await _adminService.GetAdminUserByAdminId(adminId);
        var empUserRating = await _performanceReviewService.GetTopEmpUserRatingMetrics(5);
        var topRatedEmployees = await _performanceReviewService.GetTopRatedEmployees();
        var empStatusTotal = await _employeeService.GetEmployeeStatusTotals();

        return new AdminDashboardPageDTO
        {
            AdminUser = adminUser,
            EmpUserRatingMetrics = empUserRating,
            TopRatedEmployees = topRatedEmployees,
            EmployeeStatusTotals = empStatusTotal
        };
    }


    /// <inheritdoc/>
    public async Task<AdminEmpDetailsPageDTO> GetAdminEmpDetailsPageInfo(int employeeId)
    {
        // Execute database operations step by step (DbContext threading issues)
        var empUser = await _empUserService.GetEmpUserByEmpId(employeeId);
        var equipment = await _equipmentService.GetEquipmentByEmployeeId(employeeId);
        var leaveBalances = await _leaveBalanceService.GetAllLeaveBalancesByEmployeeId(employeeId);
        var ratingMetrics = await _performanceReviewService.GetEmpUserRatingMetricsByEmpId(employeeId);
        var gatherings = await _gatheringService.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId);

        // Create the DTO with all the gathered information
        return new AdminEmpDetailsPageDTO
        {
            EmpUser = empUser,
            Equipment = equipment,
            LeaveBalances = leaveBalances,
            EmpUserRatingMetrics = ratingMetrics,
            Gatherings = gatherings
                .ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<List<AdminEmpManagePageListItemDTO>> GetAdminEmpManagementPageInfo()
    {
        // Execute database operations step by step (DbContext threading issues)
        var empUsers = await _empUserService.GetAllEmpUsers();
        var empManageList = new List<AdminEmpManagePageListItemDTO>();

        // For each employee, get the rating metrics and total leave balance sum
        foreach (var empUser in empUsers)
        {
            var ratingMetrics = await _performanceReviewService.GetEmpUserRatingMetricsByEmpId(empUser.EmployeeId);
            var totalLeaveBalanceSum = await _leaveBalanceService.GetTotalLeaveBalanceSum(empUser.EmployeeId);

            // Add the employee details and metrics to the list
            empManageList.Add(new AdminEmpManagePageListItemDTO
            {
                EmpUser = empUser,
                EmpUserRatingMetrics = ratingMetrics,
                TotalLeaveBalanceSum = totalLeaveBalanceSum
            });
        }
        
        return empManageList;
    }

    /// <inheritdoc/>
    public async Task<EmployeeProfilePageDTO> GetEmployeeProfilePageInfo(int employeeId)
    {
        // Execute database operations step by step (DbContext threading issues)
        var empUser = await _empUserService.GetEmpUserByEmpId(employeeId);
        var equipment = await _equipmentService.GetEquipmentByEmployeeId(employeeId);
        var ratingMetrics = await _performanceReviewService.GetEmpUserRatingMetricsByEmpId(employeeId);

        // Create the DTO with all the gathered information
        return new EmployeeProfilePageDTO
        {
            EmpUser = empUser,
            Equipment = equipment,
            EmpUserRatingMetrics = ratingMetrics,
        };
        
    }

    /// <inheritdoc/>
    public async Task<EmployeeLeaveOverviewPageDTO> GetEmployeeLeaveOverviewPageInfo(int employeeId)
    {
        var leaveRequests = await _leaveRequestService.GetLeaveRequestsByEmployeeId(employeeId);
        var leaveBalances = await _leaveBalanceService.GetAllLeaveBalancesByEmployeeId(employeeId);

        return new EmployeeLeaveOverviewPageDTO
        {
            LeaveRequests = leaveRequests,
            LeaveBalances = leaveBalances
        };
    }
    
} 