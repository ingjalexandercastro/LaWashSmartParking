using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Domain.Entities;

namespace LaWashSmartParking.Simulator.Communication;

public interface IApiClient
{
    Task RegisterDeviceAsync(IoTDevice device);
    Task RegisterSlotAsync(CreateParkingSpotDto slot);
    Task OccupySpotAsync(string serialNumber);
    Task FreeSpotAsync(string serialNumber);
}
