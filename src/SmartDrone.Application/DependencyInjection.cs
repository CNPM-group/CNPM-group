using Microsoft.Extensions.DependencyInjection;
using SmartDrone.Application.Stations;

namespace SmartDrone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStationService, StationService>();
        return services;
    }
}
