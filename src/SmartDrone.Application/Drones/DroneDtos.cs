
using SmartDrone.Domain.Enums;

namespace SmartDrone.Application.Drones;

public sealed record DroneDto(
    Guid Id,
    string DroneCode,
    string Model,
    decimal MaxPayloadKg,
    decimal MaxRangeKm,
    decimal CruiseSpeedKmh,
    short BatteryPct,
    DroneStatus Status,
    Guid? CurrentStationId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
