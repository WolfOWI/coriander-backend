using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoriCore.Tests.Unit.Services
{
    public class EquipmentServiceTests
    {
        private readonly EquipmentService _equipmentService;
        private readonly AppDbContext _context;

        public EquipmentServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);
            _equipmentService = new EquipmentService(_context);
        }

        #region GetEquipmentByEmployeeId Tests

        [Fact]
        public async Task GetEquipmentByEmployeeId_ReturnsCorrectEquipment_WhenEmployeeHasEquipment()
        {
            // Arrange
            int employeeId = 1;
            var category = CreateTestEquipmentCategory(1, "Laptops");
            var equipment1 = CreateTestEquipment(1, employeeId, category.EquipmentCatId, "MacBook Pro", EquipmentCondition.Good);
            var equipment2 = CreateTestEquipment(2, employeeId, category.EquipmentCatId, "Dell Monitor", EquipmentCondition.New);
            var equipment3 = CreateTestEquipment(3, 2, category.EquipmentCatId, "Other Employee's Laptop", EquipmentCondition.Good); // Different employee

            _context.EquipmentCategories.Add(category);
            _context.Equipments.AddRange(equipment1, equipment2, equipment3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetEquipmentByEmployeeId(employeeId);

            // Assert
            Assert.Equal(2, result.Count); // Only equipment for employee 1
            Assert.All(result, e => Assert.Equal(employeeId, e.EmployeeId));
            Assert.Contains(result, e => e.EquipmentName == "MacBook Pro");
            Assert.Contains(result, e => e.EquipmentName == "Dell Monitor");
            Assert.DoesNotContain(result, e => e.EquipmentName == "Other Employee's Laptop");
        }

        [Fact]
        public async Task GetEquipmentByEmployeeId_ReturnsEmptyList_WhenEmployeeHasNoEquipment()
        {
            // Arrange
            int employeeId = 999; // Non-existent employee
            var category = CreateTestEquipmentCategory(1, "Laptops");
            var equipment = CreateTestEquipment(1, 1, category.EquipmentCatId, "Someone else's laptop", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetEquipmentByEmployeeId(employeeId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEquipmentByEmployeeId_IncludesCategoryInformation()
        {
            // Arrange
            int employeeId = 1;
            var category = CreateTestEquipmentCategory(1, "Office Supplies");
            var equipment = CreateTestEquipment(1, employeeId, category.EquipmentCatId, "Wireless Mouse", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetEquipmentByEmployeeId(employeeId);

            // Assert
            var equipmentDto = result.First();
            Assert.Equal(category.EquipmentCatName, equipmentDto.EquipmentCategoryName);
            Assert.Equal(category.EquipmentCatId, equipmentDto.EquipmentCatId);
        }

        #endregion

        #region CreateEquipmentItemsAsync Tests

        [Fact]
        public async Task CreateEquipmentItemsAsync_CreatesMultipleItems_WhenValidData()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Computers");
            _context.EquipmentCategories.Add(category);
            await _context.SaveChangesAsync();

            var equipmentDTOs = new List<CreateEquipmentDTO>
            {
                new CreateEquipmentDTO
                {
                    EmployeeId = 1,
                    EquipmentCatId = category.EquipmentCatId,
                    EquipmentName = "Laptop 1",
                    AssignedDate = DateOnly.FromDateTime(DateTime.Now),
                    Condition = EquipmentCondition.New
                },
                new CreateEquipmentDTO
                {
                    EmployeeId = 2,
                    EquipmentCatId = category.EquipmentCatId,
                    EquipmentName = "Laptop 2",
                    AssignedDate = DateOnly.FromDateTime(DateTime.Now),
                    Condition = EquipmentCondition.Good
                }
            };

            // Act
            var result = await _equipmentService.CreateEquipmentItemsAsync(equipmentDTOs);

            // Assert
            var createdItems = result.ToList();
            Assert.Equal(2, createdItems.Count);

            // Verify items were saved to database
            var savedEquipment = await _context.Equipments.ToListAsync();
            Assert.Equal(2, savedEquipment.Count);
            Assert.Contains(savedEquipment, e => e.EquipmentName == "Laptop 1");
            Assert.Contains(savedEquipment, e => e.EquipmentName == "Laptop 2");
        }

        [Fact]
        public async Task CreateEquipmentItemsAsync_CreatesUnassignedEquipment_WhenEmployeeIdIsNull()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Monitors");
            _context.EquipmentCategories.Add(category);
            await _context.SaveChangesAsync();

            var equipmentDTOs = new List<CreateEquipmentDTO>
            {
                new CreateEquipmentDTO
                {
                    EmployeeId = null, // Unassigned equipment
                    EquipmentCatId = category.EquipmentCatId,
                    EquipmentName = "Spare Monitor",
                    Condition = EquipmentCondition.Good
                }
            };

            // Act
            var result = await _equipmentService.CreateEquipmentItemsAsync(equipmentDTOs);

            // Assert
            var createdItem = result.First();
            Assert.Null(createdItem.EmployeeId);
            Assert.Equal("Spare Monitor", createdItem.EquipmentName);
        }

        [Fact]
        public async Task CreateEquipmentItemsAsync_HandlesEmptyList()
        {
            // Arrange
            var emptyList = new List<CreateEquipmentDTO>();

            // Act
            var result = await _equipmentService.CreateEquipmentItemsAsync(emptyList);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region EditEquipmentItemAsync Tests

        [Fact]
        public async Task EditEquipmentItemAsync_UpdatesAllFields_WhenValidData()
        {
            // Arrange
            var category1 = CreateTestEquipmentCategory(1, "Laptops");
            var category2 = CreateTestEquipmentCategory(2, "Monitors");
            var equipment = CreateTestEquipment(1, 1, category1.EquipmentCatId, "Old Laptop", EquipmentCondition.Used);

            _context.EquipmentCategories.AddRange(category1, category2);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateEquipmentDTO
            {
                EmployeeId = 2,
                EquipmentCatId = category2.EquipmentCatId,
                EquipmentName = "Updated Monitor",
                AssignedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                Condition = EquipmentCondition.New
            };

            // Act
            var result = await _equipmentService.EditEquipmentItemAsync(equipment.EquipmentId, updateDto);

            // Assert
            Assert.Equal(2, result.EmployeeId);
            Assert.Equal(category2.EquipmentCatId, result.EquipmentCatId);
            Assert.Equal("Updated Monitor", result.EquipmentName);
            Assert.Equal(EquipmentCondition.New, result.Condition);
            Assert.Equal(category2.EquipmentCatName, result.EquipmentCategoryName);

            // Verify changes were saved to database
            var updatedEquipment = await _context.Equipments.FindAsync(equipment.EquipmentId);
            Assert.Equal("Updated Monitor", updatedEquipment.EquipmentName);
        }

        [Fact]
        public async Task EditEquipmentItemAsync_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Keyboards");
            var equipment = CreateTestEquipment(1, 1, category.EquipmentCatId, "Mechanical Keyboard", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateEquipmentDTO
            {
                EmployeeId = null, // Don't update
                EquipmentCatId = null, // Don't update
                EquipmentName = "Updated Keyboard Name", // Update this
                AssignedDate = null, // Don't update
                Condition = null // Don't update
            };

            // Act
            var result = await _equipmentService.EditEquipmentItemAsync(equipment.EquipmentId, updateDto);

            // Assert
            Assert.Equal(1, result.EmployeeId); // Should remain unchanged
            Assert.Equal(category.EquipmentCatId, result.EquipmentCatId); // Should remain unchanged
            Assert.Equal("Updated Keyboard Name", result.EquipmentName); // Should be updated
            Assert.Equal(EquipmentCondition.Good, result.Condition); // Should remain unchanged
        }

        [Fact]
        public async Task EditEquipmentItemAsync_ThrowsException_WhenEquipmentNotFound()
        {
            // Arrange
            var updateDto = new UpdateEquipmentDTO
            {
                EquipmentName = "This should fail"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _equipmentService.EditEquipmentItemAsync(999, updateDto));
        }

        #endregion

        #region DeleteEquipmentItemAsync Tests

        [Fact]
        public async Task DeleteEquipmentItemAsync_ReturnsTrue_WhenEquipmentExists()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Printers");
            var equipment = CreateTestEquipment(1, null, category.EquipmentCatId, "Laser Printer", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.DeleteEquipmentItemAsync(equipment.EquipmentId);

            // Assert
            Assert.True(result);

            // Verify equipment was deleted from database
            var deletedEquipment = await _context.Equipments.FindAsync(equipment.EquipmentId);
            Assert.Null(deletedEquipment);
        }

        [Fact]
        public async Task DeleteEquipmentItemAsync_ReturnsFalse_WhenEquipmentNotFound()
        {
            // Arrange & Act
            var result = await _equipmentService.DeleteEquipmentItemAsync(999);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region AssignEquipmentAsync Tests

        [Fact]
        public async Task AssignEquipmentAsync_AssignsSuccessfully_WhenEquipmentIsUnassigned()
        {
            // Arrange
            int employeeId = 1;
            var category = CreateTestEquipmentCategory(1, "Tablets");
            var equipment1 = CreateTestEquipment(1, null, category.EquipmentCatId, "iPad Pro", EquipmentCondition.New);
            var equipment2 = CreateTestEquipment(2, null, category.EquipmentCatId, "Surface Pro", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.AddRange(equipment1, equipment2);
            await _context.SaveChangesAsync();

            var equipmentIds = new List<int> { equipment1.EquipmentId, equipment2.EquipmentId };

            // Act
            var result = await _equipmentService.AssignEquipmentAsync(employeeId, equipmentIds);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Equipment assigned successfully", result.Message);

            // Verify equipment was assigned
            var assignedEquipment = await _context.Equipments.Where(e => equipmentIds.Contains(e.EquipmentId)).ToListAsync();
            Assert.All(assignedEquipment, e => Assert.Equal(employeeId, e.EmployeeId));
            Assert.All(assignedEquipment, e => Assert.Equal(DateOnly.FromDateTime(DateTime.Now), e.AssignedDate));
        }

        [Fact]
        public async Task AssignEquipmentAsync_ReturnsError_WhenEquipmentNotFound()
        {
            // Arrange
            int employeeId = 1;
            var equipmentIds = new List<int> { 999 }; // Non-existent equipment

            // Act
            var result = await _equipmentService.AssignEquipmentAsync(employeeId, equipmentIds);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Contains("Equipment with ID 999 not found", result.Message);
        }

        [Fact]
        public async Task AssignEquipmentAsync_ReturnsError_WhenEquipmentAlreadyAssigned()
        {
            // Arrange
            int employeeId = 1;
            int otherEmployeeId = 2;
            var category = CreateTestEquipmentCategory(1, "Phones");
            var equipment = CreateTestEquipment(1, otherEmployeeId, category.EquipmentCatId, "iPhone", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            var equipmentIds = new List<int> { equipment.EquipmentId };

            // Act
            var result = await _equipmentService.AssignEquipmentAsync(employeeId, equipmentIds);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Contains("is already assigned to another employee", result.Message);
        }

        [Fact]
        public async Task AssignEquipmentAsync_AllowsReassignmentToSameEmployee()
        {
            // Arrange
            int employeeId = 1;
            var category = CreateTestEquipmentCategory(1, "Headphones");
            var equipment = CreateTestEquipment(1, employeeId, category.EquipmentCatId, "Noise-Cancelling Headphones", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            var equipmentIds = new List<int> { equipment.EquipmentId };

            // Act
            var result = await _equipmentService.AssignEquipmentAsync(employeeId, equipmentIds);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Equipment assigned successfully", result.Message);
        }

        #endregion

        #region ForceAssignEquipmentAsync Tests

        [Fact]
        public async Task ForceAssignEquipmentAsync_AssignsSuccessfully_EvenWhenAlreadyAssigned()
        {
            // Arrange
            int currentEmployeeId = 1;
            int newEmployeeId = 2;
            var category = CreateTestEquipmentCategory(1, "Cameras");
            var equipment = CreateTestEquipment(1, currentEmployeeId, category.EquipmentCatId, "Digital Camera", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            var equipmentIds = new List<int> { equipment.EquipmentId };

            // Act
            var result = await _equipmentService.ForceAssignEquipmentAsync(newEmployeeId, equipmentIds);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Equipment assigned successfully", result.Message);

            // Verify equipment was reassigned
            var reassignedEquipment = await _context.Equipments.FindAsync(equipment.EquipmentId);
            Assert.Equal(newEmployeeId, reassignedEquipment.EmployeeId);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), reassignedEquipment.AssignedDate);
        }

        [Fact]
        public async Task ForceAssignEquipmentAsync_ReturnsError_WhenEquipmentNotFound()
        {
            // Arrange
            int employeeId = 1;
            var equipmentIds = new List<int> { 999 };

            // Act
            var result = await _equipmentService.ForceAssignEquipmentAsync(employeeId, equipmentIds);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Contains("Equipment with ID 999 not found", result.Message);
        }

        #endregion

        #region GetAllAssignedEquipItems Tests

        [Fact]
        public async Task GetAllAssignedEquipItems_ReturnsOnlyAssignedEquipment()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Computers");
            var user1 = CreateTestUser(1, "Employee One");
            var user2 = CreateTestUser(2, "Employee Two");
            var employee1 = CreateTestEmployee(1, user1);
            var employee2 = CreateTestEmployee(2, user2);

            var assignedEquipment1 = CreateTestEquipment(1, employee1.EmployeeId, category.EquipmentCatId, "Laptop 1", EquipmentCondition.Good);
            var assignedEquipment2 = CreateTestEquipment(2, employee2.EmployeeId, category.EquipmentCatId, "Laptop 2", EquipmentCondition.New);
            var unassignedEquipment = CreateTestEquipment(3, null, category.EquipmentCatId, "Spare Laptop", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Users.AddRange(user1, user2);
            _context.Employees.AddRange(employee1, employee2);
            _context.Equipments.AddRange(assignedEquipment1, assignedEquipment2, unassignedEquipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetAllAssignedEquipItems();

            // Assert
            Assert.Equal(2, result.Count); // Only assigned equipment
            Assert.All(result, item => Assert.NotNull(item.Equipment.EmployeeId));
            Assert.Contains(result, item => item.FullName == "Employee One");
            Assert.Contains(result, item => item.FullName == "Employee Two");
        }

        [Fact]
        public async Task GetAllAssignedEquipItems_ReturnsEmptyList_WhenNoAssignedEquipment()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Monitors");
            var unassignedEquipment = CreateTestEquipment(1, null, category.EquipmentCatId, "Spare Monitor", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(unassignedEquipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetAllAssignedEquipItems();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetAllUnassignedEquipItems Tests

        [Fact]
        public async Task GetAllUnassignedEquipItems_ReturnsOnlyUnassignedEquipment()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Accessories");
            var assignedEquipment = CreateTestEquipment(1, 1, category.EquipmentCatId, "Assigned Mouse", EquipmentCondition.Good);
            var unassignedEquipment1 = CreateTestEquipment(2, null, category.EquipmentCatId, "Spare Mouse", EquipmentCondition.New);
            var unassignedEquipment2 = CreateTestEquipment(3, null, category.EquipmentCatId, "Spare Keyboard", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.AddRange(assignedEquipment, unassignedEquipment1, unassignedEquipment2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetAllUnassignedEquipItems();

            // Assert
            Assert.Equal(2, result.Count); // Only unassigned equipment
            Assert.All(result, item => Assert.True(item.EmployeeId == null || item.EmployeeId == 0));
            Assert.Contains(result, item => item.EquipmentName == "Spare Mouse");
            Assert.Contains(result, item => item.EquipmentName == "Spare Keyboard");
        }

        [Fact]
        public async Task GetAllUnassignedEquipItems_ReturnsEmptyList_WhenAllEquipmentAssigned()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Projectors");
            var assignedEquipment = CreateTestEquipment(1, 1, category.EquipmentCatId, "Conference Room Projector", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(assignedEquipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.GetAllUnassignedEquipItems();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region UnlinkEquipmentFromEmployee Tests

        [Fact]
        public async Task UnlinkEquipmentFromEmployee_UnlinksSuccessfully_WhenEquipmentIsAssigned()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Tools");
            var equipment = CreateTestEquipment(1, 1, category.EquipmentCatId, "Screwdriver Set", EquipmentCondition.Good);
            equipment.AssignedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.UnlinkEquipmentFromEmployee(equipment.EquipmentId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Equal("Equipment unlinked from employee successfully", result.Message);

            // Verify equipment was unlinked
            var unlinkedEquipment = await _context.Equipments.FindAsync(equipment.EquipmentId);
            Assert.Null(unlinkedEquipment.EmployeeId);
            Assert.Null(unlinkedEquipment.AssignedDate);
        }

        [Fact]
        public async Task UnlinkEquipmentFromEmployee_ReturnsError_WhenEquipmentNotFound()
        {
            // Arrange & Act
            var result = await _equipmentService.UnlinkEquipmentFromEmployee(999);

            // Assert
            Assert.Equal(404, result.Code);
            Assert.Equal("Equipment with ID 999 not found", result.Message);
        }

        [Fact]
        public async Task UnlinkEquipmentFromEmployee_ReturnsError_WhenEquipmentNotAssigned()
        {
            // Arrange
            var category = CreateTestEquipmentCategory(1, "Software");
            var equipment = CreateTestEquipment(1, null, category.EquipmentCatId, "Unassigned License", EquipmentCondition.Good);

            _context.EquipmentCategories.Add(category);
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.UnlinkEquipmentFromEmployee(equipment.EquipmentId);

            // Assert
            Assert.Equal(400, result.Code);
            Assert.Equal("Equipment with ID 1 is already unlinked", result.Message);
        }

        #endregion

        #region MassUnlinkEquipmentFromEmployee Tests

        [Fact]
        public async Task MassUnlinkEquipmentFromEmployee_UnlinksAllEquipment_WhenEmployeeHasEquipment()
        {
            // Arrange
            int employeeId = 1;
            var user = CreateTestUser(employeeId, "Test Employee");
            var employee = CreateTestEmployee(employeeId, user);
            var category = CreateTestEquipmentCategory(1, "Office Setup");
            var equipment1 = CreateTestEquipment(1, employeeId, category.EquipmentCatId, "Desk Lamp", EquipmentCondition.Good);
            var equipment2 = CreateTestEquipment(2, employeeId, category.EquipmentCatId, "Ergonomic Chair", EquipmentCondition.New);
            var equipment3 = CreateTestEquipment(3, 2, category.EquipmentCatId, "Other Employee's Equipment", EquipmentCondition.Good); // Different employee

            _context.Users.Add(user);
            _context.Employees.Add(employee);
            _context.EquipmentCategories.Add(category);
            _context.Equipments.AddRange(equipment1, equipment2, equipment3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.MassUnlinkEquipmentFromEmployee(employeeId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Contains("equipment item(s) unlinked from employee successfully", result.Message);

            // Verify only the target employee's equipment was unlinked
            var employee1Equipment = await _context.Equipments.Where(e => e.EquipmentId == 1 || e.EquipmentId == 2).ToListAsync();
            Assert.All(employee1Equipment, e => Assert.Null(e.EmployeeId));
            Assert.All(employee1Equipment, e => Assert.Null(e.AssignedDate));

            // Verify other employee's equipment was not affected
            var otherEmployeeEquipment = await _context.Equipments.FindAsync(3);
            Assert.Equal(2, otherEmployeeEquipment.EmployeeId); // Should remain assigned
        }

        [Fact]
        public async Task MassUnlinkEquipmentFromEmployee_ReturnsSuccess_WhenEmployeeHasNoEquipment()
        {
            // Arrange
            int employeeId = 1;
            var user = CreateTestUser(employeeId, "Test Employee");
            var employee = CreateTestEmployee(employeeId, user);

            _context.Users.Add(user);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _equipmentService.MassUnlinkEquipmentFromEmployee(employeeId);

            // Assert
            Assert.Equal(200, result.Code);
            Assert.Contains("equipment item(s) unlinked from employee successfully", result.Message);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a test equipment category
        /// </summary>
        private static EquipmentCategory CreateTestEquipmentCategory(int categoryId, string categoryName)
        {
            return new EquipmentCategory
            {
                EquipmentCatId = categoryId,
                EquipmentCatName = categoryName
            };
        }

        /// <summary>
        /// Creates a test equipment item
        /// </summary>
        private static Equipment CreateTestEquipment(int equipmentId, int? employeeId, int categoryId, string name, EquipmentCondition condition)
        {
            return new Equipment
            {
                EquipmentId = equipmentId,
                EmployeeId = employeeId,
                EquipmentCatId = categoryId,
                EquipmentName = name,
                Condition = condition,
                AssignedDate = employeeId.HasValue ? DateOnly.FromDateTime(DateTime.Now) : null
            };
        }

        /// <summary>
        /// Creates a test user
        /// </summary>
        private static User CreateTestUser(int userId, string fullName)
        {
            return new User
            {
                UserId = userId,
                FullName = fullName,
                Email = $"user{userId}@example.com",
                Role = UserRole.Employee
            };
        }

        /// <summary>
        /// Creates a test employee
        /// </summary>
        private static Employee CreateTestEmployee(int employeeId, User user)
        {
            return new Employee
            {
                EmployeeId = employeeId,
                UserId = user.UserId,
                User = user,
                Gender = Gender.Male,
                DateOfBirth = new DateOnly(1990, 1, 1),
                PhoneNumber = $"123-456-789{employeeId}",
                JobTitle = "Test Employee",
                Department = "Test Department",
                SalaryAmount = 50000,
                PayCycle = PayCycle.Monthly,
                EmployDate = DateOnly.FromDateTime(DateTime.Now),
                EmployType = EmployType.FullTime
            };
        }

        #endregion
    }
}