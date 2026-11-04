using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record DroneRequest(
    [Required, MaxLength(30)] string DroneCode,
    [Required, MaxLength(100)] string Model,
    [Range(0.1, 50)] decimal MaxPayloadKg,
    [Range(0.1, 500)] decimal MaxRangeKm,
    [Range(1, 200)] decimal CruiseSpeedKmh,
    [Range(0, 100)] short BatteryPct = 100,
    DroneStatus Status = DroneStatus.Idle,
    Guid? CurrentStationId = null)
{
    public Drone ToDomain() => new()
    {
        DroneCode = DroneCode,
        Model = Model,
        MaxPayloadKg = MaxPayloadKg,
        MaxRangeKm = MaxRangeKm,
        CruiseSpeedKmh = CruiseSpeedKmh,
        BatteryPct = BatteryPct,
        Status = Status.ToString(),
        CurrentStationId = CurrentStationId
    };
}

public sealed record DroneResponse(
    Guid Id,
    string DroneCode,
    string Model,
    decimal MaxPayloadKg,
    decimal MaxRangeKm,
    decimal CruiseSpeedKmh,
    short BatteryPct,
    string Status,
    Guid? CurrentStationId,
    DateTime CreatedAtUtc)
{
    public static DroneResponse From(Drone d) =>
        new(d.Id, d.DroneCode, d.Model, d.MaxPayloadKg, d.MaxRangeKm, d.CruiseSpeedKmh, d.BatteryPct, d.Status, d.CurrentStationId, d.CreatedAtUtc);
}
