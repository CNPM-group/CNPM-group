
namespace SmartDrone.Domain.Entities;

public class FlightTelemetry
{
    public long Id { get; set; }

    public Guid FlightId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public decimal? AltitudeM { get; set; }

    public short? BatteryPct { get; set; }

    public DateTime RecordedAtUtc { get; set; } =
        DateTime.UtcNow;
}
