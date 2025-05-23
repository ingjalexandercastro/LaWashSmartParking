using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Domain.Entities;

namespace LaWashSmartParking.Application.Services
{
    public class IoTDeviceService : IIoTDeviceService
    {
        private readonly IIoTDeviceRepository _repo;

        public IoTDeviceService(IIoTDeviceRepository repo)
        {
            _repo = repo;
        }

        // POST /api/devices
        async Task<IoTDeviceDto> IIoTDeviceService.RegisterAsync(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw new ArgumentException("Serial number is required.");

            var exists = await _repo.ExistsBySerialNumberAsync(serialNumber);

            if (exists)
                throw new InvalidOperationException($"A device with the serial number '{serialNumber}' is already registered.");


            var device = new IoTDeviceDto
            {
                Id = Guid.NewGuid(),
                SerialNumber = serialNumber,
                RegisteredAt = DateTime.UtcNow
            };

            await _repo.AddAsync(device);
            return device;
        }

        async Task<IEnumerable<IoTDeviceDto>> IIoTDeviceService.GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        async Task<IoTDeviceDto?> IIoTDeviceService.GetBySerialNumberAsync(string serialNumber)
        {
            return await _repo.GetBySerialNumberAsync(serialNumber);
        }
    }
}
