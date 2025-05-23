using LaWashSmartParking.Application.DTOs;

namespace LaWashSmartParking.Application.Interfaces
{
    public interface IIoTDeviceService
    {
        Task<IoTDeviceDto> RegisterAsync(string serialNumber);
        Task<IEnumerable<IoTDeviceDto>> GetAllAsync();
        Task<IoTDeviceDto?> GetBySerialNumberAsync(string serialNumber);
    }
}
    