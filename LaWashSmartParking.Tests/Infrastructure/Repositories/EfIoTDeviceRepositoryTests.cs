using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Infrastructure.Data;
using LaWashSmartParking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LaWashSmartParking.Infrastructure.Tests.Repositories
{
    public class EfIoTDeviceRepositoryTests
    {
 
        private SmartParkingDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SmartParkingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SmartParkingDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDevice()
        {
            var context = CreateContext();
            var repo = new EfIoTDeviceRepository(context);

            var device = new IoTDeviceDto { SerialNumber = "ABC123" };

            await repo.AddAsync(device);

            Assert.Equal(1, await context.ioTDevices.CountAsync());
        }

        [Fact]
        public async Task ExistsBySerialNumberAsync_ShouldReturnTrue_IfExists()
        {
            var context = CreateContext();
            var repo = new EfIoTDeviceRepository(context);
            var device = new IoTDevice { SerialNumber = "XYZ" };
            context.ioTDevices.Add(device);
            await context.SaveChangesAsync();

            var exists = await repo.ExistsBySerialNumberAsync("XYZ");

            Assert.True(exists);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDevice_IfExists()
        {
            var context = CreateContext();
            var repo = new EfIoTDeviceRepository(context);
            var device = new IoTDevice { Id = Guid.NewGuid(), SerialNumber = "GETID" };
            context.ioTDevices.Add(device);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(device.Id);

            Assert.NotNull(result);
            Assert.Equal("GETID", result?.SerialNumber);
        }
    }
}
