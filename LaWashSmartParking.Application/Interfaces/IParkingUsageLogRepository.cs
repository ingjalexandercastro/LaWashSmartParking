using LaWashSmartParking.Domain.Entities;

namespace LaWashSmartParking.Application.Interfaces
{
    public interface IParkingUsageLogRepository
    {
        Task AddAsync(ParkingUsageLog log);
        Task<IEnumerable<ParkingUsageLog>> GetBySpotIdAsync(Guid spotId);
        Task UpdateAsync(ParkingUsageLog log);
        Task<IEnumerable<ParkingUsageLogDto>> GetAllAsync();
    }
}
