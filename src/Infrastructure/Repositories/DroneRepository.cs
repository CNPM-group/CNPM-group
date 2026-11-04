using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class DroneRepository(AppDbContext db) : IDroneRepository
{
    public async Task<IReadOnlyList<Drone>> ListAsync(CancellationToken ct = default) =>
        await db.Drones.AsNoTracking().OrderBy(d => d.DroneCode).ToListAsync(ct);

    public Task<Drone?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Drones.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<bool> CodeExistsAsync(string droneCode, Guid? exceptId, CancellationToken ct = default) =>
        db.Drones.AnyAsync(d => d.DroneCode == droneCode && d.Id != exceptId, ct);

    public Task<bool> AnyCanCarryAsync(decimal weightKg, CancellationToken ct = default) =>
        db.Drones.AnyAsync(d => d.MaxPayloadKg >= weightKg, ct);

    public async Task AddAsync(Drone drone, CancellationToken ct = default)
    {
        db.Drones.Add(drone);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Drone drone, CancellationToken ct = default)
    {
        if (db.Entry(drone).State == EntityState.Detached)
            db.Drones.Update(drone);

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Drone drone, CancellationToken ct = default)
    {
        db.Drones.Remove(drone);
        await db.SaveChangesAsync(ct);
    }
}
