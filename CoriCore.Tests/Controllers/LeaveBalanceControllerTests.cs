using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoriCore.Controllers;
using CoriCore.Data;
using CoriCore.DTOs;
using CoriCore.Interfaces;
using CoriCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoriCore.Tests.Controllers
{
    public class LeaveBalanceControllerTests : IDisposable
    {
        private readonly Mock<ILeaveBalanceService> _mockLeaveBalanceService;
        private readonly LeaveBalanceController _controller;
        private readonly AppDbContext _context;

        public LeaveBalanceControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mockLeaveBalanceService = new Mock<ILeaveBalanceService>();
            _controller = new LeaveBalanceController(_context, _mockLeaveBalanceService.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetLeaveBalancesByEmployeeId_ReturnsOkResult_WithLeaveBalances()
        {
            // Arrange
            var employeeId = 1;
            var expectedBalances = new List<LeaveBalanceDTO>
            {
                new LeaveBalanceDTO
                {
                    LeaveBalanceId = 1,
                    RemainingDays = 10,
                    LeaveTypeName = "Annual Leave",
                    Description = "Standard annual leave",
                    DefaultDays = 20
                }
            };

            _mockLeaveBalanceService
                .Setup(s => s.GetAllLeaveBalancesByEmployeeId(employeeId))
                .ReturnsAsync(expectedBalances);

            // Act
            var result = await _controller.GetLeaveBalancesByEmployeeId(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBalances = Assert.IsType<List<LeaveBalanceDTO>>(okResult.Value);
            Assert.Single(returnedBalances);
            Assert.Equal(expectedBalances[0].LeaveBalanceId, returnedBalances[0].LeaveBalanceId);
            Assert.Equal(expectedBalances[0].RemainingDays, returnedBalances[0].RemainingDays);
        }

        [Fact]
        public async Task GetLeaveBalancesByEmployeeId_ReturnsOkResult_WhenNoBalancesFound()
        {
            // Arrange
            var employeeId = 1;
            var emptyBalances = new List<LeaveBalanceDTO>();

            _mockLeaveBalanceService
                .Setup(s => s.GetAllLeaveBalancesByEmployeeId(employeeId))
                .ReturnsAsync(emptyBalances);

            // Act
            var result = await _controller.GetLeaveBalancesByEmployeeId(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBalances = Assert.IsType<List<LeaveBalanceDTO>>(okResult.Value);
            Assert.Empty(returnedBalances);
        }
    }
}