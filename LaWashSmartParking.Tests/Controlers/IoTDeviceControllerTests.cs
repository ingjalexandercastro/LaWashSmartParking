using LaWashSmartParking.API.Controllers;
using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Application.Services;
using LaWashSmartParking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LaWashSmartParking.Controllers.Tests
{

    public class IoTDeviceControllerTests
    {
        private readonly Mock<IIoTDeviceService> _mockService;
        private readonly IoTDeviceController _controller;

        public IoTDeviceControllerTests()
        {
            _mockService = new Mock<IIoTDeviceService>();
            _controller = new IoTDeviceController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult_WhenDeviceIsRegistered()
        {
            //Arrange
            var serialNumber = "SN12345";
            var createdDevice = new IoTDeviceDto { Id = Guid.NewGuid(), SerialNumber = serialNumber };
            _mockService.Setup(s => s.RegisterAsync(It.IsAny<string>()))
            .ReturnsAsync(new IoTDeviceDto { Id = createdDevice.Id, SerialNumber = createdDevice.SerialNumber });

            var dto = new CreateIoTDeviceDto { SerialNumber = serialNumber };

            //Act
            var result = await _controller.Create(dto);

            //Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(IoTDeviceController.GetAll), createdAtActionResult.ActionName);
            var returnedDevice = Assert.IsType<IoTDeviceDto>(createdAtActionResult.Value);

            Assert.Equal(createdDevice.Id, returnedDevice.Id);
            Assert.Equal(createdDevice.SerialNumber, returnedDevice.SerialNumber);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockService.Setup(s => s.RegisterAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error registering device"));

            var dto = new CreateIoTDeviceDto { SerialNumber = "SN12345" };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfDevices()
        {
            // Arrange
            var devices = new List<IoTDeviceDto>
                {
                    new IoTDeviceDto { Id = Guid.NewGuid(), SerialNumber = "SN1" },
                    new IoTDeviceDto { Id = Guid.NewGuid(), SerialNumber = "SN2" }
                };
            _mockService.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(devices);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDevices = Assert.IsAssignableFrom<IEnumerable<IoTDeviceDto>>(okResult.Value);
            Assert.Equal(2, returnedDevices.Count()); 
        }


    }
}