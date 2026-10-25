using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartDrone.Application.Abstractions;
using SmartDrone.Infrastructure.Identity;
using SmartDrone.Infrastructure.Persistence;

namespace SmartDrone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {

        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(config.GetConnectionString("Default"))
             .UseSnakeCaseNamingConvention());

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<AppUser>(o =>
            {
                o.Password.RequiredLength = 8;
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.AddScoped<JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
