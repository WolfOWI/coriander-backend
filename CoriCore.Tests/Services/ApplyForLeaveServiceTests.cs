using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CoriCore.Services;
using CoriCore.DTOs;
using CoriCore.Models;
using CoriCore.Data;

namespace CoriCore.Tests.Unit.Services
{
    public class ApplyForLeaveServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task ApplyForLeave_CreatesLeaveRequest_ReturnsDTO()
        {
            // Arrange
            var context = GetDbContext();
            var leaveType = new LeaveType
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Annual",
                Description = "Annual leave",
                DefaultDays = 15
            };
            context.LeaveTypes.Add(leaveType);
            await context.SaveChangesAsync();

            var service = new ApplyForLeaveService(context);
            var fixedStartDate = new DateTime(2024, 1, 1);
            var fixedEndDate = fixedStartDate.AddDays(2);
            var dto = new ApplyForLeaveDTO
            {
                EmployeeId = 1,
                LeaveTypeId = 1,
                Comment = "Vacation"
            };

            // Act
            var result = await service.ApplyForLeave(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.EmployeeId, result.EmployeeId);
            Assert.Equal(dto.LeaveTypeId, result.LeaveTypeId);
            Assert.Equal(dto.Comment, result.Comment);
            Assert.Equal(LeaveStatus.Pending, result.Status);
            Assert.Equal("Annual", result.LeaveTypeName);
            Assert.Equal("Annual leave", result.Description);
            Assert.Equal(15, result.DefaultDays);
        }

        [Fact]
        public async Task ApplyForLeave_ThrowsException_WhenLeaveTypeMissing()
        {
            // Arrange
            var context = GetDbContext();
            var service = new ApplyForLeaveService(context);
            var fixedStartDate = new DateTime(2024, 1, 1);
            var fixedEndDate = fixedStartDate.AddDays(2);
            var dto = new ApplyForLeaveDTO
            {
                EmployeeId = 1,
                LeaveTypeId = 99, 
                Comment = "Vacation"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.ApplyForLeave(dto);
            });
        }
    }
}