
namespace SmartDrone.Domain.Entities;

public class StationRoute
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FromStationId { get; set; }

    public Guid ToStationId { get; set; }

    public decimal DistanceKm { get; set; }

    public bool IsActive { get; set; } = true;
}
