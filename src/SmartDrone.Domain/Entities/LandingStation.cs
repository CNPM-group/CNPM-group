using SmartDrone.Domain.Enums;

namespace SmartDrone.Domain.Entities;

/// <summary>Trạm đáp. Cũng là "đỉnh" của đồ thị nếu sau này làm Dijkstra.</summary>
public class LandingStation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public StationStatus Status { get; set; } = StationStatus.Available;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
