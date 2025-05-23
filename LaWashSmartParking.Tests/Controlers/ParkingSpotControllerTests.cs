using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using LaWashSmartParking.API.Controllers;
using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaWashSmartParking.Domain.Entities;

namespace LaWashSmartParking.Controllers.Tests
{

    public class ParkingSpotControllerTests
    {
        private readonly Mock<IParkingSpotService> _mockService;
        private readonly Mock<IIoTDeviceService> _mockIoTService;
        private readonly ParkingSpotController _controller;

        public ParkingSpotControllerTests()
        {
            _mockService = new Mock<IParkingSpotService>();
            _mockIoTService = new Mock<IIoTDeviceService>();
            _controller = new ParkingSpotController(_mockIoTService.Object, _mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithListOfSpots()
        {
            // Arrange
            var spots = new List<ParkingSpotDto> {
            new ParkingSpotDto { Id = Guid.NewGuid(), Location = "A1" },
            new ParkingSpotDto { Id = Guid.NewGuid(), Location = "B1" }
        };
            _mockService.Setup(s => s.GetAllSpotsAsync()).ReturnsAsync(spots);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSpots = Assert.IsAssignableFrom<IEnumerable<ParkingSpotDto>>(okResult.Value);
            Assert.Equal(2, ((List<ParkingSpotDto>)returnedSpots).Count);
        }

        [Fact]
        public async Task Create_ReturnsCreatedStatus_WhenSuccess()
        {
            var dto = new CreateParkingSpotDto { Location = "C1", SerialNumber = "SN-123456" };
            var device = new IoTDevice {Id = Guid.NewGuid(),SerialNumber = dto.SerialNumber,RegisteredAt = DateTime.UtcNow};

            _mockIoTService.Setup(s => s.GetBySerialNumberAsync(dto.SerialNumber))
                      .ReturnsAsync(new IoTDeviceDto { Id = Guid.NewGuid(), SerialNumber = dto.SerialNumber });

            _mockService.Setup(s => s.AddParkingSpotAsync(dto.Location, It.IsAny<Guid>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenExceptionThrown()
        {
            var dto = new CreateParkingSpotDto { Location = "D1", SerialNumber = "SN-123456" };
            var device = new IoTDevice { Id = Guid.NewGuid(), SerialNumber = dto.SerialNumber, RegisteredAt = DateTime.UtcNow };
            _mockService.Setup(s => s.AddParkingSpotAsync(dto.Location, device.Id))
                        .ThrowsAsync(new Exception("Error"));

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.RemoveSpotAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenExceptionThrown()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.RemoveSpotAsync(id)).ThrowsAsync(new Exception("Not found"));

            var result = await _controller.Delete(id);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task OccupyBySerial_ReturnsOk_WhenSuccess()
        {
            var serial = "SN123";
            _mockService.Setup(s => s.OccupySpotAsync(serial)).Returns(Task.CompletedTask);

            var result = await _controller.OccupyBySerial(serial);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Spot occupied", okResult.Value.ToString());
        }

        [Fact]
        public async Task OccupyBySerial_ReturnsBadRequest_WhenError()
        {
            var serial = "SN456";
            _mockService.Setup(s => s.OccupySpotAsync(serial)).ThrowsAsync(new Exception("Device not found"));

            var result = await _controller.OccupyBySerial(serial);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Device not found", badRequest.Value.ToString());
        }

        [Fact]
        public async Task FreeBySerial_ReturnsOk_WhenSuccess()
        {
            var serial = "SN789";
            _mockService.Setup(s => s.FreeSpotAsync(serial)).Returns(Task.CompletedTask);

            var result = await _controller.FreeBySerial(serial);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Spot freed", okResult.Value.ToString());
        }

        [Fact]
        public async Task FreeBySerial_ReturnsBadRequest_WhenError()
        {
            var serial = "SN987";
            _mockService.Setup(s => s.FreeSpotAsync(serial)).ThrowsAsync(new Exception("Error freeing"));

            var result = await _controller.FreeBySerial(serial);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error freeing", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetParkingStatusCount_ReturnsOk_WithCounts()
        {
            // Arrange
            _mockService.Setup(s => s.GetParkingSpotStatusCountAsync())
                        .ReturnsAsync((5, 3));

            // Act
            var result = await _controller.GetParkingStatusCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ParkingSpotStatusCountDto>(okResult.Value);

            Assert.Equal(5, (int)value.Free);
            Assert.Equal(3, (int)value.Occupied);
        }
    }
}