using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;
using SmartDrone.Infrastructure.Models;
using SmartDrone.Infrastructure.Repositories;
using SmartDrone.Infrastructure.Services;
using SmartDrone.Services;

namespace SmartDrone;

public static class DependencyContainer
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(o => o
            .UseNpgsql(config.GetConnectionString("Default"))
            .UseSnakeCaseNamingConvention());

        services.AddIdentityCore<AppUser>(o =>
            {
                o.Password.RequiredLength = 8;
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<IDroneRepository, DroneRepository>();

        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddHttpClient<ILlmClient, LlmClient>();

        services.AddScoped<AuthService>();
        services.AddScoped<StationService>();
        services.AddScoped<OrderService>();
        services.AddScoped<CustomerAddressService>();
        services.AddScoped<AiService>();
        services.AddScoped<EtaService>();
        services.AddScoped<DroneService>();

        return services;
    }
}
