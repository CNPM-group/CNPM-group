using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class DroneService(IDroneRepository drones, IStationRepository stations)
{
    public Task<IReadOnlyList<Drone>> ListAsync(CancellationToken ct = default) =>
        drones.ListAsync(ct);

    public async Task<Drone> GetAsync(Guid id, CancellationToken ct = default) =>
        await drones.GetByIdAsync(id, ct) ?? throw new NotFoundException("Không tìm thấy drone.");

    public async Task<Drone> CreateAsync(Drone input, CancellationToken ct = default)
    {
        await ValidateAsync(input, null, ct);

        var drone = new Drone();
        Apply(drone, input);

        await drones.AddAsync(drone, ct);
        return drone;
    }

    public async Task<Drone> UpdateAsync(Guid id, Drone input, CancellationToken ct = default)
    {
        var drone = await GetAsync(id, ct);
        await ValidateAsync(input, id, ct);

        Apply(drone, input);
        drone.UpdatedAtUtc = DateTime.UtcNow;

        await drones.UpdateAsync(drone, ct);
        return drone;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var drone = await GetAsync(id, ct);
        if (drone.Status is nameof(DroneStatus.Assigned) or nameof(DroneStatus.InFlight))
            throw new ConflictException("Drone đang được gán hoặc đang bay, không xóa được.");

        await drones.DeleteAsync(drone, ct);
    }

    private async Task ValidateAsync(Drone input, Guid? exceptId, CancellationToken ct)
    {
        if (await drones.CodeExistsAsync(input.DroneCode.Trim(), exceptId, ct))
            throw new ConflictException($"Mã drone {input.DroneCode.Trim()} đã tồn tại.");

        if (input.CurrentStationId is { } stationId && !await stations.ExistsAsync(stationId, ct))
            throw new ValidationException("Trạm hiện tại của drone không tồn tại.");
    }

    private static void Apply(Drone target, Drone input)
    {
        target.DroneCode = input.DroneCode.Trim();
        target.Model = input.Model.Trim();
        target.MaxPayloadKg = input.MaxPayloadKg;
        target.MaxRangeKm = input.MaxRangeKm;
        target.CruiseSpeedKmh = input.CruiseSpeedKmh;
        target.BatteryPct = input.BatteryPct;
        target.Status = input.Status;
        target.CurrentStationId = input.CurrentStationId;
    }
}
