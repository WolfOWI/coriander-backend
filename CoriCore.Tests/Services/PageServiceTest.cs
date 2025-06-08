using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoriCore.Services;
using CoriCore.DTOs.Page_Specific;
using CoriCore.DTOs;
using CoriCore.Interfaces;

public class PageServiceTests
{
    private readonly Mock<IEmpUserService> _empUserService = new();
    private readonly Mock<IEquipmentService> _equipmentService = new();
    private readonly Mock<ILeaveBalanceService> _leaveBalanceService = new();
    private readonly Mock<IPerformanceReviewService> _performanceReviewService = new();
    private readonly Mock<ILeaveRequestService> _leaveRequestService = new();
    private readonly Mock<IEmployeeService> _employeeService = new();
    private readonly Mock<IAdminService> _adminService = new();
    private readonly Mock<IGatheringService> _gatheringService = new();
    private readonly Mock<IEmpLeaveRequestService> _empLeaveRequestService = new();

    private PageService CreateService() => new PageService(
        _empUserService.Object,
        _equipmentService.Object,
        _leaveBalanceService.Object,
        _performanceReviewService.Object,
        _leaveRequestService.Object,
        _employeeService.Object,
        _adminService.Object,
        _gatheringService.Object
    );

    [Fact]
    public async Task GetAdminDashboardPageInfo_ReturnsDashboard()
    {
        // Arrange
        var adminId = 1;
        _adminService.Setup(s => s.GetAdminUserByAdminId(adminId)).ReturnsAsync(new AdminUserDTO());
        _performanceReviewService.Setup(s => s.GetTopEmpUserRatingMetrics(5)).ReturnsAsync(new List<EmpUserRatingMetricsDTO>());
        _performanceReviewService.Setup(s => s.GetTopRatedEmployees()).ReturnsAsync(new List<TopRatedEmployeesDTO>());
        _employeeService.Setup(s => s.GetEmployeeStatusTotals()).ReturnsAsync(new EmpTotalStatsDTO());

        var service = CreateService();

        // Act
        var result = await service.GetAdminDashboardPageInfo(adminId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AdminUser);
        Assert.NotNull(result.EmpUserRatingMetrics);
        Assert.NotNull(result.TopRatedEmployees);
        Assert.NotNull(result.EmployeeStatusTotals);
    }

    [Fact]
    public async Task GetAdminEmpDetailsPageInfo_ReturnsDetails()
    {
        // Arrange
        var employeeId = 1;
        _empUserService.Setup(s => s.GetEmpUserByEmpId(employeeId)).ReturnsAsync(new EmpUserDTO());
        _equipmentService.Setup(s => s.GetEquipmentByEmployeeId(employeeId)).ReturnsAsync(new List<EquipmentDTO>());
        _leaveBalanceService.Setup(s => s.GetAllLeaveBalancesByEmployeeId(employeeId)).ReturnsAsync(new List<LeaveBalanceDTO>());
        _performanceReviewService.Setup(s => s.GetEmpUserRatingMetricsByEmpId(employeeId)).ReturnsAsync(new EmpUserRatingMetricsDTO());
        _gatheringService.Setup(s => s.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId)).ReturnsAsync(new List<GatheringDTO>());

        var service = CreateService();

        // Act
        var result = await service.GetAdminEmpDetailsPageInfo(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.EmpUser);
        Assert.NotNull(result.Equipment);
        Assert.NotNull(result.LeaveBalances);
        Assert.NotNull(result.EmpUserRatingMetrics);
        Assert.NotNull(result.Gatherings);
    }

    [Fact]
    public async Task GetAdminEmpManagementPageInfo_ReturnsList()
    {
        // Arrange
        var empUsers = new List<EmpUserDTO> { new EmpUserDTO { EmployeeId = 1 } };
        _empUserService.Setup(s => s.GetAllEmpUsers()).ReturnsAsync(empUsers);
        _performanceReviewService.Setup(s => s.GetEmpUserRatingMetricsByEmpId(It.IsAny<int>())).ReturnsAsync(new EmpUserRatingMetricsDTO());

        var service = CreateService();

        // Act
        var result = await service.GetAdminEmpManagementPageInfo();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.NotNull(result[0].EmpUser);
        Assert.NotNull(result[0].EmpUserRatingMetrics);
    }

    [Fact]
    public async Task GetEmployeeProfilePageInfo_ReturnsProfile()
    {
        // Arrange
        var employeeId = 1;
        _empUserService.Setup(s => s.GetEmpUserByEmpId(employeeId)).ReturnsAsync(new EmpUserDTO());
        _equipmentService.Setup(s => s.GetEquipmentByEmployeeId(employeeId)).ReturnsAsync(new List<EquipmentDTO>());
        _performanceReviewService.Setup(s => s.GetEmpUserRatingMetricsByEmpId(employeeId)).ReturnsAsync(new EmpUserRatingMetricsDTO());

        var service = CreateService();

        // Act
        var result = await service.GetEmployeeProfilePageInfo(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.EmpUser);
        Assert.NotNull(result.Equipment);
        Assert.NotNull(result.EmpUserRatingMetrics);
    }

    [Fact]
    public async Task GetEmployeeLeaveOverviewPageInfo_ReturnsOverview()
    {
        // Arrange
        var employeeId = 1;
        _leaveRequestService.Setup(s => s.GetLeaveRequestsByEmployeeId(employeeId)).ReturnsAsync(new List<LeaveRequestDTO>());
        _leaveBalanceService.Setup(s => s.GetAllLeaveBalancesByEmployeeId(employeeId)).ReturnsAsync(new List<LeaveBalanceDTO>());

        var service = CreateService();

        // Act
        var result = await service.GetEmployeeLeaveOverviewPageInfo(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.LeaveRequests);
        Assert.NotNull(result.LeaveBalances);
    }
}