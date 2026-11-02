using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class DroneRepository(AppDbContext db) : IDroneRepository
{
    public Task<bool> AnyCanCarryAsync(decimal weightKg, CancellationToken ct = default) =>
        db.Drones.AnyAsync(d => d.MaxPayloadKg >= weightKg, ct);
}
