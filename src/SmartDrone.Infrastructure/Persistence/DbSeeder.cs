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
            logger.LogWarning("Chưa có migration. Chạy: dotnet ef migrations add Initial -p src/SmartDrone.Infrastructure -s src/SmartDrone.Api -o Persistence/Migrations");
            return;
        }

        await db.Database.MigrateAsync();

        var roleManager = sp.GetRequiredService<RoleManager<AppRole>>();
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new AppRole { Name = role });
        }

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

        // Seed 20 tuyến bay có hướng giữa 5 trạm
        var stations = await db.Stations
            .OrderBy(s => s.Name)
            .ToListAsync();

        if (stations.Count == 5)
        {
            // Lấy các tuyến đã tồn tại để tránh tạo trùng
            var existingRoutes = await db.StationRoutes
                .Select(r => new { r.FromStationId, r.ToStationId })
                .ToListAsync();

            var existingPairs = existingRoutes
                .Select(r => (r.FromStationId, r.ToStationId))
                .ToHashSet();

            for (int i = 0; i < stations.Count; i++)
            {
                for (int j = 0; j < stations.Count; j++)
                {
                    // Không tạo tuyến từ một trạm đến chính nó
                    if (i == j)
                        continue;

                    var fromId = stations[i].Id;
                    var toId = stations[j].Id;

                    // Không tạo lại tuyến đã tồn tại
                    if (existingPairs.Contains((fromId, toId)))
                        continue;

                    db.StationRoutes.Add(new StationRoute
                    {
                        FromStationId = fromId,
                        ToStationId = toId,
                        DistanceKm = 1.0m,
                        IsActive = true
                    });
                }
            }

            await db.SaveChangesAsync();
        }
        else
        {
            logger.LogWarning(
                "Không seed tuyến bay vì cần đúng 5 trạm, hiện có {Count} trạm.",
                stations.Count);
        }

        // Seed 3 drone mẫu, tránh tạo trùng khi chạy nhiều lần
        if (stations.Count == 5)
        {
            var droneSamples = new[]
            {
                new { Code = "DRONE-001", Model = "DJI Matrice 350 RTK", StationId = stations[0].Id },
                new { Code = "DRONE-002", Model = "DJI Matrice 350 RTK", StationId = stations[1].Id },
                new { Code = "DRONE-003", Model = "DJI Matrice 350 RTK", StationId = stations[2].Id }
            };

            var existingCodes = await db.Drones
                .Select(d => d.DroneCode)
                .ToListAsync();

            foreach (var sample in droneSamples)
            {
                if (existingCodes.Contains(sample.Code))
                    continue;

                db.Drones.Add(new Drone
                {
                    DroneCode = sample.Code,
                    Model = sample.Model,
                    MaxPayloadKg = 2.50m,
                    MaxRangeKm = 15.00m,
                    CruiseSpeedKmh = 40.0m,
                    BatteryPct = 100,
                    Status = "Idle",
                    CurrentStationId = sample.StationId,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        }
        else
        {
            logger.LogWarning(
                "Không seed drone vì cần đúng 5 trạm, hiện có {Count} trạm.",
                stations.Count);
        }

        if (bool.TryParse(config["Seed:DemoUsers"], out var seedDemo) && seedDemo)
        {
            var password = config["Seed:DemoPassword"];
            if (string.IsNullOrWhiteSpace(password)) return;

            var users = sp.GetRequiredService<UserManager<AppUser>>();
            foreach (var role in Roles.All)
            {
                var email = $"{role.ToLowerInvariant()}@drone.local";
                if (await users.FindByEmailAsync(email) is not null) continue;

                var user = new AppUser { UserName = email, Email = email, FullName = $"Demo {role}", EmailConfirmed = true };
                if ((await users.CreateAsync(user, password)).Succeeded)
                    await users.AddToRoleAsync(user, role);
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
