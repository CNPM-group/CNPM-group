using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;
using SmartDrone.Infrastructure.Models;
using SmartDrone.Infrastructure.Realtime;
using SmartDrone.Infrastructure.Repositories;
using SmartDrone.Infrastructure.Services;
using SmartDrone.Services;

namespace SmartDrone;

public static class DependencyContainer
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<AppDbContext>((sp, o) => o
            .UseNpgsql(config.GetConnectionString("Default"))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

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
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddHttpClient<ILlmClient, LlmClient>();
        services.AddSingleton<IOrderEvents, SignalROrderEvents>();
        services.AddSingleton<IFileStorage, MinioFileStorage>();

        services.AddScoped<AuthService>();
        services.AddScoped<StationService>();
        services.AddScoped<OrderService>();
        services.AddScoped<CustomerAddressService>();
        services.AddScoped<AiService>();
        services.AddScoped<AiInsightService>();
        services.AddScoped<CustomerService>();
        services.AddScoped<AuditService>();
        services.AddScoped<EtaService>();
        services.AddScoped<DroneService>();
        services.AddScoped<DeliveryService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<ReportService>();
        services.AddScoped<UserService>();
        services.AddScoped<PackageImageService>();
        services.AddScoped<TrackingService>();
        services.AddHostedService<DroneSimulator>();

        return services;
    }
}
