using System;
using CoriCore.DTOs;
using CoriCore.DTOs.Page_Specific;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.Interfaces;
using Moq;

namespace CoriCore.Tests.Unit.Services;

public class PageServiceTests
{
    private readonly PageService _service;
    private readonly Mock<IEmpUserService> _mockEmpUserService;
    private readonly Mock<IEquipmentService> _mockEquipmentService;
    private readonly Mock<ILeaveBalanceService> _mockLeaveBalanceService;
    private readonly Mock<IPerformanceReviewService> _mockPerformanceReviewService;
    private readonly Mock<ILeaveRequestService> _mockLeaveRequestService;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly Mock<IGatheringService> _mockGatheringService;

    public PageServiceTests()
    {
        _mockEmpUserService = new Mock<IEmpUserService>();
        _mockEquipmentService = new Mock<IEquipmentService>();
        _mockLeaveBalanceService = new Mock<ILeaveBalanceService>();
        _mockPerformanceReviewService = new Mock<IPerformanceReviewService>();
        _mockLeaveRequestService = new Mock<ILeaveRequestService>();
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockAdminService = new Mock<IAdminService>();
        _mockGatheringService = new Mock<IGatheringService>();

        _service = new PageService(
            _mockEmpUserService.Object,
            _mockEquipmentService.Object,
            _mockLeaveBalanceService.Object,
            _mockPerformanceReviewService.Object,
            _mockLeaveRequestService.Object,
            _mockEmployeeService.Object,
            _mockAdminService.Object,
            _mockGatheringService.Object
        );
    }

    [Fact]
    public async Task GetAdminDashboardPageInfo_CallsAllRequiredServices()
    {
        // Arrange
        var adminId = 1;

        _mockAdminService.Setup(x => x.GetAdminUserByAdminId(adminId))
            .ReturnsAsync((AdminUserDTO)null);
        _mockPerformanceReviewService.Setup(x => x.GetTopEmpUserRatingMetrics(5))
            .ReturnsAsync(new List<EmpUserRatingMetricsDTO>());
        _mockPerformanceReviewService.Setup(x => x.GetTopRatedEmployees())
            .ReturnsAsync(new List<TopRatedEmployeesDTO>());
        _mockEmployeeService.Setup(x => x.GetEmployeeStatusTotals())
            .ReturnsAsync(new EmpTotalStatsDTO());

        // Act
        var result = await _service.GetAdminDashboardPageInfo(adminId);

        // Assert
        Assert.NotNull(result);
        _mockAdminService.Verify(x => x.GetAdminUserByAdminId(adminId), Times.Once);
        _mockPerformanceReviewService.Verify(x => x.GetTopEmpUserRatingMetrics(5), Times.Once);
        _mockPerformanceReviewService.Verify(x => x.GetTopRatedEmployees(), Times.Once);
        _mockEmployeeService.Verify(x => x.GetEmployeeStatusTotals(), Times.Once);
    }

    [Fact]
    public async Task GetAdminEmpDetailsPageInfo_CallsAllRequiredServices()
    {
        // Arrange
        var employeeId = 1;

        _mockEmpUserService.Setup(x => x.GetEmpUserByEmpId(employeeId))
            .ReturnsAsync(new EmpUserDTO());
        _mockEquipmentService.Setup(x => x.GetEquipmentByEmployeeId(employeeId))
            .ReturnsAsync(new List<EquipmentDTO>());
        _mockLeaveBalanceService.Setup(x => x.GetAllLeaveBalancesByEmployeeId(employeeId))
            .ReturnsAsync(new List<LeaveBalanceDTO>());
        _mockPerformanceReviewService.Setup(x => x.GetEmpUserRatingMetricsByEmpId(employeeId))
            .ReturnsAsync(new EmpUserRatingMetricsDTO());
        _mockGatheringService.Setup(x => x.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId))
            .ReturnsAsync(new List<GatheringDTO>());

        // Act
        var result = await _service.GetAdminEmpDetailsPageInfo(employeeId);

