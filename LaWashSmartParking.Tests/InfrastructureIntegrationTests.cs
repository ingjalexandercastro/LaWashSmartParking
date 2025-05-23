using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Domain.Enum;
using LaWashSmartParking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LaWashSmartParking.Tests
{
    public class InfrastructureIntegrationTests
    {
        [Fact]
        public async Task CanInsertParkingSpotWithDevice()
        {
            var options = new DbContextOptionsBuilder<SmartParkingDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            using var context = new SmartParkingDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            var device = new IoTDevice { Id = Guid.NewGuid(), SerialNumber = "TestSensor" };
            var spot = new ParkingSpot
            {
                Id = Guid.NewGuid(),
                Location = "T-01",
                Status = ParkingStatus.Free,
                AssignedDeviceId = device.Id
            };

            context.ioTDevices.Add(device);
            context.ParkingSpots.Add(spot);
            await context.SaveChangesAsync();

            var count = await context.ParkingSpots.CountAsync();
            Assert.Equal(1, count);
        }
    }
}
