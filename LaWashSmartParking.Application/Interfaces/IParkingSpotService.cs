using LaWashSmartParking.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaWashSmartParking.Application.Interfaces
{
    public interface IParkingSpotService
    {
        Task<IEnumerable<ParkingSpotDto>> GetAllSpotsAsync();
        Task AddParkingSpotAsync(string location, Guid deviceId);
        Task RemoveSpotAsync(Guid id);
        Task OccupySpotAsync(string serialNumber);
        Task FreeSpotAsync(string serialNumber);
        Task<(int Free, int Occupied)> GetParkingSpotStatusCountAsync();
        Task<PaginatedResult<ParkingSpotDto>> GetPagedAsync(int page, int size);
    }
}
