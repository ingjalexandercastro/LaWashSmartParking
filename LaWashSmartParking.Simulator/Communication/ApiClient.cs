using System.Text.Json;
using System.Text;
using LaWashSmartParking.Domain.Entities;
using System.Net.Http.Json;
using LaWashSmartParking.Application.DTOs;

namespace LaWashSmartParking.Simulator.Communication;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task RegisterDeviceAsync(IoTDevice device)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/iot-devices", device);
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.Conflict)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex){
                throw new HttpRequestException($"Error al registrar dispositivo: {response.StatusCode} - {ex.Message}");
            }
        }
    }

    public async Task RegisterSlotAsync(CreateParkingSpotDto slot)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/ParkingSpot", slot);
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.Conflict)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
            {
                throw new HttpRequestException($"Error al registrar slot: {response.StatusCode} - {ex.Message}");
            }
        }
    }

    public async Task OccupySpotAsync(string serialNumber)
    {
        var response = await _httpClient.PostAsync($"/api/ParkingSpot/{serialNumber}/occupy", null);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Failed OCCUPY {serialNumber}: {response.StatusCode} - {content}");
        }
    }

    public async Task FreeSpotAsync(string serialNumber)
    {
        var response = await _httpClient.PostAsync($"/api/ParkingSpot/{serialNumber}/free", null);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Failed FREE {serialNumber}: {response.StatusCode} - {content}");
        }
    }
}
