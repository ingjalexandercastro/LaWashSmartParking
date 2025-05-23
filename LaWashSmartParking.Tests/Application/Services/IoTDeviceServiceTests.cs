using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Application.Services;
using LaWashSmartParking.Domain.Entities;
using Moq;
using Xunit;

namespace LaWashSmartParking.Application.Tests.Services
{
    public class IoTDeviceServiceTests
    {
        private readonly Mock<IIoTDeviceRepository> _mockRepo;
        private readonly IIoTDeviceService _service;

        public IoTDeviceServiceTests()
        {
            _mockRepo = new Mock<IIoTDeviceRepository>();
            _service = new IoTDeviceService(_mockRepo.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithValidSerialNumber_ShouldRegisterDevice()
        {
            // Arrange
            var serial = "DEVICE-1234";
            _mockRepo.Setup(r => r.ExistsBySerialNumberAsync(serial)).ReturnsAsync(false);

            // Act
            var result = await _service.RegisterAsync(serial);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serial, result.SerialNumber);
            _mockRepo.Verify(r => r.AddAsync(It.Is<IoTDeviceDto>(d => d.SerialNumber == serial)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RegisterAsync_WithInvalidSerialNumber_ShouldThrowArgumentException(string? invalidSerial)
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.RegisterAsync(invalidSerial));
            Assert.Equal("Serial number is required.", ex.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingSerialNumber_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var serial = "DEVICE-5678";
            _mockRepo.Setup(r => r.ExistsBySerialNumberAsync(serial)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(serial));
            Assert.Equal($"A device with the serial number '{serial}' is already registered.", ex.Message);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDevices()
        {
            // Arrange
            var devices = new List<IoTDeviceDto>
            {
                new IoTDeviceDto { Id = Guid.NewGuid(), SerialNumber = "DEVICE-1" },
                new IoTDeviceDto { Id = Guid.NewGuid(), SerialNumber = "DEVICE-2" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(devices);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, d => d.SerialNumber == "DEVICE-1");
            Assert.Contains(result, d => d.SerialNumber == "DEVICE-2");
        }
    }
}