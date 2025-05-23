using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using LaWashSmartParking.Simulator.Configuration;
using LaWashSmartParking.Simulator.Communication;
using LaWashSmartParking.Simulator.Services;

namespace LaWashSmartParking.Simulator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSimulatorServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SimulatorSettings>(
                configuration.GetSection("SimulatorSettings"));

            services.AddHttpClient<IApiClient, ApiClient>((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<SimulatorSettings>>().Value;
                client.BaseAddress = new Uri(settings.ApiBaseUrl);
            });

            services.AddSingleton<IDeviceSimulatorService, DeviceSimulatorService>();
        }
    }
}
