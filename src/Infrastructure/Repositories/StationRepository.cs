using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class StationRepository(AppDbContext db) : IStationRepository
{
    public async Task<IReadOnlyList<LandingStation>> ListAsync(CancellationToken ct = default) =>
        await db.Stations.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);

    public Task<LandingStation?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Stations.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        db.Stations.AnyAsync(s => s.Id == id, ct);

    public async Task AddAsync(LandingStation station, CancellationToken ct = default)
    {
        db.Stations.Add(station);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(LandingStation station, CancellationToken ct = default)
    {
        if (db.Entry(station).State == EntityState.Detached)
            db.Stations.Update(station);

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(LandingStation station, CancellationToken ct = default)
    {
        db.Stations.Remove(station);
        await db.SaveChangesAsync(ct);
    }
}
