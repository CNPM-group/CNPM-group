
namespace SmartDrone.Domain.Entities;

public class DroneFlight
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrderId { get; set; }

    public Guid DroneId { get; set; }

    public short LegIndex { get; set; } = 1;

    public Guid DepartureStationId { get; set; }

    public Guid ArrivalStationId { get; set; }

    public DateTime? ScheduledDepartureAtUtc { get; set; }

    public DateTime? ActualDepartureAtUtc { get; set; }

    public DateTime? ActualArrivalAtUtc { get; set; }

    public decimal? CurrentLatitude { get; set; }

    public decimal? CurrentLongitude { get; set; }

    public string FlightStatus { get; set; } = "Scheduled";

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
