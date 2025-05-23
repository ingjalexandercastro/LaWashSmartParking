using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Domain.Enum;
using LaWashSmartParking.Infrastructure.Data;
using LaWashSmartParking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LaWashSmartParking.Infrastructure.Tests.Repositories
{
    public class EfParkingSpotRepositoryTests
    {
        private SmartParkingDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SmartParkingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SmartParkingDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddParkingSpot()
        {
            var context = CreateContext();
            var repo = new EfParkingSpotRepository(context);

            var spot = new ParkingSpot { Location = "Zone A", Status = ParkingStatus.Free };

            await repo.AddAsync(spot);

            Assert.Single(await context.ParkingSpots.ToListAsync());
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldReturnSpot_IfExists()
        {
            var context = CreateContext();
            var repo = new EfParkingSpotRepository(context);

            var spot = new ParkingSpot { Location = "Zone B", Status = ParkingStatus.Free };
            context.ParkingSpots.Add(spot);
            await context.SaveChangesAsync();

            var result = await repo.GetByLocationAsync("Zone B");

            Assert.NotNull(result);
            Assert.Equal("Zone B", result?.Location);
        }


        [Fact]
        public async Task GetParkingSpotStatusCountAsync_ShouldReturnCorrectCounts()
        {
            var context = CreateContext();
            var repo = new EfParkingSpotRepository(context);

            context.ParkingSpots.AddRange(
                new ParkingSpot { Location = "1", Status = ParkingStatus.Free },
                new ParkingSpot { Location = "2", Status = ParkingStatus.Free },
                new ParkingSpot { Location = "3", Status = ParkingStatus.Occupied }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetParkingSpotStatusCountAsync();

            Assert.Equal(2, result.Free);
            Assert.Equal(1, result.Occupied);
        }
    }
}
