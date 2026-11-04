using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class DeliveryRepository(AppDbContext db) : IDeliveryRepository
{
    public Task<int> CountFlightsAsync(Guid orderId, CancellationToken ct = default) =>
        db.DroneFlights.CountAsync(f => f.OrderId == orderId, ct);

    public Task<DroneFlight?> GetLatestFlightAsync(Guid orderId, CancellationToken ct = default) =>
        db.DroneFlights
            .Where(f => f.OrderId == orderId)
            .OrderByDescending(f => f.LegIndex)
            .FirstOrDefaultAsync(ct);

    public Task<DeliveryConfirmation?> GetConfirmationAsync(Guid orderId, CancellationToken ct = default) =>
        db.DeliveryConfirmations.FirstOrDefaultAsync(c => c.OrderId == orderId, ct);

    public void AddFlight(DroneFlight flight) => db.DroneFlights.Add(flight);

    public void AddConfirmation(DeliveryConfirmation confirmation) => db.DeliveryConfirmations.Add(confirmation);
}
