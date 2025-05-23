using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Infrastructure.Data;
using LaWashSmartParking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LaWashSmartParking.Infrastructure.Tests.Repositories
{
    public class EfParkingUsageLogRepositoryTests
    {
        private SmartParkingDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SmartParkingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SmartParkingDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddLog()
        {
            var context = CreateContext();
            var repo = new EfParkingUsageLogRepository(context);

            var log = new ParkingUsageLog
            {
                Id = Guid.NewGuid(),
                ParkingSpotId = Guid.NewGuid(),
                IoTDeviceId = Guid.NewGuid(),
                OccupiedAt = DateTime.UtcNow
            };

            await repo.AddAsync(log);

            Assert.Single(context.ParkingUsageLogs);
        }

        [Fact]
        public async Task GetBySpotIdAsync_ShouldReturnLogs()
        {
            var context = CreateContext();
            var repo = new EfParkingUsageLogRepository(context);
            var spotId = Guid.NewGuid();

            context.ParkingUsageLogs.AddRange(
                new ParkingUsageLog { ParkingSpotId = spotId, IoTDeviceId = Guid.NewGuid(), OccupiedAt = DateTime.UtcNow },
                new ParkingUsageLog { ParkingSpotId = spotId, IoTDeviceId = Guid.NewGuid(), OccupiedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var logs = await repo.GetBySpotIdAsync(spotId);
            Assert.Equal(2, logs.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateLog()
        {
            var context = CreateContext();
            var repo = new EfParkingUsageLogRepository(context);
            var log = new ParkingUsageLog
            {
                Id = Guid.NewGuid(),
                ParkingSpotId = Guid.NewGuid(),
                IoTDeviceId = Guid.NewGuid(),
                OccupiedAt = DateTime.UtcNow
            };

            context.ParkingUsageLogs.Add(log);
            await context.SaveChangesAsync();

            log.FreedAt = log.OccupiedAt.AddMinutes(30);
            await repo.UpdateAsync(log);

            var updated = await context.ParkingUsageLogs.FindAsync(log.Id);
            Assert.Equal(log.FreedAt, updated?.FreedAt);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnDtoWithDetails()
        {
            var context = CreateContext();
            var repo = new EfParkingUsageLogRepository(context);

            var spot = new ParkingSpot { Id = Guid.NewGuid(), Location = "Zone X", Status = Domain.Enum.ParkingStatus.Free };
            var device = new IoTDevice { Id = Guid.NewGuid(), SerialNumber = "SN12345" };
            var log = new ParkingUsageLog
            {
                Id = Guid.NewGuid(),
                ParkingSpotId = spot.Id,
                IoTDeviceId = device.Id,
                OccupiedAt = DateTime.UtcNow,
                FreedAt = DateTime.UtcNow.AddMinutes(10),
                ParkingSpot = spot,
                IoTDevice = device
            };

            context.ParkingSpots.Add(spot);
            context.ioTDevices.Add(device);
            context.ParkingUsageLogs.Add(log);
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            var logDto = result.FirstOrDefault();
            Assert.NotNull(logDto);
            Assert.Equal("Zone X", logDto?.Location);
            Assert.Equal("SN12345", logDto?.SerialNumber);
        }
    }
}
