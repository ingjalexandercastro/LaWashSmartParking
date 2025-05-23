using LaWashSmartParking.Domain.Entities;

namespace LaWashSmartParking.Application.Interfaces
{
    public interface IParkingSpotRepository
    {
        Task AddAsync(ParkingSpot spot);
        Task UpdateAsync(ParkingSpot spot);
        Task<ParkingSpot?> GetByIdAsync(Guid id);
        Task<IEnumerable<ParkingSpot>> GetAllAsync();
        Task RemoveAsync(Guid id);
        Task<ParkingSpot?> GetByLocationAsync(string location);
        Task<ParkingSpot?> GetByDeviceIdAsync(Guid deviceId);
        Task<ParkingSpot?> GetByAssignedDeviceIdAsync(Guid deviceId);
        Task<(int Free, int Occupied)> GetParkingSpotStatusCountAsync();
        Task<PaginatedResult<ParkingSpot>> GetPagedAsync(int page, int size);
    }
}
