using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Domain.Entities;

namespace LaWashSmartParking.Application.Interfaces
{
    public interface IIoTDeviceRepository
    {
        Task<IoTDevice?> GetByIdAsync(Guid id);
        Task AddAsync(IoTDeviceDto device);
        Task<IEnumerable<IoTDeviceDto>> GetAllAsync();
        Task<bool> ExistsBySerialNumberAsync(string serialNumber);
        Task<IoTDeviceDto?> GetBySerialNumberAsync(string serialNumber);
    }
}
