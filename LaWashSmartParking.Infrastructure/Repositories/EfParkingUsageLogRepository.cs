using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaWashSmartParking.Infrastructure.Repositories
{
    public class EfParkingUsageLogRepository : IParkingUsageLogRepository
    {
        private readonly SmartParkingDbContext _context;

        public EfParkingUsageLogRepository(SmartParkingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddAsync(ParkingUsageLog log)
        {
            return Task.Run(() =>
            {
                _context.ParkingUsageLogs.Add(log);
                _context.SaveChanges();
            });
        }

        public async Task<IEnumerable<ParkingUsageLogDto>> GetAllAsync()
        {
            return await _context.ParkingUsageLogs
                .Include(log => log.ParkingSpot)
                .Include(log => log.IoTDevice)
                .Select(log => new ParkingUsageLogDto {
                    Id = log.Id,
                    ParkingSpotId = log.ParkingSpotId,
                    IoTDeviceId = log.IoTDeviceId,
                    SerialNumber = log.IoTDevice.SerialNumber,
                    Location = log.ParkingSpot.Location,
                    OccupiedAt = log.OccupiedAt,
                    FreedAt = log.FreedAt,
                    Duration = log.FreedAt.HasValue ? log.FreedAt.Value - log.OccupiedAt : null
                })
                .ToListAsync();
        }


        public Task<IEnumerable<ParkingUsageLog>> GetBySpotIdAsync(Guid spotId)
        {
            return Task.Run(() =>
            {
                return _context.ParkingUsageLogs
                    .Where(log => log.ParkingSpotId == spotId)
                    .AsEnumerable();
            });
        }

        public Task UpdateAsync(ParkingUsageLog log)
        {
            return Task.Run(() =>
            {
                _context.ParkingUsageLogs.Update(log);
                _context.SaveChanges();
            });
        }
    }
}
