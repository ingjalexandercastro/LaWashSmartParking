using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaWashSmartParking.Application.Services
{
    public class ParkingSpotService : IParkingSpotService
    {
        private readonly IParkingSpotRepository _parkingRepo;
        private readonly IIoTDeviceRepository _deviceRepo;
        private readonly IParkingUsageLogRepository _logRepo;

        public ParkingSpotService(
            IParkingSpotRepository parkingRepo,
            IIoTDeviceRepository deviceRepo,
            IParkingUsageLogRepository logRepo)
        {
            _parkingRepo = parkingRepo;
            _deviceRepo = deviceRepo;
            _logRepo = logRepo;
        }

        // POST /api/parking-spots
        public async Task AddParkingSpotAsync(string location, Guid deviceId)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required.");

            if (await _parkingRepo.GetByLocationAsync(location) != null)
                throw new Exception("A parking spot with this location already exists.");

            if (await _parkingRepo.GetByDeviceIdAsync(deviceId) != null)
                throw new Exception("This IoT device is already assigned to another spot.");

            var device = await _deviceRepo.GetByIdAsync(deviceId);
            if (device == null)
                throw new Exception("IoT device not found.");

            var spot = new ParkingSpot
            {
                Id = Guid.NewGuid(),
                Location = location,
                Status = ParkingStatus.Free,
                AssignedDeviceId = deviceId
            };

            await _parkingRepo.AddAsync(spot);
        }

        // GET /api/parking-spots
        async Task<IEnumerable<ParkingSpotDto>> IParkingSpotService.GetAllSpotsAsync()
        {
            var entities = await _parkingRepo.GetAllAsync();

            return entities.Select(e => new ParkingSpotDto
            {
                Id = e.Id,
                Location = e.Location,
                AssignedDeviceId = e.AssignedDeviceId,
                Status = e.Status,
            });
        }

        // DELETE /api/parking-spots/{id}
        public async Task RemoveSpotAsync(Guid spotId)
        {
            var spot = await _parkingRepo.GetByIdAsync(spotId);

            if (spot == null)
                throw new Exception("Parking spot not found.");

            if (spot.Status == ParkingStatus.Occupied)
                throw new Exception("Cannot remove an occupied parking spot.");

            await _parkingRepo.RemoveAsync(spotId);
        }

        // POST /api/parking-spots/{id}/occupy
        public async Task OccupySpotAsync(string serialNumber)
        {
            var device = await _deviceRepo.GetBySerialNumberAsync(serialNumber)
        ?? throw new Exception("Device not found.");

            var spot = await _parkingRepo.GetByAssignedDeviceIdAsync(device.Id)
                ?? throw new Exception("No parking spot assigned to this device.");

            if (spot.Status == ParkingStatus.Occupied)
                throw new Exception("The spot is already occupied.");

            spot.Status = ParkingStatus.Occupied;
            await _parkingRepo.UpdateAsync(spot);

            var log = new ParkingUsageLog
            {
                Id = Guid.NewGuid(),
                ParkingSpotId = spot.Id,
                IoTDeviceId = device.Id,
                OccupiedAt = DateTime.UtcNow
            };

            await _logRepo.AddAsync(log);
        }

        // POST /api/parking-spots/{id}/free
        public async Task FreeSpotAsync(string serialNumber)
        {
            var device = await _deviceRepo.GetBySerialNumberAsync(serialNumber)
                ?? throw new Exception("Device not found.");

            var spot = await _parkingRepo.GetByAssignedDeviceIdAsync(device.Id)
                ?? throw new Exception("No parking spot assigned to this device.");

            if (spot.Status == ParkingStatus.Free)
                throw new Exception("The spot is already free.");

            spot.Status = ParkingStatus.Free;
            await _parkingRepo.UpdateAsync(spot);

            var logs = await _logRepo.GetBySpotIdAsync(spot.Id);
            var lastLog = logs
                .Where(l => l.FreedAt == null && l.IoTDeviceId == device.Id)
                .OrderByDescending(l => l.OccupiedAt)
                .FirstOrDefault();

            if (lastLog != null)
            {
                lastLog.FreedAt = DateTime.UtcNow;

                await _logRepo.UpdateAsync(lastLog);
            }
        }

        // GET /api/parking-spots/status-count
        public async Task<(int Free, int Occupied)> GetParkingSpotStatusCountAsync()
        {
            var (freeCount, occupiedCount) = await _parkingRepo.GetParkingSpotStatusCountAsync();
            return (Free: freeCount, Occupied: occupiedCount);
        }

        // GET /api/parking-spots/paged
        public Task<PaginatedResult<ParkingSpotDto>> GetPagedAsync(int page, int size)
        {
            return _parkingRepo.GetPagedAsync(page, size)
                .ContinueWith(t =>
                {
                    var paged = t.Result;
                    var dtos = paged.Items.Select(ps => new ParkingSpotDto
                    {
                        Id = ps.Id,
                        Location = ps.Location,
                        Status = ps.Status,
                        AssignedDeviceId = ps.AssignedDeviceId
                    });

                    return new PaginatedResult<ParkingSpotDto>
                    {
                        Items = dtos,
                        TotalItems = paged.TotalItems,
                        PageNumber = paged.PageNumber,
                        PageSize = paged.PageSize
                    };
                });
        }
    }
}