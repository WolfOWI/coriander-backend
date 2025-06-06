using System;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoriCore.Tests.Unit.Controllers;

public class EmployeeControllerTests
{
    private readonly EmployeeController _controller;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly AppDbContext _context;

    public EmployeeControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockUserService = new Mock<IUserService>();
        _controller = new EmployeeController(_context, _mockEmployeeService.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task SetupUserAsEmployee_ReturnsStatusCode_FromService()
    {
        // Arrange
        var employeeDto = new EmployeeDto
        {
            PhoneNumber = "",
            UserId = 1,
            EmployDate = DateOnly.FromDateTime(DateTime.Now),
            Department = "IT",
            JobTitle = "Software Developer"
        };
        _mockEmployeeService.Setup(x => x.RegisterEmployeeAsync(employeeDto))
            .ReturnsAsync((201, "Employee created successfully"));

        // Act
        var result = await _controller.SetupUserAsEmployee(employeeDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, statusCodeResult.StatusCode);
        _mockEmployeeService.Verify(x => x.RegisterEmployeeAsync(employeeDto), Times.Once);
    }

    [Fact]
    public async Task ToggleEmpSuspension_ReturnsStatusCode_FromService()
    {
        // Arrange
        var employeeId = 1;
        _mockEmployeeService.Setup(x => x.ToggleEmpSuspensionAsync(employeeId))
            .ReturnsAsync((200, "Employee suspension toggled"));

        // Act
        var result = await _controller.ToggleEmpSuspension(employeeId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockEmployeeService.Verify(x => x.ToggleEmpSuspensionAsync(employeeId), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeStatusTotals_ReturnsOkWithStats()
    {
        // Arrange
        var empStats = new EmpTotalStatsDTO
        {
            TotalEmployees = 10,
            TotalFullTimeEmployees = 6,
            TotalPartTimeEmployees = 2,
            TotalSuspendedEmployees = 2
        };

        _mockEmployeeService.Setup(x => x.GetEmployeeStatusTotals())
            .ReturnsAsync(empStats);

        // Act
        var result = await _controller.GetEmployeeStatusTotals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedStats = Assert.IsType<EmpTotalStatsDTO>(okResult.Value);
        Assert.Equal(10, returnedStats.TotalEmployees);
        _mockEmployeeService.Verify(x => x.GetEmployeeStatusTotals(), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployee_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employeeId = 999;

        // Act
        var result = await _controller.DeleteEmployee(employeeId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockEmployeeService.Verify(x => x.DeleteEmployeeByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEmployee_ReturnsStatusCode_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, UserId = 1 };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        _mockUserService.Setup(x => x.SetUserRoleAsync(employee.UserId, (int)UserRole.Unassigned))
            .ReturnsAsync(200);
        _mockEmployeeService.Setup(x => x.DeleteEmployeeByIdAsync(employee.EmployeeId))
            .ReturnsAsync((200, "Employee deleted successfully"));

        // Act
        var result = await _controller.DeleteEmployee(employee.EmployeeId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockUserService.Verify(x => x.SetUserRoleAsync(employee.UserId, (int)UserRole.Unassigned), Times.Once);
        _mockEmployeeService.Verify(x => x.DeleteEmployeeByIdAsync(employee.EmployeeId), Times.Once);
    }

    [Fact]
    public async Task GetEmployees_ReturnsOkWithEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { EmployeeId = 1, UserId = 1 },
            new Employee { EmployeeId = 2, UserId = 2 }
        };

        _context.Employees.AddRange(employees);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEmployees();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);
        Assert.Equal(2, returnedEmployees.Count());
    }

    [Fact]
    public async Task GetEmployee_ReturnsOk_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, UserId = 1 };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEmployee(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEmployee = Assert.IsType<Employee>(okResult.Value);
        Assert.Equal(employee.EmployeeId, returnedEmployee.EmployeeId);
    }

    [Fact]
    public async Task GetEmployee_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange & Act
        var result = await _controller.GetEmployee(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}