using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoriCore.Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for EmployeeService - focusing on business logic and validation
    /// These tests demonstrate how to test complex service methods with multiple dependencies
    /// </summary>
    public class EmployeeServiceTests
    {
        private readonly EmployeeService _employeeService;
        private readonly AppDbContext _context;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILeaveBalanceService> _mockLeaveBalanceService;
        private readonly Mock<IEquipmentService> _mockEquipmentService;
        private readonly Mock<IEmailService> _mockEmailService;

        public EmployeeServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);

            // Setup mocks for all dependencies
            _mockUserService = new Mock<IUserService>();
            _mockLeaveBalanceService = new Mock<ILeaveBalanceService>();
            _mockEquipmentService = new Mock<IEquipmentService>();
            _mockEmailService = new Mock<IEmailService>();

            // Create the service with dependencies
            _employeeService = new EmployeeService(
                _context,
                _mockUserService.Object,
                _mockLeaveBalanceService.Object,
                _mockEquipmentService.Object,
                _mockEmailService.Object
            );
        }

        #region ValidateEmployeeInfoAsync Tests

        [Fact]
        public async Task ValidateEmployeeInfoAsync_ReturnsSuccess_WhenAllFieldsAreValid()
        {
            // Arrange - Create a valid employee DTO
            var validEmployeeDto = new EmployeeDto
            {
                UserId = 1,
                PhoneNumber = "123-456-7890",
                JobTitle = "Software Developer",
                Department = "IT",
                Gender = Gender.Male,
                DateOfBirth = new DateOnly(1990, 1, 1),
                SalaryAmount = 50000,
                PayCycle = PayCycle.Monthly,
                EmployType = EmployType.FullTime,
                EmployDate = DateOnly.FromDateTime(DateTime.Now)
            };

            // Act - Call the validation method
            var result = await _employeeService.ValidateEmployeeInfoAsync(validEmployeeDto);

            // Assert - Should return success
            Assert.Equal(201, result.Code);
            Assert.Equal("Validation successful", result.Message);
        }

        [Fact]
        public async Task ValidateEmployeeInfoAsync_ReturnsError_WhenEmployeeDtoIsNull()
        {
            // Arrange - Pass null DTO
            EmployeeDto nullDto = null;

            // Act
            var result = await _employeeService.ValidateEmployeeInfoAsync(nullDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Invalid Employee data", result.Message);
        }

        [Fact]
        public async Task ValidateEmployeeInfoAsync_ReturnsError_WhenUserIdIsInvalid()
        {
            // Arrange - Create DTO with invalid UserId
            var invalidEmployeeDto = new EmployeeDto
            {
                UserId = 0, // Invalid - should be > 0
                PhoneNumber = "123-456-7890",
                JobTitle = "Software Developer",
                Department = "IT"
            };

            // Act
            var result = await _employeeService.ValidateEmployeeInfoAsync(invalidEmployeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Missing or invalid UserId", result.Message);
        }

        [Fact]
        public async Task ValidateEmployeeInfoAsync_ReturnsError_WhenPhoneNumberIsEmpty()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                UserId = 1,
                PhoneNumber = "", // Empty phone number
                JobTitle = "Software Developer",
                Department = "IT"
            };

            // Act
            var result = await _employeeService.ValidateEmployeeInfoAsync(employeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Phone number is required", result.Message);
        }

        [Fact]
        public async Task ValidateEmployeeInfoAsync_ReturnsError_WhenJobTitleIsEmpty()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                UserId = 1,
                PhoneNumber = "123-456-7890",
                JobTitle = "", // Empty job title
                Department = "IT"
            };

            // Act
            var result = await _employeeService.ValidateEmployeeInfoAsync(employeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Job title is required", result.Message);
        }

        [Fact]
        public async Task ValidateEmployeeInfoAsync_ReturnsError_WhenDepartmentIsEmpty()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                UserId = 1,
                PhoneNumber = "123-456-7890",
                JobTitle = "Software Developer",
                Department = "" // Empty department
            };

            // Act
            var result = await _employeeService.ValidateEmployeeInfoAsync(employeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Department is required", result.Message);
        }

        #endregion

        #region CreateEmployeeAsync Tests

        [Fact]
        public async Task CreateEmployeeAsync_CreatesEmployee_WhenValidationPasses()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();

            // Setup mocks to return success
            _mockLeaveBalanceService.Setup(x => x.CreateDefaultLeaveBalances(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeService.CreateEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(201, result.Code);
            Assert.Equal("Employee successfully registered", result.Message);

            // Verify employee was added to database
            var employeeInDb = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == employeeDto.UserId);
            Assert.NotNull(employeeInDb);
            Assert.Equal(employeeDto.JobTitle, employeeInDb.JobTitle);
            Assert.Equal(employeeDto.Department, employeeInDb.Department);
            Assert.False(employeeInDb.IsSuspended); // Should default to false
        }

        [Fact]
        public async Task CreateEmployeeAsync_ReturnsError_WhenValidationFails()
        {
            // Arrange - Create invalid DTO (missing job title)
            var invalidEmployeeDto = new EmployeeDto
            {
                UserId = 1,
                PhoneNumber = "123-456-7890",
                JobTitle = "", // Invalid
                Department = "IT"
            };

            // Act
            var result = await _employeeService.CreateEmployeeAsync(invalidEmployeeDto);

            // Assert - Should return validation error
            Assert.Equal(400, result.Code);
            Assert.Equal("Job title is required", result.Message);

            // Verify no employee was created
            var employeeCount = await _context.Employees.CountAsync();
            Assert.Equal(0, employeeCount);
        }

        [Fact]
        public async Task CreateEmployeeAsync_ReturnsError_WhenLeaveBalanceCreationFails()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();

            // Setup mock to simulate leave balance creation failure
            _mockLeaveBalanceService.Setup(x => x.CreateDefaultLeaveBalances(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _employeeService.CreateEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Failed to create default leave balances", result.Message);
        }

        [Fact]
        public async Task CreateEmployeeAsync_AssignsEquipment_WhenEquipmentIdsProvided()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();
            employeeDto.EquipmentIds = new List<int> { 1, 2, 3 };

            // Setup mocks
            _mockLeaveBalanceService.Setup(x => x.CreateDefaultLeaveBalances(It.IsAny<int>()))
                .ReturnsAsync(true);
            _mockEquipmentService.Setup(x => x.AssignEquipmentAsync(It.IsAny<int>(), It.IsAny<List<int>>()))
                .ReturnsAsync((200, "Equipment assigned successfully"));

            // Act
            var result = await _employeeService.CreateEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(201, result.Code);

            // Verify equipment assignment was called
            _mockEquipmentService.Verify(x => x.AssignEquipmentAsync(It.IsAny<int>(), employeeDto.EquipmentIds), Times.Once);
        }

        [Fact]
        public async Task CreateEmployeeAsync_SendsEmail_WhenUserIsVerified()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();
            var verifiedUser = new User
            {
                UserId = employeeDto.UserId,
                Email = "test@example.com",
                FullName = "Test User",
                IsVerified = true
            };

            _context.Users.Add(verifiedUser);
            await _context.SaveChangesAsync();

            _mockLeaveBalanceService.Setup(x => x.CreateDefaultLeaveBalances(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeService.CreateEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(201, result.Code);

            // Verify email was sent
            _mockEmailService.Verify(x => x.SendAccountActivatedEmailAsync(
                verifiedUser.Email,
                verifiedUser.FullName,
                It.IsAny<List<string>>()), Times.Once);
        }

        #endregion

        #region RegisterEmployeeAsync Tests

        [Fact]
        public async Task RegisterEmployeeAsync_RegistersEmployee_WhenAllValidationsPass()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();

            // Setup mocks for success scenario
            _mockUserService.Setup(x => x.EmployeeAdminExistsAsync(employeeDto.UserId))
                .ReturnsAsync(200); // User doesn't exist as employee/admin
            _mockUserService.Setup(x => x.SetUserRoleAsync(employeeDto.UserId, (int)UserRole.Employee))
                .ReturnsAsync(201);
            _mockLeaveBalanceService.Setup(x => x.CreateDefaultLeaveBalances(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeService.RegisterEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(201, result.Code);
            Assert.Equal("Employee successfully registered", result.Message);

            // Verify all service calls were made
            _mockUserService.Verify(x => x.EmployeeAdminExistsAsync(employeeDto.UserId), Times.Once);
            _mockUserService.Verify(x => x.SetUserRoleAsync(employeeDto.UserId, (int)UserRole.Employee), Times.Once);
        }

        [Fact]
        public async Task RegisterEmployeeAsync_ReturnsError_WhenUserAlreadyExistsAsEmployeeOrAdmin()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();

            // Setup mock to simulate user already exists as employee/admin
            _mockUserService.Setup(x => x.EmployeeAdminExistsAsync(employeeDto.UserId))
                .ReturnsAsync(400);

            // Act
            var result = await _employeeService.RegisterEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Contains("is assigned as an Admin or Employee", result.Message);
        }

        [Fact]
        public async Task RegisterEmployeeAsync_ReturnsError_WhenRoleSettingFails()
        {
            // Arrange
            var employeeDto = CreateValidEmployeeDto();

            _mockUserService.Setup(x => x.EmployeeAdminExistsAsync(employeeDto.UserId))
                .ReturnsAsync(200);
            _mockUserService.Setup(x => x.SetUserRoleAsync(employeeDto.UserId, (int)UserRole.Employee))
                .ReturnsAsync(400); // Role setting fails

            // Act
            var result = await _employeeService.RegisterEmployeeAsync(employeeDto);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Failed to set user role", result.Message);
        }

        #endregion

        #region ToggleEmpSuspensionAsync Tests

        [Fact]
        public async Task ToggleEmpSuspensionAsync_SuspendsEmployee_WhenEmployeeIsNotSuspended()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                UserId = 1,
                JobTitle = "Developer",
                Department = "IT",
                PhoneNumber = "123-456-7890",
                IsSuspended = false // Currently not suspended
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _employeeService.ToggleEmpSuspensionAsync(employee.EmployeeId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Contains("suspended", result.Message);

            // Verify suspension status changed
            var updatedEmployee = await _context.Employees.FindAsync(employee.EmployeeId);
            Assert.True(updatedEmployee.IsSuspended);
        }

        [Fact]
        public async Task ToggleEmpSuspensionAsync_UnsuspendsEmployee_WhenEmployeeIsSuspended()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                UserId = 1,
                JobTitle = "Developer",
                Department = "IT",
                PhoneNumber = "123-456-7890",
                IsSuspended = true // Currently suspended
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _employeeService.ToggleEmpSuspensionAsync(employee.EmployeeId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Contains("unsuspended", result.Message);

            // Verify suspension status changed
            var updatedEmployee = await _context.Employees.FindAsync(employee.EmployeeId);
            Assert.False(updatedEmployee.IsSuspended);
        }

        [Fact]
        public async Task ToggleEmpSuspensionAsync_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange - No employee with this ID exists
            int nonExistentEmployeeId = 999;

            // Act
            var result = await _employeeService.ToggleEmpSuspensionAsync(nonExistentEmployeeId);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Equal("Employee not found", result.Message);
        }

        #endregion

        #region DeleteEmployeeByIdAsync Tests

        [Fact]
        public async Task DeleteEmployeeByIdAsync_DeletesEmployee_WhenEmployeeExists()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                UserId = 1,
                JobTitle = "Developer",
                Department = "IT",
                PhoneNumber = "123-456-7890"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Setup mock for equipment unlinking
            _mockEquipmentService.Setup(x => x.MassUnlinkEquipmentFromEmployee(employee.EmployeeId))
                .ReturnsAsync((200, "Equipment unlinked successfully"));

            // Act
            var result = await _employeeService.DeleteEmployeeByIdAsync(employee.EmployeeId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Employee deleted successfully", result.Message);

            // Verify employee was deleted
            var deletedEmployee = await _context.Employees.FindAsync(employee.EmployeeId);
            Assert.Null(deletedEmployee);

            // Verify equipment unlinking was called
            _mockEquipmentService.Verify(x => x.MassUnlinkEquipmentFromEmployee(employee.EmployeeId), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployeeByIdAsync_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange - No employee exists
            int nonExistentEmployeeId = 999;

            // Act
            var result = await _employeeService.DeleteEmployeeByIdAsync(nonExistentEmployeeId);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Equal("Employee not found", result.Message);
        }

        [Fact]
        public async Task DeleteEmployeeByIdAsync_ReturnsError_WhenEquipmentUnlinkingFails()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                UserId = 1,
                JobTitle = "Developer",
                Department = "IT",
                PhoneNumber = "123-456-7890"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Setup mock to simulate equipment unlinking failure
            _mockEquipmentService.Setup(x => x.MassUnlinkEquipmentFromEmployee(employee.EmployeeId))
                .ReturnsAsync((400, "Failed to unlink equipment"));

            // Act
            var result = await _employeeService.DeleteEmployeeByIdAsync(employee.EmployeeId);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Contains("Failed to unlink equipment", result.Message);

            // Verify employee was not deleted
            var stillExistsEmployee = await _context.Employees.FindAsync(employee.EmployeeId);
            Assert.NotNull(stillExistsEmployee);
        }

        #endregion

        #region GetEmployeeStatusTotals Tests

        [Fact]
        public async Task GetEmployeeStatusTotals_ReturnsCorrectCounts_WithMixedEmployeeTypes()
        {
            // Arrange - Create employees with different types and suspension statuses
            var employees = new List<Employee>
            {
                CreateEmployee(1, EmployType.FullTime, false),
                CreateEmployee(2, EmployType.FullTime, false),
                CreateEmployee(3, EmployType.PartTime, false),
                CreateEmployee(4, EmployType.Contract, false),
                CreateEmployee(5, EmployType.Intern, false),
                CreateEmployee(6, EmployType.FullTime, true), // Suspended
                CreateEmployee(7, EmployType.PartTime, true)  // Suspended
            };

            _context.Employees.AddRange(employees);
            await _context.SaveChangesAsync();

            // Act
            var result = await _employeeService.GetEmployeeStatusTotals();

            // Assert
            Assert.Equal(7, result.TotalEmployees); // All employees
            Assert.Equal(2, result.TotalFullTimeEmployees); // Only non-suspended full-time
            Assert.Equal(1, result.TotalPartTimeEmployees); // Only non-suspended part-time
            Assert.Equal(1, result.TotalContractEmployees); // Only non-suspended contract
            Assert.Equal(1, result.TotalInternEmployees); // Only non-suspended intern
            Assert.Equal(2, result.TotalSuspendedEmployees); // All suspended employees
        }

        [Fact]
        public async Task GetEmployeeStatusTotals_ReturnsZeros_WhenNoEmployeesExist()
        {
            // Arrange - No employees in database

            // Act
            var result = await _employeeService.GetEmployeeStatusTotals();

            // Assert
            Assert.Equal(0, result.TotalEmployees);
            Assert.Equal(0, result.TotalFullTimeEmployees);
            Assert.Equal(0, result.TotalPartTimeEmployees);
            Assert.Equal(0, result.TotalContractEmployees);
            Assert.Equal(0, result.TotalInternEmployees);
            Assert.Equal(0, result.TotalSuspendedEmployees);
        }

        [Fact]
        public async Task GetEmployeeStatusTotals_ExcludesSuspendedEmployees_FromTypeCounts()
        {
            // Arrange - Create only suspended employees
            var suspendedEmployees = new List<Employee>
            {
                CreateEmployee(1, EmployType.FullTime, true),
                CreateEmployee(2, EmployType.PartTime, true),
                CreateEmployee(3, EmployType.Contract, true)
            };

            _context.Employees.AddRange(suspendedEmployees);
            await _context.SaveChangesAsync();

            // Act
            var result = await _employeeService.GetEmployeeStatusTotals();

            // Assert
            Assert.Equal(3, result.TotalEmployees); // All employees counted
            Assert.Equal(0, result.TotalFullTimeEmployees); // Suspended employees excluded
            Assert.Equal(0, result.TotalPartTimeEmployees); // Suspended employees excluded
            Assert.Equal(0, result.TotalContractEmployees); // Suspended employees excluded
            Assert.Equal(0, result.TotalInternEmployees); // Suspended employees excluded
            Assert.Equal(3, result.TotalSuspendedEmployees); // All are suspended
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a valid EmployeeDto for testing purposes
        /// </summary>
        private static EmployeeDto CreateValidEmployeeDto()
        {
            return new EmployeeDto
            {
                UserId = 1,
                Gender = Gender.Male,
                DateOfBirth = new DateOnly(1990, 1, 1),
                PhoneNumber = "123-456-7890",
                JobTitle = "Software Developer",
                Department = "IT",
                SalaryAmount = 50000,
                PayCycle = PayCycle.Monthly,
                EmployType = EmployType.FullTime,
                EmployDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        /// <summary>
        /// Creates an Employee entity for testing
        /// </summary>
        private static Employee CreateEmployee(int userId, EmployType employType, bool isSuspended)
        {
            return new Employee
            {
                UserId = userId,
                Gender = Gender.Male,
                DateOfBirth = new DateOnly(1990, 1, 1),
                PhoneNumber = $"123-456-789{userId}",
                JobTitle = "Test Job",
                Department = "Test Department",
                SalaryAmount = 50000,
                PayCycle = PayCycle.Monthly,
                EmployType = employType,
                EmployDate = DateOnly.FromDateTime(DateTime.Now),
                IsSuspended = isSuspended
            };
        }

        #endregion
    }
}