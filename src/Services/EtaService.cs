using Microsoft.Extensions.Options;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class EtaService(IStationRepository stations, IOptions<EtaOptions> options)
{
    private readonly EtaOptions _opt = options.Value;

    public async Task<EtaEstimate> EstimateAsync(Guid originStationId, Guid destinationStationId, decimal weightKg, CancellationToken ct = default)
    {
        if (originStationId == Guid.Empty || destinationStationId == Guid.Empty)
            throw new ValidationException("ID trạm không hợp lệ.");

        if (weightKg is < 0.01m or > 50m)
            throw new ValidationException("Khối lượng phải từ 0.01 đến 50 kg.");

        if (_opt.DroneSpeedKmPerHour <= 0 || _opt.WeightMinutesPerKg < 0)
            throw new InvalidOperationException("Cấu hình ETA không hợp lệ.");

        var origin = await stations.GetByIdAsync(originStationId, ct);
        var destination = await stations.GetByIdAsync(destinationStationId, ct);
        if (origin is null || destination is null)
            throw new NotFoundException("Không tìm thấy trạm đi hoặc trạm đến.");

        var distanceKm = GeoDistance.HaversineKm(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);
        var minutes = distanceKm / _opt.DroneSpeedKmPerHour * 60 + (double)weightKg * _opt.WeightMinutesPerKg;

        return new EtaEstimate(Math.Round(distanceKm, 2), Math.Round(minutes, 1));
    }
}
