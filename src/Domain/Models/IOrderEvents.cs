namespace SmartDrone.Domain.Models;

public interface IOrderEvents
{
    Task StatusChangedAsync(DeliveryOrder order, CancellationToken ct = default);
    Task DroneMovedAsync(TrackingSnapshot snapshot, CancellationToken ct = default);
}
