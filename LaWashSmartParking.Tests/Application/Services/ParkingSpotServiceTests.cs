
using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Application.Services;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Domain.Enum;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LaWashSmartParking.Application.Tests.Services
{
    public class ParkingSpotServiceTests
    {
        private readonly Mock<IParkingSpotRepository> _parkingRepoMock;
        private readonly Mock<IIoTDeviceRepository> _deviceRepoMock;
        private readonly Mock<IParkingUsageLogRepository> _logRepoMock;
        private readonly IParkingSpotService _service;

        public ParkingSpotServiceTests()
        {
            _parkingRepoMock = new Mock<IParkingSpotRepository>();
            _deviceRepoMock = new Mock<IIoTDeviceRepository>();
            _logRepoMock = new Mock<IParkingUsageLogRepository>();

            _service = new ParkingSpotService(
                _parkingRepoMock.Object,
                _deviceRepoMock.Object,
                _logRepoMock.Object
            );
        }

        [Fact]
        public async Task AddParkingSpotAsync_ShouldThrowException_WhenLocationExists()
        {
            var location = "A1";
            var deviceId = Guid.NewGuid();

            _parkingRepoMock.Setup(r => r.GetByLocationAsync(location))
                .ReturnsAsync(new ParkingSpot());

            await Assert.ThrowsAsync<Exception>(() =>
                _service.AddParkingSpotAsync(location, deviceId));
        }

        [Fact]
        public async Task AddParkingSpotAsync_ShouldThrowException_WhenDeviceAlreadyAssigned()
        {
            var location = "B1";
            var deviceId = Guid.NewGuid();

            _parkingRepoMock.Setup(r => r.GetByLocationAsync(location))
                .ReturnsAsync((ParkingSpot)null);

            _parkingRepoMock.Setup(r => r.GetByDeviceIdAsync(deviceId))
                .ReturnsAsync(new ParkingSpot());

            await Assert.ThrowsAsync<Exception>(() =>
                _service.AddParkingSpotAsync(location, deviceId));
        }

        [Fact]
        public async Task AddParkingSpotAsync_ShouldThrowException_WhenDeviceNotFound()
        {
            var location = "C1";
            var deviceId = Guid.NewGuid();

            _parkingRepoMock.Setup(r => r.GetByLocationAsync(location)).ReturnsAsync((ParkingSpot)null);
            _parkingRepoMock.Setup(r => r.GetByDeviceIdAsync(deviceId)).ReturnsAsync((ParkingSpot)null);
            _deviceRepoMock.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync((IoTDevice)null);

            await Assert.ThrowsAsync<Exception>(() =>
                _service.AddParkingSpotAsync(location, deviceId));
        }

        [Fact]
        public async Task GetAllSpotsAsync_ShouldReturnListOfSpots()
        {
            var spots = new List<ParkingSpot> {
                new ParkingSpot { Id = Guid.NewGuid(), Location = "D1", Status = ParkingStatus.Free }
            };

            _parkingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

            var result = await _service.GetAllSpotsAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task RemoveSpotAsync_ShouldThrowException_WhenSpotIsOccupied()
        {
            var spotId = Guid.NewGuid();
            var spot = new ParkingSpot { Id = spotId, Status = ParkingStatus.Occupied };

            _parkingRepoMock.Setup(r => r.GetByIdAsync(spotId)).ReturnsAsync(spot);

            await Assert.ThrowsAsync<Exception>(() => _service.RemoveSpotAsync(spotId));
        }

        [Fact]
        public async Task OccupySpotAsync_ShouldThrowException_WhenSpotAlreadyOccupied()
        {
            var serial = "XYZ";
            var deviceId = Guid.NewGuid();
            var device = new IoTDeviceDto { Id = deviceId, SerialNumber = serial };
            var spot = new ParkingSpot { AssignedDeviceId = deviceId, Status = ParkingStatus.Occupied };

            _deviceRepoMock.Setup(r => r.GetBySerialNumberAsync(serial)).ReturnsAsync(device);
            _parkingRepoMock.Setup(r => r.GetByAssignedDeviceIdAsync(deviceId)).ReturnsAsync(spot);

            await Assert.ThrowsAsync<Exception>(() => _service.OccupySpotAsync(serial));
        }

        [Fact]
        public async Task FreeSpotAsync_ShouldUpdateStatus_AndSetFreedAtInLog()
        {
            var serial = "XYZ";
            var deviceId = Guid.NewGuid();
            var spotId = Guid.NewGuid();

            var device = new IoTDeviceDto { Id = deviceId };
            var spot = new ParkingSpot { Id = spotId, AssignedDeviceId = deviceId, Status = ParkingStatus.Occupied };
            var log = new ParkingUsageLog
            {
                Id = Guid.NewGuid(),
                ParkingSpotId = spotId,
                IoTDeviceId = deviceId,
                OccupiedAt = DateTime.UtcNow,
                FreedAt = null
            };

            _deviceRepoMock.Setup(r => r.GetBySerialNumberAsync(serial)).ReturnsAsync(device);
            _parkingRepoMock.Setup(r => r.GetByAssignedDeviceIdAsync(deviceId)).ReturnsAsync(spot);
            _logRepoMock.Setup(r => r.GetBySpotIdAsync(spotId)).ReturnsAsync(new List<ParkingUsageLog> { log });

            await _service.FreeSpotAsync(serial);

            _parkingRepoMock.Verify(r => r.UpdateAsync(It.Is<ParkingSpot>(s => s.Status == ParkingStatus.Free)), Times.Once);
            _logRepoMock.Verify(r => r.UpdateAsync(It.Is<ParkingUsageLog>(l => l.FreedAt != null)), Times.Once);
        }

        [Fact]
        public async Task GetParkingSpotStatusCountAsync_ShouldReturnTuple()
        {
            _parkingRepoMock.Setup(r => r.GetParkingSpotStatusCountAsync()).ReturnsAsync((2, 3));

            var (free, occupied) = await _service.GetParkingSpotStatusCountAsync();

            Assert.Equal(2, free);
            Assert.Equal(3, occupied);
        }
    }
}
