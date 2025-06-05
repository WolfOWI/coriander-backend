using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using CoriCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CoriCore.Tests.Unit.Services;

public class EmployeeServiceTests
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _service;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILeaveBalanceService> _mockLeaveBalanceService;
    private readonly Mock<IEquipmentService> _mockEquipmentService;
    private readonly Mock<IEmailService> _mockEmailService;

    public EmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);

        _mockUserService = new Mock<IUserService>();
        _mockLeaveBalanceService = new Mock<ILeaveBalanceService>();
        _mockEquipmentService = new Mock<IEquipmentService>();
        _mockEmailService = new Mock<IEmailService>();

        _service = new EmployeeService(
            _context,
            _mockUserService.Object,
            _mockLeaveBalanceService.Object,
            _mockEquipmentService.Object,
            _mockEmailService.Object
        );
    }

    [Fact]
    public async Task ValidateEmployeeInfoAsync_ReturnsCode400_WhenEmployeeDtoIsNull()
    {
        // Arrange
        EmployeeDto employeeDto = null;

        // Act
        var result = await _service.ValidateEmployeeInfoAsync(employeeDto);

        // Assert
        Assert.Equal(400, result.Code);
        Assert.Equal("Invalid Employee data", result.Message);
    }

    [Fact]
    public async Task ValidateEmployeeInfoAsync_ReturnsCode400_WhenPhoneNumberMissing()
    {
        // Arrange
        var employeeDto = new EmployeeDto
        {
            UserId = 1,
            PhoneNumber = "", // EMPTY!
            JobTitle = "Developer",
            Department = "IT"
        };

        // Act
        var result = await _service.ValidateEmployeeInfoAsync(employeeDto);

        // Assert
        Assert.Equal(400, result.Code);
        Assert.Equal("Phone number is required", result.Message);
    }

    [Fact]
    public async Task ValidateEmployeeInfoAsync_ReturnsCode201_WhenValidData()
    {
        // Arrange
        var employeeDto = new EmployeeDto
        {
            UserId = 1,
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime
        };

        // Act
        var result = await _service.ValidateEmployeeInfoAsync(employeeDto);

        // Assert
        Assert.Equal(201, result.Code);
        Assert.Equal("Validation successful", result.Message);
    }

    [Fact]
    public async Task CreateEmployeeAsync_CreatesEmployee_WhenValidData()
    {
        // Arrange
        var employeeDto = new EmployeeDto
        {
            UserId = 1,
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime
        };

        _mockLeaveBalanceService.Setup(x => x.CreateDefaultLeaveBalances(It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.CreateEmployeeAsync(employeeDto);

        // Assert
        Assert.Equal(201, result.Code);
        Assert.Equal("Employee successfully registered", result.Message);

        var employeeInDb = await _context.Employees.FirstAsync();
        Assert.Equal(employeeDto.UserId, employeeInDb.UserId);
        Assert.Equal(employeeDto.PhoneNumber, employeeInDb.PhoneNumber);
        Assert.Equal(employeeDto.JobTitle, employeeInDb.JobTitle);
        Assert.False(employeeInDb.IsSuspended);
    }

    [Fact]
    public async Task RegisterEmployeeAsync_ReturnsCode400_WhenUserAlreadyLinked()
    {
        // Arrange
        var employeeDto = new EmployeeDto
        {
            UserId = 1,
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT"
        };

        _mockUserService.Setup(x => x.EmployeeAdminExistsAsync(1))
            .ReturnsAsync(400);

        // Act
        var result = await _service.RegisterEmployeeAsync(employeeDto);

        // Assert
        Assert.Equal(400, result.Code);
        Assert.Contains("assigned as an Admin or Employee", result.Message);
    }

    [Fact]
    public async Task ToggleEmpSuspensionAsync_ReturnsCode404_WhenEmployeeNotFound()
    {
        // Arrange
        var employeeId = 999;

        // Act
        var result = await _service.ToggleEmpSuspensionAsync(employeeId);

        // Assert
        Assert.Equal(404, result.Code);
        Assert.Equal("Employee not found", result.Message);
    }

    [Fact]
    public async Task ToggleEmpSuspensionAsync_TogglesSuspension_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = 1,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ToggleEmpSuspensionAsync(1);

        // Assert
        Assert.Equal(200, result.Code);
        Assert.Contains("suspended", result.Message);

        var updatedEmployee = await _context.Employees.FindAsync(1);
        Assert.True(updatedEmployee.IsSuspended);
    }

    [Fact]
    public async Task DeleteEmployeeByIdAsync_ReturnsCode404_WhenEmployeeNotFound()
    {
        // Arrange
        var employeeId = 999;

        // Act
        var result = await _service.DeleteEmployeeByIdAsync(employeeId);

        // Assert
        Assert.Equal(404, result.Code);
        Assert.Equal("Employee not found", result.Message);
    }

    [Fact]
    public async Task DeleteEmployeeByIdAsync_DeletesEmployee_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeId = 1,
            UserId = 1,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        _mockEquipmentService.Setup(x => x.MassUnlinkEquipmentFromEmployee(1))
            .ReturnsAsync((200, "Success"));

        // Act
        var result = await _service.DeleteEmployeeByIdAsync(1);

        // Assert
        Assert.Equal(200, result.Code);
        Assert.Equal("Employee deleted successfully", result.Message);

        var deletedEmployee = await _context.Employees.FindAsync(1);
        Assert.Null(deletedEmployee);
    }

    [Fact]
    public async Task GetEmployeeStatusTotals_ReturnsCorrectCounts()
    {
        // Arrange
        var activeEmployee = new Employee
        {
            EmployeeId = 1,
            UserId = 1,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Developer",
            Department = "IT",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        var suspendedEmployee = new Employee
        {
            EmployeeId = 2,
            UserId = 2,
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1992, 3, 15),
            PhoneNumber = "0987654321",
            JobTitle = "Designer",
            Department = "Design",
            SalaryAmount = 45000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2021, 6, 1),
            EmployType = EmployType.PartTime,
            IsSuspended = true
        };

        _context.Employees.AddRange(activeEmployee, suspendedEmployee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetEmployeeStatusTotals();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalEmployees);
        Assert.Equal(1, result.TotalSuspendedEmployees);
    }
}