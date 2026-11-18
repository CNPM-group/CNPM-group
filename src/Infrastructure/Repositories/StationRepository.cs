using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain;
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
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Khóa ngoại chặn xóa (còn đơn / chuyến bay trỏ tới) → báo 409 thay vì lỗi 500.
            db.Entry(station).State = EntityState.Unchanged;
            throw new ConflictException("Trạm đã có đơn hàng hoặc drone dùng tới, không xóa được. Chuyển trạm sang Bảo trì thay vì xóa.");
        }
    }

    public async Task<IReadOnlyList<StationLoad>> LoadAsync(CancellationToken ct = default)
    {
        var stationIds = await db.Stations.AsNoTracking().Select(s => s.Id).ToListAsync(ct);

        var incoming = await db.DeliveryOrders.AsNoTracking()
            .Where(o => o.Status == "Scheduled" || o.Status == "InTransit")
            .GroupBy(o => o.DestinationStationId)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, ct);

        var waiting = await db.DeliveryOrders.AsNoTracking()
            .Where(o => o.Status == "ArrivedAtStation")
            .GroupBy(o => o.DestinationStationId)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, ct);

        var outgoing = await db.DeliveryOrders.AsNoTracking()
            .Where(o => o.Status == "Pending" || o.Status == "Approved" || o.Status == "Scheduled")
            .GroupBy(o => o.OriginStationId)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, ct);

        return stationIds
            .Select(id => new StationLoad(id, incoming.GetValueOrDefault(id), waiting.GetValueOrDefault(id), outgoing.GetValueOrDefault(id)))
            .ToList();
    }
}
