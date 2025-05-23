using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LaWashSmartParking.Infrastructure.Repositories
{
    public class EfIoTDeviceRepository : IIoTDeviceRepository
    {
        private readonly SmartParkingDbContext _context;

        public EfIoTDeviceRepository(SmartParkingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IoTDeviceDto deviceDto)
        {
            var device = new IoTDevice
            {
                Id = deviceDto.Id,
                SerialNumber = deviceDto.SerialNumber,
                RegisteredAt = deviceDto.RegisteredAt
            };

            await _context.ioTDevices.AddAsync(device);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsBySerialNumberAsync(string serialNumber)
        {
            return await _context.ioTDevices
                .AnyAsync(d => d.SerialNumber == serialNumber);
        }

        public async Task<IEnumerable<IoTDeviceDto>> GetAllAsync()
        {
            var devices = await _context.ioTDevices.ToListAsync();
            return devices.Select(device => new IoTDeviceDto
            {
                Id = device.Id,
                SerialNumber = device.SerialNumber,
                RegisteredAt = device.RegisteredAt
            });
        }

        public Task<IoTDevice?> GetByIdAsync(Guid id)
        {
            return _context.ioTDevices
                .FirstOrDefaultAsync(device => device.Id == id);
        }

        public async Task<IoTDeviceDto?> GetBySerialNumberAsync(string serialNumber)
        {
            var device = await _context.ioTDevices
       .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber);

            if (device == null) return null;

            return new IoTDeviceDto
            {
                Id = device.Id,
                SerialNumber = device.SerialNumber,
                RegisteredAt = device.RegisteredAt
            };
        }
    }
}
