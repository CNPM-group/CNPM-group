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

    public async Task SavePositionAsync(Guid flightId, decimal latitude, decimal longitude, DateTime recordedAtUtc, CancellationToken ct = default)
    {
        await db.DroneFlights
            .Where(f => f.Id == flightId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(f => f.CurrentLatitude, (decimal?)latitude)
                .SetProperty(f => f.CurrentLongitude, (decimal?)longitude), ct);

        db.FlightTelemetries.Add(new FlightTelemetry
        {
            FlightId = flightId,
            Latitude = latitude,
            Longitude = longitude,
            RecordedAtUtc = recordedAtUtc
        });
        await db.SaveChangesAsync(ct);
    }

    public void AddFlight(DroneFlight flight) => db.DroneFlights.Add(flight);

    public void AddConfirmation(DeliveryConfirmation confirmation) => db.DeliveryConfirmations.Add(confirmation);
}
