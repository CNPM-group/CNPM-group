using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartDrone.Domain.Common;
using SmartDrone.Domain.Entities;
using SmartDrone.Domain.Enums;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
        var db = sp.GetRequiredService<AppDbContext>();
        
if (!db.Database.GetMigrations().Any())
        {
            logger.LogWarning(
                "Chưa có migration nào. Chạy: dotnet ef migrations add Initial -p src/SmartDrone.Infrastructure -s src/SmartDrone.Api -o Persistence/Migrations");
            return;
        }

        await db.Database.MigrateAsync();

        // 1) Roles
        var roleManager = sp.GetRequiredService<RoleManager<AppRole>>();
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new AppRole { Name = role });
        }

        // 2) Stations mẫu (tọa độ xấp xỉ, sửa lại cho đúng khu của nhóm)
        if (!await db.Stations.AnyAsync())
        {
            db.Stations.AddRange(
                NewStation("Trạm KTX Khu A", 10.883000m, 106.807000m),
                NewStation("Trạm Thư viện", 10.881500m, 106.805500m),
                NewStation("Trạm Nhà điều hành", 10.882200m, 106.808800m),
                NewStation("Trạm Căng tin", 10.884100m, 106.806200m),
                NewStation("Trạm Cổng chính", 10.880500m, 106.807900m));
            await db.SaveChangesAsync();
        }

        // 3) Demo users (chỉ bật qua config Seed:DemoUsers, mặc định chỉ Development)
       if (bool.TryParse(config["Seed:DemoUsers"], out var seedDemoUsers)
    && seedDemoUsers)
            var password = config["Seed:DemoPassword"];
            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("Seed:DemoUsers bật nhưng thiếu Seed:DemoPassword, bỏ qua.");
                return;
            }

            var users = sp.GetRequiredService<UserManager<AppUser>>();
            foreach (var role in Roles.All)
            {
                var email = $"{role.ToLowerInvariant()}@drone.local";
                if (await users.FindByEmailAsync(email) is not null) continue;

                var user = new AppUser { UserName = email, Email = email, FullName = $"Demo {role}", EmailConfirmed = true };
                var result = await users.CreateAsync(user, password);
                if (result.Succeeded)
                    await users.AddToRoleAsync(user, role);
                else
                    logger.LogWarning("Seed user {Email} lỗi: {Errors}", email,
                        string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static LandingStation NewStation(string name, decimal lat, decimal lng) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Latitude = lat,
        Longitude = lng,
        Status = StationStatus.Available,
        CreatedAtUtc = DateTime.UtcNow
    };
}
