namespace LaWashSmartParking.Simulator.Services;

public interface IDeviceSimulatorService
{
    Task InitializeAsync();
    Task RunAsync(CancellationToken cancellationToken);
}