        // Assert
        Assert.NotNull(result);
        _mockEmpUserService.Verify(x => x.GetEmpUserByEmpId(employeeId), Times.Once);
        _mockEquipmentService.Verify(x => x.GetEquipmentByEmployeeId(employeeId), Times.Once);
        _mockLeaveBalanceService.Verify(x => x.GetAllLeaveBalancesByEmployeeId(employeeId), Times.Once);
        _mockPerformanceReviewService.Verify(x => x.GetEmpUserRatingMetricsByEmpId(employeeId), Times.Once);
        _mockGatheringService.Verify(x => x.GetAllUpcomingAndCompletedGatheringsByEmployeeIdDescending(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetAdminEmpManagementPageInfo_ProcessesAllEmployees()
    {
        // Arrange
        var empUsers = new List<EmpUserDTO>
        {
            new EmpUserDTO { EmployeeId = 1 },
            new EmpUserDTO { EmployeeId = 2 }
        };

        _mockEmpUserService.Setup(x => x.GetAllEmpUsers())
            .ReturnsAsync(empUsers);
        _mockPerformanceReviewService.Setup(x => x.GetEmpUserRatingMetricsByEmpId(It.IsAny<int>()))
            .ReturnsAsync(new EmpUserRatingMetricsDTO());
        _mockLeaveBalanceService.Setup(x => x.GetTotalLeaveBalanceSum(It.IsAny<int>()))
            .ReturnsAsync(new LeaveBalanceSumDTO());

        // Act
        var result = await _service.GetAdminEmpManagementPageInfo();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockEmpUserService.Verify(x => x.GetAllEmpUsers(), Times.Once);
        _mockPerformanceReviewService.Verify(x => x.GetEmpUserRatingMetricsByEmpId(It.IsAny<int>()), Times.Exactly(2));
        _mockLeaveBalanceService.Verify(x => x.GetTotalLeaveBalanceSum(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetEmployeeProfilePageInfo_CallsAllRequiredServices()
    {
        // Arrange
        var employeeId = 1;

        _mockEmpUserService.Setup(x => x.GetEmpUserByEmpId(employeeId))
            .ReturnsAsync(new EmpUserDTO());
        _mockEquipmentService.Setup(x => x.GetEquipmentByEmployeeId(employeeId))
            .ReturnsAsync(new List<EquipmentDTO>());
        _mockPerformanceReviewService.Setup(x => x.GetEmpUserRatingMetricsByEmpId(employeeId))
            .ReturnsAsync(new EmpUserRatingMetricsDTO());

        // Act
        var result = await _service.GetEmployeeProfilePageInfo(employeeId);

        // Assert
        Assert.NotNull(result);
        _mockEmpUserService.Verify(x => x.GetEmpUserByEmpId(employeeId), Times.Once);
        _mockEquipmentService.Verify(x => x.GetEquipmentByEmployeeId(employeeId), Times.Once);
        _mockPerformanceReviewService.Verify(x => x.GetEmpUserRatingMetricsByEmpId(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeLeaveOverviewPageInfo_CallsAllRequiredServices()
    {
        // Arrange
        var employeeId = 1;

        _mockLeaveRequestService.Setup(x => x.GetLeaveRequestsByEmployeeId(employeeId))
            .ReturnsAsync(new List<LeaveRequestDTO>());
        _mockLeaveBalanceService.Setup(x => x.GetAllLeaveBalancesByEmployeeId(employeeId))
            .ReturnsAsync(new List<LeaveBalanceDTO>());

        // Act
        var result = await _service.GetEmployeeLeaveOverviewPageInfo(employeeId);

        // Assert
        Assert.NotNull(result);
        _mockLeaveRequestService.Verify(x => x.GetLeaveRequestsByEmployeeId(employeeId), Times.Once);
        _mockLeaveBalanceService.Verify(x => x.GetAllLeaveBalancesByEmployeeId(employeeId), Times.Once);
    }
}