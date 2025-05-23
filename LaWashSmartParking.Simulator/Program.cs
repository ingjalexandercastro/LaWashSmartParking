using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LaWashSmartParking.Simulator.Configuration;
using LaWashSmartParking.Simulator.Extensions;
using LaWashSmartParking.Simulator.Services;

namespace LaWashSmartParking.Simulator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, cfg) =>
                    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSimulatorServices(ctx.Configuration);
                })
                .Build();

            var simulator = host.Services.GetRequiredService<IDeviceSimulatorService>();
            await simulator.InitializeAsync();               
            await simulator.RunAsync(CancellationToken.None);
        }
    }
}
