namespace LaWashSmartParking.Simulator.Configuration;

public class SimulatorSettings
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public List<DeviceConfig> Devices { get; set; } = new();
    public List<SlotConfig> Slots { get; set; } = new();
}

public class DeviceConfig
{
    public string SerialNumber { get; set; }
}

public class SlotConfig
{
    public string Location { get; set; }
    public string SerialNumber { get; set; }
}
