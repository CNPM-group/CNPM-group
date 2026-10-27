using Microsoft.Extensions.DependencyInjection;
using SmartDrone.Application.Stations;
using SmartDrone.Application.Orders;

namespace SmartDrone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStationService, StationService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
