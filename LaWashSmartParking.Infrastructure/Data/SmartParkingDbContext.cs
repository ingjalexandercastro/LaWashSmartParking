using LaWashSmartParking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaWashSmartParking.Infrastructure.Data
{
    public class SmartParkingDbContext : DbContext
    {
        public SmartParkingDbContext(DbContextOptions<SmartParkingDbContext> options)
            : base(options)
        {
        }

        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<ParkingUsageLog> ParkingUsageLogs { get; set; }
        public DbSet<IoTDevice> ioTDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ParkingSpot>()
                .HasIndex(p => p.Location)
                .IsUnique();

            modelBuilder.Entity<ParkingSpot>()
                .HasIndex(p => p.AssignedDeviceId)
                .IsUnique()
                .HasFilter("[AssignedDeviceId] IS NOT NULL");

            modelBuilder.Entity<IoTDevice>()
                .HasIndex(d => d.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<ParkingUsageLog>()
                .HasOne(p => p.ParkingSpot)
                .WithMany(s => s.ParkingUsageLogs)
                .HasForeignKey(p => p.ParkingSpotId);

            modelBuilder.Entity<ParkingUsageLog>()
                .HasOne(p => p.IoTDevice)
                .WithMany(d => d.ParkingUsageLogs)
                .HasForeignKey(p => p.IoTDeviceId);
        }
    }
}
