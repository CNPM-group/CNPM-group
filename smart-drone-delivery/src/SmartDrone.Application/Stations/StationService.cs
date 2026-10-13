using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Application.Stations;

public sealed class StationService(IAppDbContext db) : IStationService
{
    public async Task<IReadOnlyList<StationDto>> GetAllAsync(CancellationToken ct = default) =>
        await db.Stations
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new StationDto(s.Id, s.Name, s.Latitude, s.Longitude, s.Status, s.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<StationDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var s = await db.Stations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return s is null ? null : ToDto(s);
    }

    public async Task<StationDto> CreateAsync(StationRequest request, CancellationToken ct = default)
    {
        var s = new LandingStation
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = request.Status,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Stations.Add(s);
        await db.SaveChangesAsync(ct);
        return ToDto(s);
    }

    public async Task<StationDto?> UpdateAsync(Guid id, StationRequest request, CancellationToken ct = default)
    {
        var s = await db.Stations.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null) return null;

        s.Name = request.Name.Trim();
        s.Latitude = request.Latitude;
        s.Longitude = request.Longitude;
        s.Status = request.Status;
        await db.SaveChangesAsync(ct);
        return ToDto(s);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var s = await db.Stations.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null) return false;

        db.Stations.Remove(s);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static StationDto ToDto(LandingStation s) =>
        new(s.Id, s.Name, s.Latitude, s.Longitude, s.Status, s.CreatedAtUtc);
}
