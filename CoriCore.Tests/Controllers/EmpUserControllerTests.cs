using System;
using CoriCore.Controllers;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class EmpUserControllerTests
{
    private readonly EmpUserController _controller;
    private readonly Mock<IEmpUserService> _mockEmpUserService;

    public EmpUserControllerTests()
    {
        _mockEmpUserService = new Mock<IEmpUserService>();
        _controller = new EmpUserController(_mockEmpUserService.Object);
    }

    [Fact]
    public async Task GetAllEmpUsers_ReturnsOkWithEmpUsers()
    {
        // Arrange
        var empUsers = new List<EmpUserDTO>
        {
            new EmpUserDTO { EmployeeId = 1, FullName = "John Doe" },
            new EmpUserDTO { EmployeeId = 2, FullName = "Jane Smith" }
        };

        _mockEmpUserService.Setup(x => x.GetAllEmpUsers())
            .ReturnsAsync(empUsers);

        // Act
        var result = await _controller.GetAllEmpUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEmpUsers = Assert.IsAssignableFrom<IEnumerable<EmpUserDTO>>(okResult.Value);
        Assert.Equal(2, returnedEmpUsers.Count());
        _mockEmpUserService.Verify(x => x.GetAllEmpUsers(), Times.Once);
    }

    [Fact]
    public async Task GetEmpUserById_ReturnsOkWithEmpUser_WhenEmployeeExists()
    {
        // Arrange
        var employeeId = 1;
        var empUser = new EmpUserDTO
        {
            EmployeeId = employeeId,
            FullName = "John Doe",
            Email = "john@example.com"
        };

        _mockEmpUserService.Setup(x => x.GetEmpUserByEmpId(employeeId))
            .ReturnsAsync(empUser);

        // Act
        var result = await _controller.GetEmpUserById(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEmpUser = Assert.IsType<EmpUserDTO>(okResult.Value);
        Assert.Equal(employeeId, returnedEmpUser.EmployeeId);
        Assert.Equal("John Doe", returnedEmpUser.FullName);
        _mockEmpUserService.Verify(x => x.GetEmpUserByEmpId(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetEmpUserById_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employeeId = 999;
        var exceptionMessage = "Employee not found";

        _mockEmpUserService.Setup(x => x.GetEmpUserByEmpId(employeeId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetEmpUserById(employeeId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(exceptionMessage, notFoundResult.Value);
        _mockEmpUserService.Verify(x => x.GetEmpUserByEmpId(employeeId), Times.Once);
    }

    [Fact]
    public async Task EditEmpUserDetailsById_ReturnsCorrectStatusCode_WhenUpdateSuccessful()
    {
        // Arrange
        var employeeId = 1;
        var updateDto = new EmployeeUpdateDTO
        {
            JobTitle = "Senior Developer",
            Department = "IT"
        };

        var serviceResult = (Code: 200, Message: "Employee updated successfully");

        _mockEmpUserService.Setup(x => x.UpdateEmpUserDetailsByIdAsync(employeeId, updateDto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.EditEmpUserDetailsById(employeeId, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockEmpUserService.Verify(x => x.UpdateEmpUserDetailsByIdAsync(employeeId, updateDto), Times.Once);
    }

    [Fact]
    public async Task EditEmpUserDetailsById_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employeeId = 999;
        var updateDto = new EmployeeUpdateDTO
        {
            JobTitle = "Senior Developer"
        };

        var serviceResult = (Code: 404, Message: "Employee not found");

        _mockEmpUserService.Setup(x => x.UpdateEmpUserDetailsByIdAsync(employeeId, updateDto))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.EditEmpUserDetailsById(employeeId, updateDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, statusCodeResult.StatusCode);
        _mockEmpUserService.Verify(x => x.UpdateEmpUserDetailsByIdAsync(employeeId, updateDto), Times.Once);
    }

    [Fact]
    public async Task GetAllEmpsEquipStats_ReturnsOkWithEquipStats()
    {
        // Arrange
        var comparedEquipId = 1;
        var equipStats = new List<EmpUserEquipStatsDTO>
        {
            new EmpUserEquipStatsDTO
            {
                EmployeeId = 1,
                FullName = "John Doe"
            },
            new EmpUserEquipStatsDTO
            {
                EmployeeId = 2,
                FullName = "Jane Smith"
            }
        };

        _mockEmpUserService.Setup(x => x.GetAllEmpsEquipStats(comparedEquipId))
            .ReturnsAsync(equipStats);

        // Act
        var result = await _controller.GetAllEmpsEquipStats(comparedEquipId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStats = Assert.IsAssignableFrom<IEnumerable<EmpUserEquipStatsDTO>>(okResult.Value);
        Assert.Equal(2, returnedStats.Count());
        _mockEmpUserService.Verify(x => x.GetAllEmpsEquipStats(comparedEquipId), Times.Once);
    }

    [Fact]
    public async Task GetAllEmpsEquipStats_ReturnsEmptyList_WhenNoEmployeesExist()
    {
        // Arrange
        var comparedEquipId = 1;
        var emptyStats = new List<EmpUserEquipStatsDTO>();

        _mockEmpUserService.Setup(x => x.GetAllEmpsEquipStats(comparedEquipId))
            .ReturnsAsync(emptyStats);

        // Act
        var result = await _controller.GetAllEmpsEquipStats(comparedEquipId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStats = Assert.IsAssignableFrom<IEnumerable<EmpUserEquipStatsDTO>>(okResult.Value);
        Assert.Empty(returnedStats);
        _mockEmpUserService.Verify(x => x.GetAllEmpsEquipStats(comparedEquipId), Times.Once);
    }
}