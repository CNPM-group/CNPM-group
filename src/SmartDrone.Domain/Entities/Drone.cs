
namespace SmartDrone.Domain.Entities;

public class Drone
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DroneCode { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public decimal MaxPayloadKg { get; set; }

    public decimal MaxRangeKm { get; set; }

    public decimal CruiseSpeedKmh { get; set; }

    public short BatteryPct { get; set; } = 100;

    public string Status { get; set; } = "Idle";

    public Guid? CurrentStationId { get; set; }

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
