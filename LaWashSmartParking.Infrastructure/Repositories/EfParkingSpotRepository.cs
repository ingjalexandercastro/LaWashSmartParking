

using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Domain.Enum;
using LaWashSmartParking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LaWashSmartParking.Infrastructure.Repositories
{
    public class EfParkingSpotRepository : IParkingSpotRepository
    {
        private readonly SmartParkingDbContext _context;

        public EfParkingSpotRepository(SmartParkingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ParkingSpot spot)
        {
            _context.ParkingSpots.Add(spot);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ParkingSpot>> GetAllAsync()
        {
            return await _context.ParkingSpots.ToListAsync();
        }

        public async Task<ParkingSpot?> GetByAssignedDeviceIdAsync(Guid deviceId)
        {
            return await _context.ParkingSpots
                .FirstOrDefaultAsync(s => s.AssignedDeviceId == deviceId);
        }

        public Task<ParkingSpot?> GetByDeviceIdAsync(Guid deviceId)
        {
            return _context.ParkingSpots
                 .FirstOrDefaultAsync(spot => spot.AssignedDeviceId == deviceId);
        }

        public Task<ParkingSpot?> GetByIdAsync(Guid id)
        {
            return _context.ParkingSpots
                 .FirstOrDefaultAsync(spot => spot.Id == id);
        }

        public Task<ParkingSpot?> GetByLocationAsync(string location)
        {
            return _context.ParkingSpots
                 .FirstOrDefaultAsync(spot => spot.Location == location);
        }

        public async Task<(int Free, int Occupied)> GetParkingSpotStatusCountAsync()
        {
            var freeCount = await _context.ParkingSpots
                .CountAsync(s => s.Status == ParkingStatus.Free);

            var occupiedCount = await _context.ParkingSpots
                .CountAsync(s => s.Status == ParkingStatus.Occupied);

            return (Free: freeCount, Occupied: occupiedCount);
        }

        public Task RemoveAsync(Guid id)
        {
            return _context.ParkingSpots
                 .Where(spot => spot.Id == id)
                 .ExecuteDeleteAsync();
        }

        public Task UpdateAsync(ParkingSpot spot)
        {
            return _context.ParkingSpots
                 .Where(s => s.Id == spot.Id)
                 .ExecuteUpdateAsync(s => s.SetProperty(p => p.Location, spot.Location)
                                            .SetProperty(p => p.AssignedDeviceId, spot.AssignedDeviceId)
                                            .SetProperty(p => p.Status, spot.Status));
        }

        public async Task<PaginatedResult<ParkingSpot>> GetPagedAsync(int page, int size)
        {
            var query = _context.ParkingSpots.AsNoTracking();
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(ps => ps.Location)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PaginatedResult<ParkingSpot>
            {
                Items = items,
                TotalItems = total,
                PageNumber = page,
                PageSize = size
            };
        }
    }
}
