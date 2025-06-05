using System;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Tests.Unit.Services;

public class EquipmentServiceTests
{
    private readonly AppDbContext _context;
    private readonly EquipmentService _service;

    public EquipmentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _service = new EquipmentService(_context);
    }

    [Fact]
    public async Task GetEquipmentByEmployeeId_ReturnsEmptyList_WhenNoEquipmentAssigned()
    {
        // Arrange
        var employeeId = 1;

        // Act
        var result = await _service.GetEquipmentByEmployeeId(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEquipmentByEmployeeId_ReturnsEquipment_WhenEquipmentAssigned()
    {
        // Arrange
        var category = new EquipmentCategory
        {
            EquipmentCatId = 1,
            EquipmentCatName = "Laptop"
        };

        var equipment = new Equipment
        {
            EquipmentId = 1,
            EmployeeId = 1,
            EquipmentCatId = 1,
            EquipmentCategory = category,
            EquipmentName = "MacBook Pro",
            AssignedDate = DateOnly.FromDateTime(DateTime.Now),
            Condition = EquipmentCondition.Good
        };

        _context.EquipmentCategories.Add(category);
        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetEquipmentByEmployeeId(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var equipmentDto = result.First();
        Assert.Equal(equipment.EquipmentId, equipmentDto.EquipmentId);
        Assert.Equal(equipment.EmployeeId, equipmentDto.EmployeeId);
        Assert.Equal(equipment.EquipmentName, equipmentDto.EquipmentName);
        Assert.Equal(category.EquipmentCatName, equipmentDto.EquipmentCategoryName);
    }

    [Fact]
    public async Task CreateEquipmentItemsAsync_CreatesMultipleItems()
    {
        // Arrange
        var equipmentDTOs = new List<CreateEquipmentDTO>
        {
            new CreateEquipmentDTO
            {
                EmployeeId = 1,
                EquipmentCatId = 1,
                EquipmentName = "Laptop",
                AssignedDate = DateOnly.FromDateTime(DateTime.Now),
                Condition = EquipmentCondition.New
            },
            new CreateEquipmentDTO
            {
                EmployeeId = 1,
                EquipmentCatId = 2,
                EquipmentName = "Mouse",
                AssignedDate = DateOnly.FromDateTime(DateTime.Now),
                Condition = EquipmentCondition.Good
            }
        };

        // Act
        var result = await _service.CreateEquipmentItemsAsync(equipmentDTOs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        var equipmentInDb = await _context.Equipments.ToListAsync();
        Assert.Equal(2, equipmentInDb.Count);
    }

    [Fact]
    public async Task EditEquipmentItemAsync_ThrowsException_WhenEquipmentNotFound()
    {
        // Arrange
        var dto = new UpdateEquipmentDTO
        {
            EquipmentName = "Updated Laptop"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.EditEquipmentItemAsync(999, dto));
    }

    [Fact]
    public async Task EditEquipmentItemAsync_UpdatesEquipment_WhenEquipmentExists()
    {
        // Arrange
        var category = new EquipmentCategory
        {
            EquipmentCatId = 1,
            EquipmentCatName = "Laptop"
        };

        var equipment = new Equipment
        {
            EquipmentId = 1,
            EmployeeId = 1,
            EquipmentCatId = 1,
            EquipmentCategory = category,
            EquipmentName = "MacBook Pro",
            AssignedDate = DateOnly.FromDateTime(DateTime.Now),
            Condition = EquipmentCondition.Good
        };

        _context.EquipmentCategories.Add(category);
        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();

        var dto = new UpdateEquipmentDTO
        {
            EquipmentName = "Updated MacBook Pro",
            Condition = EquipmentCondition.Good
        };

        // Act
        var result = await _service.EditEquipmentItemAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated MacBook Pro", result.EquipmentName);
        Assert.Equal(EquipmentCondition.Good, result.Condition);

        var updatedEquipment = await _context.Equipments.FindAsync(1);
        Assert.Equal("Updated MacBook Pro", updatedEquipment.EquipmentName);
        Assert.Equal(EquipmentCondition.Good, updatedEquipment.Condition);
    }

    [Fact]
    public async Task DeleteEquipmentItemAsync_ReturnsFalse_WhenEquipmentNotFound()
    {
        // Arrange & Act
        var result = await _service.DeleteEquipmentItemAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteEquipmentItemAsync_ReturnsTrue_WhenEquipmentDeleted()
    {
        // Arrange
        var equipment = new Equipment
        {
            EquipmentId = 1,
            EquipmentCatId = 1,
            EquipmentName = "Test Equipment",
            Condition = EquipmentCondition.Good
        };

        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteEquipmentItemAsync(1);

        // Assert
        Assert.True(result);

        var deletedEquipment = await _context.Equipments.FindAsync(1);
        Assert.Null(deletedEquipment);
    }

    [Fact]
    public async Task AssignEquipmentAsync_ReturnsCode404_WhenEquipmentNotFound()
    {
        // Arrange
        var employeeId = 1;
        var equipmentIds = new List<int> { 999 };

        // Act
        var result = await _service.AssignEquipmentAsync(employeeId, equipmentIds);

        // Assert
        Assert.Equal(404, result.Code);
        Assert.Contains("Equipment with ID 999 not found", result.Message);
    }

    [Fact]
    public async Task AssignEquipmentAsync_ReturnsCode400_WhenEquipmentAlreadyAssigned()
    {
        // Arrange
        var equipment = new Equipment
        {
            EquipmentId = 1,
            EmployeeId = 2, // Already assigned to another employee
            EquipmentCatId = 1,
            EquipmentName = "Test Equipment",
            Condition = EquipmentCondition.Good
        };

        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();

        var employeeId = 1;
        var equipmentIds = new List<int> { 1 };

        // Act
        var result = await _service.AssignEquipmentAsync(employeeId, equipmentIds);

        // Assert
        Assert.Equal(400, result.Code);
        Assert.Contains("already assigned to another employee", result.Message);
    }

    [Fact]
    public async Task AssignEquipmentAsync_ReturnsCode200_WhenAssignmentSuccessful()
    {
        // Arrange
        var equipment = new Equipment
        {
            EquipmentId = 1,
            EquipmentCatId = 1,
            EquipmentName = "Test Equipment",
            Condition = EquipmentCondition.Good
        };

        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();

        var employeeId = 1;
        var equipmentIds = new List<int> { 1 };

        // Act
        var result = await _service.AssignEquipmentAsync(employeeId, equipmentIds);

        // Assert
        Assert.Equal(200, result.Code);
        Assert.Equal("Equipment assigned successfully", result.Message);

        var assignedEquipment = await _context.Equipments.FindAsync(1);
        Assert.Equal(employeeId, assignedEquipment.EmployeeId);
        Assert.NotNull(assignedEquipment.AssignedDate);
    }

    [Fact]
    public async Task GetAllUnassignedEquipItems_ReturnsOnlyUnassignedEquipment()
    {
        // Arrange
        var category = new EquipmentCategory
        {
            EquipmentCatId = 1,
            EquipmentCatName = "Laptop"
        };

        var assignedEquipment = new Equipment
        {
            EquipmentId = 1,
            EmployeeId = 1,
            EquipmentCatId = 1,
            EquipmentCategory = category,
            EquipmentName = "Assigned Laptop",
            Condition = EquipmentCondition.Good
        };

        var unassignedEquipment = new Equipment
        {
            EquipmentId = 2,
            EquipmentCatId = 1,
            EquipmentCategory = category,
            EquipmentName = "Unassigned Laptop",
            Condition = EquipmentCondition.New
        };

        _context.EquipmentCategories.Add(category);
        _context.Equipments.AddRange(assignedEquipment, unassignedEquipment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUnassignedEquipItems();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(unassignedEquipment.EquipmentId, result.First().EquipmentId);
        Assert.Equal("Unassigned Laptop", result.First().EquipmentName);
    }
}