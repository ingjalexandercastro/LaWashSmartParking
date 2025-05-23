using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LaWashSmartParking.Infrastructure.Data
{
    public class SmartParkingDbContextFactory : IDesignTimeDbContextFactory<SmartParkingDbContext>
    {
        public SmartParkingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartParkingDbContext>();
            optionsBuilder.UseSqlite("Data Source=smart_parking.db");

            return new SmartParkingDbContext(optionsBuilder.Options);
        }
    }
}