using Microsoft.AspNetCore.SignalR;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Realtime;

public sealed class SignalROrderEvents(IHubContext<DeliveryHub> hub, ILogger<SignalROrderEvents> logger) : IOrderEvents
{
    public const string EventName = "OrderStatusUpdated";
    public const string DroneEventName = "DroneLocation";

    public async Task StatusChangedAsync(DeliveryOrder order, CancellationToken ct = default)
    {
        var payload = new
        {
            orderId = order.Id,
            trackingCode = order.TrackingCode,
            status = order.Status,
            updatedAt = order.UpdatedAtUtc
        };

        try
        {
            await hub.Clients
                .Groups(DeliveryHub.StaffGroup, DeliveryHub.UserGroup(order.CustomerId))
                .SendAsync(EventName, payload, ct);
        }
        catch (Exception ex)
        {
            // Đơn đã lưu xong, gửi realtime lỗi thì chỉ ghi log, không trả lỗi cho người bấm.
            logger.LogWarning(ex, "Không gửi được {Event} cho đơn {OrderId}", EventName, order.Id);
        }
    }

    public async Task DroneMovedAsync(TrackingSnapshot snapshot, CancellationToken ct = default)
    {
        var payload = new
        {
            orderId = snapshot.OrderId,
            trackingCode = snapshot.TrackingCode,
            droneCode = snapshot.DroneCode,
            latitude = snapshot.Latitude,
            longitude = snapshot.Longitude,
            progressPct = snapshot.ProgressPct,
            etaUtc = snapshot.EtaUtc
        };

        try
        {
            await hub.Clients
                .Groups(DeliveryHub.StaffGroup, DeliveryHub.UserGroup(snapshot.CustomerId))
                .SendAsync(DroneEventName, payload, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Không gửi được {Event} cho đơn {OrderId}", DroneEventName, snapshot.OrderId);
        }
    }
}
