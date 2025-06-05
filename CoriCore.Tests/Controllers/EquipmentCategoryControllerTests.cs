using System;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Tests.Unit.Controllers;

public class EquipmentCategoryControllerTests
{
    private readonly EquipmentCategoryController _controller;
    private readonly AppDbContext _context;

    public EquipmentCategoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new AppDbContext(options);
        _controller = new EquipmentCategoryController(_context);
    }

    [Fact]
    public async Task GetEquipmentCategories_ReturnsOkWithCategories()
    {
        // Arrange
        var categories = new List<EquipmentCategory>
        {
            new EquipmentCategory { EquipmentCatId = 1, EquipmentCatName = "Computers" },
            new EquipmentCategory { EquipmentCatId = 2, EquipmentCatName = "Office Equipment" }
        };

        _context.EquipmentCategories.AddRange(categories);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEquipmentCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCategories = Assert.IsAssignableFrom<IEnumerable<EquipmentCategory>>(okResult.Value);
        Assert.Equal(2, returnedCategories.Count());
    }

    [Fact]
    public async Task GetEquipmentCategory_ReturnsOk_WhenCategoryExists()
    {
        // Arrange
        var category = new EquipmentCategory { EquipmentCatId = 1, EquipmentCatName = "Computers" };
        _context.EquipmentCategories.Add(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEquipmentCategory(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCategory = Assert.IsType<EquipmentCategory>(okResult.Value);
        Assert.Equal(category.EquipmentCatId, returnedCategory.EquipmentCatId);
        Assert.Equal(category.EquipmentCatName, returnedCategory.EquipmentCatName);
    }

    [Fact]
    public async Task GetEquipmentCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange & Act
        var result = await _controller.GetEquipmentCategory(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PutEquipmentCategory_ReturnsNoContent_WhenUpdateSuccessful()
    {
        // Arrange
        var category = new EquipmentCategory { EquipmentCatId = 1, EquipmentCatName = "Computers" };
        _context.EquipmentCategories.Add(category);
        await _context.SaveChangesAsync();

        category.EquipmentCatName = "Computer Hardware";

        // Act
        var result = await _controller.PutEquipmentCategory(1, category);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var updatedCategory = await _context.EquipmentCategories.FindAsync(1);
        Assert.Equal("Computer Hardware", updatedCategory.EquipmentCatName);
    }

    [Fact]
    public async Task PutEquipmentCategory_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var category = new EquipmentCategory { EquipmentCatId = 1, EquipmentCatName = "Computers" };

        // Act
        var result = await _controller.PutEquipmentCategory(2, category);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PostEquipmentCategory_ReturnsCreatedAtAction_WithCategory()
    {
        // Arrange
        var category = new EquipmentCategory { EquipmentCatName = "Office Supplies" };

        // Act
        var result = await _controller.PostEquipmentCategory(category);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedCategory = Assert.IsType<EquipmentCategory>(createdAtActionResult.Value);
        Assert.Equal(category.EquipmentCatName, returnedCategory.EquipmentCatName);
        Assert.True(returnedCategory.EquipmentCatId > 0);
    }

    [Fact]
    public async Task DeleteEquipmentCategory_ReturnsNoContent_WhenCategoryExists()
    {
        // Arrange
        var category = new EquipmentCategory { EquipmentCatId = 1, EquipmentCatName = "Computers" };
        _context.EquipmentCategories.Add(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteEquipmentCategory(1);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var deletedCategory = await _context.EquipmentCategories.FindAsync(1);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteEquipmentCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange & Act
        var result = await _controller.DeleteEquipmentCategory(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}