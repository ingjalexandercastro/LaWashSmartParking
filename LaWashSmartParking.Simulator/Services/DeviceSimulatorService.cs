using LaWashSmartParking.Simulator.Communication;
using LaWashSmartParking.Domain.Entities;
using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Simulator.Configuration;
using Microsoft.Extensions.Options;

namespace LaWashSmartParking.Simulator.Services;

public class DeviceSimulatorService : IDeviceSimulatorService
{
    private readonly IApiClient _apiClient;

    private readonly List<IoTDevice> _devices;
    private readonly List<CreateParkingSpotDto> _slots;

    public DeviceSimulatorService(IApiClient apiClient, IOptions<SimulatorSettings> opts)
    {
        _apiClient = apiClient;
        var settings = opts.Value;

        _devices = settings.Devices
            .Select(d => new IoTDevice { Id = Guid.NewGuid(), SerialNumber = d.SerialNumber })
            .ToList();

        _slots = settings.Slots
            .Select(s => new CreateParkingSpotDto
            {
                SerialNumber = s.SerialNumber,
                Location = s.Location
            })
            .ToList();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var random = new Random();

        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var slot in _slots)
            {
                var occupy = random.Next(0, 2) == 0;
                if (occupy)
                    await _apiClient.OccupySpotAsync(slot.SerialNumber);
                else
                    await _apiClient.FreeSpotAsync(slot.SerialNumber);

                await Task.Delay(200);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }

    public async Task InitializeAsync()
    {
        foreach (var device in _devices)
        {
            try
            {
                await _apiClient.RegisterDeviceAsync(device); 
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("409") || ex.Message.Contains("AlreadyExists"))
            {
                Console.WriteLine($"Device {device.Id} already register.");
            }
        }

        foreach (var slot in _slots)
        {
            try
            {
                await _apiClient.RegisterSlotAsync(slot);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("409") || ex.Message.Contains("AlreadyExists"))
            {
                Console.WriteLine($"Slot {slot.SerialNumber} already register.");
            }
        }
    }
}
