using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class StationService(IStationRepository stations)
{
    public Task<IReadOnlyList<LandingStation>> ListAsync(CancellationToken ct = default) =>
        stations.ListAsync(ct);

    public async Task<LandingStation> GetAsync(Guid id, CancellationToken ct = default) =>
        await stations.GetByIdAsync(id, ct) ?? throw new NotFoundException("Không tìm thấy trạm.");

    public async Task<LandingStation> CreateAsync(string name, decimal latitude, decimal longitude, StationStatus status, CancellationToken ct = default)
    {
        var station = new LandingStation
        {
            Name = name.Trim(),
            Latitude = latitude,
            Longitude = longitude,
            Status = status
        };

        await stations.AddAsync(station, ct);
        return station;
    }

    public async Task<LandingStation> UpdateAsync(Guid id, string name, decimal latitude, decimal longitude, StationStatus status, CancellationToken ct = default)
    {
        var station = await GetAsync(id, ct);
        station.Name = name.Trim();
        station.Latitude = latitude;
        station.Longitude = longitude;
        station.Status = status;

        await stations.UpdateAsync(station, ct);
        return station;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var station = await GetAsync(id, ct);
        await stations.DeleteAsync(station, ct);
    }
}
