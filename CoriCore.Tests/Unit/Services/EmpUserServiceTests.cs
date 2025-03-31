using System;
using CoriCore.Data;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Tests.Unit.Services;

public class EmpUserServiceTests
{
    private readonly AppDbContext _context;        // Test database
    private readonly EmpUserService _service;      // EmpUserService (to test)

    // Constructor (runs before each test)
    public EmpUserServiceTests()
    {
        // Options for our test database
        var options = new DbContextOptionsBuilder<AppDbContext>()  // Create builder for database options
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())  // Use an in-memory database with a unique name
            .Options;  // Get the final options object

        // Create temporary database in memory just for testing
        _context = new AppDbContext(options);  // Create a new database context with our options

        // Create service using test database
        _service = new EmpUserService(_context);
    }

    // TESTS
    // ================================================================================================================
    // GetAllEmpUsers & returns list of EmpUsers
    [Fact]
    public async Task GetAllEmpUsers_ReturnsListOfEmpUsers()
    {
        // Arrange
        // ------------------------------------------------------------
        // Create multiple test users
        var user1 = new User
        {
            UserId = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            Role = UserRole.Employee
        };

        var user2 = new User
        {
            UserId = 2,
            FullName = "Jane Smith",
            Email = "jane@example.com",
            Role = UserRole.Employee
        };

        // Create multiple test employees
        var employee1 = new Employee
        {
            EmployeeId = 1,
            User = user1,
            UserId = user1.UserId,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            JobTitle = "Dev",
            Department = "Engineering",
            SalaryAmount = 50000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2020, 1, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        var employee2 = new Employee
        {
            EmployeeId = 2,
            User = user2,
            UserId = user2.UserId,
            Gender = Gender.Female,
            DateOfBirth = new DateOnly(1992, 3, 15),
            PhoneNumber = "0987654321",
            JobTitle = "QA",
            Department = "Testing",
            SalaryAmount = 45000,
            PayCycle = PayCycle.Monthly,
            EmployDate = new DateOnly(2021, 6, 1),
            EmployType = EmployType.FullTime,
            IsSuspended = false
        };

        // Add test data to database
        _context.Users.Add(user1);
        _context.Users.Add(user2);
        _context.Employees.Add(employee1);
        _context.Employees.Add(employee2);
        await _context.SaveChangesAsync();

        // ------------------------------------------------------------

        // Act (Do what we want to test)
        // ------------------------------------------------------------
        // Call our service to get all employees
        var result = await _service.GetAllEmpUsers();  // Get all employees from our service
        // ------------------------------------------------------------

        // Assert (Check if it worked)
        // ------------------------------------------------------------
        // Make sure we got something back (not null)
        Assert.NotNull(result);  // Check that we got a result (not null)
        
        // Make sure we got exactly two employees back
        Assert.Equal(2, result.Count);  // Check that we got exactly two results
        
        // Get both employees from the result
        var firstEmp = result.First(e => e.UserId == user1.UserId);
        var secondEmp = result.First(e => e.UserId == user2.UserId);
        
        // Check if all the user information is correct for first employee
        Assert.Equal(user1.UserId, firstEmp.UserId);
        Assert.Equal(user1.FullName, firstEmp.FullName);
        Assert.Equal(user1.Email, firstEmp.Email);
        Assert.Equal(user1.Role, firstEmp.Role);
        
        // Check if all the employee information is correct for first employee
        Assert.Equal(employee1.Gender, firstEmp.Gender);
        Assert.Equal(employee1.DateOfBirth, firstEmp.DateOfBirth);
        Assert.Equal(employee1.PhoneNumber, firstEmp.PhoneNumber);
        Assert.Equal(employee1.JobTitle, firstEmp.JobTitle);
        Assert.Equal(employee1.Department, firstEmp.Department);
        Assert.Equal(employee1.SalaryAmount, firstEmp.SalaryAmount);
        Assert.Equal(employee1.PayCycle, firstEmp.PayCycle);
        Assert.Equal(employee1.EmployDate, firstEmp.EmployDate);
        Assert.Equal(employee1.EmployType, firstEmp.EmployType);
        Assert.Equal(employee1.IsSuspended, firstEmp.IsSuspended);

        // Check if all the user information is correct for second employee
        Assert.Equal(user2.UserId, secondEmp.UserId);
        Assert.Equal(user2.FullName, secondEmp.FullName);
        Assert.Equal(user2.Email, secondEmp.Email);
        Assert.Equal(user2.Role, secondEmp.Role);
        
        // Check if all the employee information is correct for second employee
        Assert.Equal(employee2.Gender, secondEmp.Gender);
        Assert.Equal(employee2.DateOfBirth, secondEmp.DateOfBirth);
        Assert.Equal(employee2.PhoneNumber, secondEmp.PhoneNumber);
        Assert.Equal(employee2.JobTitle, secondEmp.JobTitle);
        Assert.Equal(employee2.Department, secondEmp.Department);
        Assert.Equal(employee2.SalaryAmount, secondEmp.SalaryAmount);
        Assert.Equal(employee2.PayCycle, secondEmp.PayCycle);
        Assert.Equal(employee2.EmployDate, secondEmp.EmployDate);
        Assert.Equal(employee2.EmployType, secondEmp.EmployType);
        Assert.Equal(employee2.IsSuspended, secondEmp.IsSuspended);
        // ------------------------------------------------------------
    }

    // GetAllEmpUsers & returns empty list when no employees in the database
    [Fact]
    public async Task GetAllEmpUsers_ReturnsEmptyList_WhenNoEmployeesExist()
    {
        // Arrange
        // ------------------------------------------------------------
        // No data in the database
        // ------------------------------------------------------------

        // Act
        // ------------------------------------------------------------
        // Call our service to get all employees
        var result = await _service.GetAllEmpUsers();  // Get all employees from our service
        // ------------------------------------------------------------

        // Assert (Check if it worked)
        // ------------------------------------------------------------
        // Make sure we got something back (not null)
        Assert.NotNull(result);  // Check that we got a result (not null)
        
        // Make sure we got an empty list back
        Assert.Empty(result);  // Check that we got an empty list
        // ------------------------------------------------------------
    }
    // ================================================================================================================
}
