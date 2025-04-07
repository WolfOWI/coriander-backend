using System;
using System.Threading.Tasks;
using System.Linq;
using CoriCore.DTOs.Page_Specific;
using CoriCore.DTOs;
using CoriCore.Interfaces;

namespace CoriCore.Services;

public class PageService : IPageService
{
    private readonly IEmpUserService _empUserService;
    private readonly IEquipmentService _equipmentService;
    private readonly ILeaveBalanceService _leaveBalanceService;
    private readonly IPerformanceReviewService _performanceReviewService;

    public PageService(
        IEmpUserService empUserService,
        IEquipmentService equipmentService,
        ILeaveBalanceService leaveBalanceService,
        IPerformanceReviewService performanceReviewService)
    {
        _empUserService = empUserService;
        _equipmentService = equipmentService;
        _leaveBalanceService = leaveBalanceService;
        _performanceReviewService = performanceReviewService;
    }

    public async Task<AdminEmpDetailsPageDTO> GetAdminEmpDetailsPageInfo(int employeeId)
    {
        // Execute database operations sequentially to avoid DbContext threading issues
        var empUser = await _empUserService.GetEmpUserByEmpId(employeeId);
        var equipment = await _equipmentService.GetEquipmentByEmployeeId(employeeId);
        var leaveBalances = await _leaveBalanceService.GetAllLeaveBalancesByEmployeeId(employeeId);
        var ratingMetrics = await _performanceReviewService.GetEmpUserRatingMetricsByEmpId(employeeId);
        var performanceReviews = await _performanceReviewService.GetPrmByEmpId(employeeId);

        // Create the DTO with all the gathered information
        return new AdminEmpDetailsPageDTO
        {
            EmpUser = empUser,
            Equipment = equipment,
            LeaveBalances = leaveBalances,
            EmpUserRatingMetrics = ratingMetrics,
            PerformanceReviews = performanceReviews
                .Select(pr => new PerformanceReviewDTO
                {
                    ReviewId = pr.ReviewId,
                    AdminId = pr.AdminId,
                    AdminName = pr.Admin.User.FullName,
                    EmployeeId = pr.EmployeeId,
                    EmployeeName = pr.Employee.User.FullName,
                    IsOnline = pr.IsOnline,
                    MeetLocation = pr.MeetLocation,
                    MeetLink = pr.MeetLink,
                    StartDate = pr.StartDate,
                    EndDate = pr.EndDate,
                    Rating = pr.Rating,
                    Comment = pr.Comment,
                    DocUrl = pr.DocUrl,
                    Status = (ReviewStatus)pr.Status
                })
                .ToList()
        };
    }

    public async Task<List<AdminEmpManagePageListItemDTO>> GetAdminEmpManagementPageInfo()
    {
        // Execute database operations sequentially to avoid DbContext threading issues
        var empUsers = await _empUserService.GetAllEmpUsers();
        var empManagementPageDTOs = new List<AdminEmpManagePageListItemDTO>();
        foreach (var empUser in empUsers)
        {
                var ratingMetrics = await _performanceReviewService.GetEmpUserRatingMetricsByEmpId(empUser.EmployeeId);
                var totalLeaveBalanceSum = await _leaveBalanceService.GetTotalLeaveBalanceSum(empUser.EmployeeId);
            empManagementPageDTOs.Add(new AdminEmpManagePageListItemDTO
            {
                EmpUser = empUser,
                EmpUserRatingMetrics = ratingMetrics,
                TotalLeaveBalanceSum = totalLeaveBalanceSum
            });
        }
        
        return empManagementPageDTOs;
    }
        
} 