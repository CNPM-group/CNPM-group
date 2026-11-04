namespace SmartDrone.Domain.Models;

public interface IDeliveryRepository
{
    Task<int> CountFlightsAsync(Guid orderId, CancellationToken ct = default);
    Task<DroneFlight?> GetLatestFlightAsync(Guid orderId, CancellationToken ct = default);
    Task<DeliveryConfirmation?> GetConfirmationAsync(Guid orderId, CancellationToken ct = default);
    void AddFlight(DroneFlight flight);
    void AddConfirmation(DeliveryConfirmation confirmation);
}
