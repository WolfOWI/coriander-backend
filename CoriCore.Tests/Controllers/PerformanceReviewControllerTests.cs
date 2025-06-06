using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CoriCore.Controllers;
using CoriCore.Interfaces;
using CoriCore.Models;
using CoriCore.DTOs;

namespace CoriCore.Tests.Controllers
{
    public class PerformanceReviewControllerTests
    {
        private readonly Mock<IPerformanceReviewService> _mockService;
        private readonly PerformanceReviewController _controller;

        public PerformanceReviewControllerTests()
        {
            _mockService = new Mock<IPerformanceReviewService>();
            _controller = new PerformanceReviewController(null, _mockService.Object);
        }

        [Fact]
        public async Task GetTopEmpUserRatingMetrics_ReturnsOk()
        {
            _mockService.Setup(s => s.GetTopEmpUserRatingMetrics(5))
                .ReturnsAsync(new List<EmpUserRatingMetricsDTO> { new EmpUserRatingMetricsDTO() });

            var result = await _controller.GetTopEmpUserRatingMetrics(5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var metrics = Assert.IsAssignableFrom<List<EmpUserRatingMetricsDTO>>(okResult.Value);
            Assert.Single(metrics);
        }

        [Fact]
        public async Task GetTopEmpUserRatingMetrics_ReturnsBadRequest_OnException()
        {
            _mockService.Setup(s => s.GetTopEmpUserRatingMetrics(It.IsAny<int>()))
                .ThrowsAsync(new ArgumentException("Invalid"));

            var result = await _controller.GetTopEmpUserRatingMetrics(5);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAllEmpUserRatingMetrics_ReturnsOk()
        {
            _mockService.Setup(s => s.GetAllEmpUserRatingMetrics())
                .ReturnsAsync(new List<EmpUserRatingMetricsDTO>());

            var result = await _controller.GetAllEmpUserRatingMetrics();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<EmpUserRatingMetricsDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetEmpUserRatingMetricsByEmpId_ReturnsOk_WhenFound()
        {
            _mockService.Setup(s => s.GetEmpUserRatingMetricsByEmpId(1))
                .ReturnsAsync(new EmpUserRatingMetricsDTO());

            var result = await _controller.GetEmpUserRatingMetricsByEmpId(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<EmpUserRatingMetricsDTO>(okResult.Value);
        }

        [Fact]
        public async Task GetEmpUserRatingMetricsByEmpId_ReturnsNotFound_WhenNull()
        {
            _mockService.Setup(s => s.GetEmpUserRatingMetricsByEmpId(1))
                .ReturnsAsync((EmpUserRatingMetricsDTO)null);

            var result = await _controller.GetEmpUserRatingMetricsByEmpId(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetTopRatedEmployees_ReturnsOk_WhenFound()
        {
            _mockService.Setup(s => s.GetTopRatedEmployees())
                .ReturnsAsync(new List<TopRatedEmployeesDTO> { new TopRatedEmployeesDTO() });

            var result = await _controller.GetTopRatedEmployees();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var employees = Assert.IsAssignableFrom<List<TopRatedEmployeesDTO>>(okResult.Value);
            Assert.Single(employees);
        }

        [Fact]
        public async Task GetTopRatedEmployees_ReturnsNotFound_WhenEmpty()
        {
            _mockService.Setup(s => s.GetTopRatedEmployees())
                .ReturnsAsync(new List<TopRatedEmployeesDTO>());

            var result = await _controller.GetTopRatedEmployees();

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}