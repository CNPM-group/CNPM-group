using Microsoft.Extensions.Options;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class TrackingService(
    IOrderRepository orders,
    IDeliveryRepository deliveries,
    IStationRepository stations,
    IDroneRepository drones,
    IOptions<SimulatorOptions> options)
{
    public async Task<TrackingSnapshot> GetAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default)
    {
        var order = await orders.GetAsync(orderId, customerScope, ct)
            ?? throw new NotFoundException("Không tìm thấy đơn hàng.");
        var flight = await deliveries.GetLatestFlightAsync(order.Id, ct);
        return await BuildAsync(order, flight, DateTime.UtcNow, ct);
    }

    // Một lượt giả lập: tính vị trí mới cho mọi drone đang bay, lưu vào chuyến bay + telemetry.
    public async Task<IReadOnlyList<TrackingSnapshot>> MoveFlyingDronesAsync(CancellationToken ct = default)
    {
        var page = await orders.PageAsync(null, nameof(OrderStatus.InTransit), null, 1, 100, ct);
        var now = DateTime.UtcNow;
        var result = new List<TrackingSnapshot>();

        foreach (var item in page.Items)
        {
            var order = await orders.GetAsync(item.Id, null, ct);
            var flight = order is null ? null : await deliveries.GetLatestFlightAsync(order.Id, ct);
            if (order is null || flight is null || flight.FlightStatus != FlightStatus.Flying)
                continue;

            var snapshot = await BuildAsync(order, flight, now, ct);
            await deliveries.SavePositionAsync(flight.Id, (decimal)snapshot.Latitude, (decimal)snapshot.Longitude, now, ct);
            result.Add(snapshot);
        }

        return result;
    }

    public static double Progress(string status, DateTime? departedAt, DateTime? eta, DateTime now, double timeScale = 1)
    {
        if (status is nameof(OrderStatus.ArrivedAtStation) or nameof(OrderStatus.Completed))
            return 1;
        if (status != nameof(OrderStatus.InTransit) || departedAt is null || eta is null)
            return 0;

        var total = (eta.Value - departedAt.Value).TotalSeconds;
        if (total <= 0)
            return 1;
        var flown = (now - departedAt.Value).TotalSeconds * Math.Max(timeScale, 0.1);
        return Math.Clamp(flown / total, 0, 1);
    }

    private async Task<TrackingSnapshot> BuildAsync(DeliveryOrder order, DroneFlight? flight, DateTime now, CancellationToken ct)
    {
        var origin = await stations.GetByIdAsync(order.OriginStationId, ct)
            ?? throw new NotFoundException("Không tìm thấy trạm gửi.");
        var destination = await stations.GetByIdAsync(order.DestinationStationId, ct)
            ?? throw new NotFoundException("Không tìm thấy trạm nhận.");
        var drone = flight is null ? null : await drones.GetByIdAsync(flight.DroneId, ct);

        var progress = Progress(order.Status, flight?.ActualDepartureAtUtc, order.EtaUtc, now, options.Value.TimeScale);
        double oLat = (double)origin.Latitude, oLng = (double)origin.Longitude;
        double dLat = (double)destination.Latitude, dLng = (double)destination.Longitude;

        return new TrackingSnapshot(
            order.Id, order.CustomerId, order.TrackingCode, order.Status, drone?.DroneCode,
            Lerp(oLat, dLat, progress),
            Lerp(oLng, dLng, progress),
            Math.Round(progress * 100, 1),
            origin.Name, oLat, oLng,
            destination.Name, dLat, dLng,
            flight?.ActualDepartureAtUtc, order.EtaUtc);
    }

    // Đầu và cuối trả đúng tọa độ trạm: tính bằng double thì a + (b − a) × 1 có thể lệch ở chữ số cuối.
    private static double Lerp(double from, double to, double t) =>
        t <= 0 ? from : t >= 1 ? to : from + (to - from) * t;
}
