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

public class EquipmentControllerTests
{
    private readonly EquipmentController _controller;
    private readonly Mock<IEquipmentService> _mockEquipmentService;
    private readonly AppDbContext _context;

    public EquipmentControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _mockEquipmentService = new Mock<IEquipmentService>();
        _controller = new EquipmentController(_context, _mockEquipmentService.Object);
    }

    [Fact]
    public async Task GetAllEquipItems_ReturnsOk_WithCombinedItems()
    {
        // Arrange
        var assignedItems = new List<EmpEquipItemDTO>
        {
            new EmpEquipItemDTO
            {
                Equipment = new EquipmentDTO { EquipmentId = 1, EquipmentName = "Laptop" },
                FullName = "John Doe"
            }
        };

        var unassignedItems = new List<EquipmentDTO>
        {
            new EquipmentDTO { EquipmentId = 2, EquipmentName = "Monitor" }
        };

        _mockEquipmentService.Setup(x => x.GetAllAssignedEquipItems())
            .ReturnsAsync(assignedItems);
        _mockEquipmentService.Setup(x => x.GetAllUnassignedEquipItems())
            .ReturnsAsync(unassignedItems);

        // Act
        var result = await _controller.GetAllEquipItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Assert.IsAssignableFrom<List<EmpEquipItemDTO>>(okResult.Value);
        Assert.Equal(2, returnedItems.Count);
    }

    [Fact]
    public async Task GetAllUnassignedEquipItems_ReturnsOk_WithUnassignedItems()
    {
        // Arrange
        var unassignedItems = new List<EquipmentDTO>
        {
            new EquipmentDTO { EquipmentId = 1, EquipmentName = "Monitor" },
            new EquipmentDTO { EquipmentId = 2, EquipmentName = "Keyboard" }
        };

        _mockEquipmentService.Setup(x => x.GetAllUnassignedEquipItems())
            .ReturnsAsync(unassignedItems);

        // Act
        var result = await _controller.GetAllUnassignedEquipItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Assert.IsAssignableFrom<List<EquipmentDTO>>(okResult.Value);
        Assert.Equal(2, returnedItems.Count);
        _mockEquipmentService.Verify(x => x.GetAllUnassignedEquipItems(), Times.Once);
    }

    [Fact]
    public async Task GetEquipmentByEmployeeId_ReturnsOk_WithEmployeeEquipment()
    {
        // Arrange
        var employeeId = 1;
        var equipment = new List<EquipmentDTO>
        {
            new EquipmentDTO { EquipmentId = 1, EquipmentName = "Laptop", EmployeeId = employeeId }
        };

        _mockEquipmentService.Setup(x => x.GetEquipmentByEmployeeId(employeeId))
            .ReturnsAsync(equipment);

        // Act
        var result = await _controller.GetEquipmentByEmployeeId(employeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEquipment = Assert.IsAssignableFrom<List<EquipmentDTO>>(okResult.Value);
        Assert.Single(returnedEquipment);
        _mockEquipmentService.Verify(x => x.GetEquipmentByEmployeeId(employeeId), Times.Once);
    }

    [Fact]
    public async Task CreateEquipmentItems_ReturnsBadRequest_WhenNoEquipmentProvided()
    {
        // Arrange & Act
        var result = await _controller.CreateEquipmentItems(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No equipment provided.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateEquipmentItems_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var equipmentDtos = new List<CreateEquipmentDTO>
        {
            new CreateEquipmentDTO { EquipmentName = "New Laptop", EquipmentCatId = 1 }
        };

        var createdEquipment = new List<Equipment>
        {
            new Equipment
            {
                EquipmentId = 1,
                EquipmentName = "New Laptop",
                EquipmentCatId = 1,
                EquipmentCategory = new EquipmentCategory { EquipmentCatId = 1, EquipmentCatName = "Computers" }
            }
        };

        _mockEquipmentService.Setup(x => x.CreateEquipmentItemsAsync(equipmentDtos))
            .ReturnsAsync(createdEquipment);

        _context.Equipments.AddRange(createdEquipment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.CreateEquipmentItems(equipmentDtos);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        _mockEquipmentService.Verify(x => x.CreateEquipmentItemsAsync(equipmentDtos), Times.Once);
    }

    [Fact]
    public async Task EditEquipmentItem_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var equipmentId = 1;
        var updateDto = new UpdateEquipmentDTO
        {
            EquipmentName = "Updated Laptop"
        };
        var updatedEquipment = new EquipmentDTO { EquipmentId = equipmentId, EquipmentName = "Updated Laptop" };

        _mockEquipmentService.Setup(x => x.EditEquipmentItemAsync(equipmentId, updateDto))
            .ReturnsAsync(updatedEquipment);

        // Act
        var result = await _controller.EditEquipmentItem(equipmentId, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedEquipment, okResult.Value);
        _mockEquipmentService.Verify(x => x.EditEquipmentItemAsync(equipmentId, updateDto), Times.Once);
    }

    [Fact]
    public async Task EditEquipmentItem_ReturnsNotFound_WhenEquipmentNotFound()
    {
        // Arrange
        var equipmentId = 999;
        var updateDto = new UpdateEquipmentDTO();

        _mockEquipmentService.Setup(x => x.EditEquipmentItemAsync(equipmentId, updateDto))
            .ReturnsAsync((EquipmentDTO?)null);

        // Act
        var result = await _controller.EditEquipmentItem(equipmentId, updateDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Equipment not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task AssignEquipmentToEmployee_ReturnsStatusCode_FromService()
    {
        // Arrange
        var employeeId = 1;
        var equipmentIds = new List<int> { 1, 2, 3 };
        _mockEquipmentService.Setup(x => x.ForceAssignEquipmentAsync(employeeId, equipmentIds))
            .ReturnsAsync((200, "Equipment assigned successfully"));

        // Act
        var result = await _controller.AssignEquipmentToEmployee(employeeId, equipmentIds);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockEquipmentService.Verify(x => x.ForceAssignEquipmentAsync(employeeId, equipmentIds), Times.Once);
    }

    [Fact]
    public async Task UnlinkEquipmentFromEmployee_ReturnsStatusCode_FromService()
    {
        // Arrange
        var equipmentId = 1;
        _mockEquipmentService.Setup(x => x.UnlinkEquipmentFromEmployee(equipmentId))
            .ReturnsAsync((200, "Equipment unlinked successfully"));

        // Act
        var result = await _controller.UnlinkEquipmentFromEmployee(equipmentId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockEquipmentService.Verify(x => x.UnlinkEquipmentFromEmployee(equipmentId), Times.Once);
    }

    [Fact]
    public async Task MassUnlinkEquipment_ReturnsStatusCode_FromService()
    {
        // Arrange
        var employeeId = 1;
        _mockEquipmentService.Setup(x => x.MassUnlinkEquipmentFromEmployee(employeeId))
            .ReturnsAsync((200, "All equipment unlinked successfully"));

        // Act
        var result = await _controller.MassUnlinkEquipment(employeeId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, statusCodeResult.StatusCode);
        _mockEquipmentService.Verify(x => x.MassUnlinkEquipmentFromEmployee(employeeId), Times.Once);
    }

    [Fact]
    public async Task DeleteEquipmentItem_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var equipmentId = 1;

        _mockEquipmentService.Setup(x => x.DeleteEquipmentItemAsync(equipmentId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteEquipmentItem(equipmentId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockEquipmentService.Verify(x => x.DeleteEquipmentItemAsync(equipmentId), Times.Once);
    }

    [Fact]
    public async Task DeleteEquipmentItem_ReturnsNotFound_WhenEquipmentNotFound()
    {
        // Arrange
        var equipmentId = 999;

        _mockEquipmentService.Setup(x => x.DeleteEquipmentItemAsync(equipmentId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteEquipmentItem(equipmentId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Equipment not found.", notFoundResult.Value);
    }
}