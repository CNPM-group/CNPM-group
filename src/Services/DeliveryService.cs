using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class DeliveryService(
    IOrderRepository orders,
    IDroneRepository drones,
    IDeliveryRepository delivery,
    INotificationRepository notifications,
    EtaService eta)
{
    public async Task<DeliveryOrder> ScheduleAsync(Guid staffId, Guid orderId, Guid droneId, DateTime? scheduledAtUtc, CancellationToken ct = default)
    {
        var order = await GetOrderAsync(orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.Scheduled);

        var drone = await drones.GetByIdAsync(droneId, ct) ?? throw new NotFoundException("Không tìm thấy drone.");
        if (drone.Status != nameof(DroneStatus.Idle))
            throw new ConflictException($"Drone {drone.DroneCode} đang {drone.Status}, chỉ gán được drone Idle.");

        var package = await orders.GetPackageAsync(order.PackageId, ct) ?? throw new NotFoundException("Không tìm thấy kiện hàng của đơn.");
        if (drone.MaxPayloadKg < package.WeightKg)
            throw new ValidationException($"Drone {drone.DroneCode} chở tối đa {drone.MaxPayloadKg} kg, kiện nặng {package.WeightKg} kg.");

        var now = DateTime.UtcNow;
        var departAt = scheduledAtUtc?.ToUniversalTime() ?? now;
        if (departAt < now.AddMinutes(-1))
            throw new ValidationException("Giờ bay phải ở hiện tại hoặc tương lai.");

        var estimate = await eta.EstimateAsync(order.OriginStationId, order.DestinationStationId, package.WeightKg, ct);

        delivery.AddFlight(new DroneFlight
        {
            OrderId = order.Id,
            DroneId = drone.Id,
            LegIndex = (short)(await delivery.CountFlightsAsync(order.Id, ct) + 1),
            DepartureStationId = order.OriginStationId,
            ArrivalStationId = order.DestinationStationId,
            ScheduledDepartureAtUtc = departAt,
            FlightStatus = FlightStatus.Scheduled
        });

        drone.Status = nameof(DroneStatus.Assigned);
        drone.UpdatedAtUtc = now;

        var history = Move(order, OrderStatus.Scheduled, staffId, $"Gán drone {drone.DroneCode}, bay lúc {departAt:HH:mm dd/MM} UTC.", now);
        order.ScheduledAtUtc = departAt;
        order.EtaUtc = departAt.AddMinutes(estimate.EstimatedMinutes);
        order.FailureReason = null;

        await orders.UpdateAsync(order, history, ct);
        return order;
    }

    public async Task<DeliveryOrder> StartAsync(Guid staffId, Guid orderId, CancellationToken ct = default)
    {
        var order = await GetOrderAsync(orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.InTransit);

        var flight = await GetFlightAsync(order.Id, FlightStatus.Scheduled, ct);
        var drone = await drones.GetByIdAsync(flight.DroneId, ct) ?? throw new NotFoundException("Không tìm thấy drone của chuyến bay.");

        var now = DateTime.UtcNow;
        flight.FlightStatus = FlightStatus.Flying;
        flight.ActualDepartureAtUtc = now;

        drone.Status = nameof(DroneStatus.InFlight);
        drone.CurrentStationId = null;
        drone.UpdatedAtUtc = now;

        if (order.ScheduledAtUtc is { } planned && order.EtaUtc is { } eta0)
            order.EtaUtc = now + (eta0 - planned);

        var history = Move(order, OrderStatus.InTransit, staffId, $"Drone {drone.DroneCode} cất cánh.", now);
        Notify(order, "Đơn đang được giao", $"Đơn {order.TrackingCode} đã cất cánh, dự kiến tới trạm lúc {order.EtaUtc:HH:mm dd/MM} UTC.", now);

        await orders.UpdateAsync(order, history, ct);
        return order;
    }

    public async Task<DeliveryOrder> ArriveAsync(Guid staffId, Guid orderId, CancellationToken ct = default)
    {
        var order = await GetOrderAsync(orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.ArrivedAtStation);

        var flight = await GetFlightAsync(order.Id, FlightStatus.Flying, ct);
        var drone = await drones.GetByIdAsync(flight.DroneId, ct) ?? throw new NotFoundException("Không tìm thấy drone của chuyến bay.");

        var now = DateTime.UtcNow;
        flight.FlightStatus = FlightStatus.Landed;
        flight.ActualArrivalAtUtc = now;

        drone.Status = nameof(DroneStatus.Idle);
        drone.CurrentStationId = flight.ArrivalStationId;
        drone.UpdatedAtUtc = now;

        var otp = OtpCode.Generate();
        var confirmation = await delivery.GetConfirmationAsync(order.Id, ct);
        if (confirmation is null)
        {
            confirmation = new DeliveryConfirmation { OrderId = order.Id };
            delivery.AddConfirmation(confirmation);
        }

        confirmation.Method = "OTP";
        confirmation.CodeHash = OtpCode.Hash(order.Id, otp);
        confirmation.ExpiresAtUtc = now + OtpCode.Lifetime;
        confirmation.FailedAttempts = 0;
        confirmation.VerifiedAtUtc = null;
        confirmation.VerifiedBy = null;

        var history = Move(order, OrderStatus.ArrivedAtStation, staffId, "Drone đã hạ cánh, gói hàng nằm tại trạm nhận.", now);
        Notify(order, "Hàng đã tới trạm", $"Đơn {order.TrackingCode} đã tới trạm. Mã nhận hàng: {otp} (hết hạn sau 24 giờ). Đọc mã này cho nhân viên trạm.", now);

        await orders.UpdateAsync(order, history, ct);
        return order;
    }

    public async Task<DeliveryOrder> ConfirmAsync(Guid staffId, Guid orderId, string otp, CancellationToken ct = default)
    {
        var order = await GetOrderAsync(orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.Completed);

        var confirmation = await delivery.GetConfirmationAsync(order.Id, ct)
            ?? throw new ConflictException("Đơn chưa có mã nhận hàng.");

        var now = DateTime.UtcNow;
        if (confirmation.FailedAttempts >= OtpCode.MaxFailedAttempts)
            throw new ConflictException("Nhập sai mã quá số lần cho phép, liên hệ điều phối viên.");
        if (now > confirmation.ExpiresAtUtc)
            throw new ConflictException("Mã nhận hàng đã hết hạn.");

        if (confirmation.CodeHash != OtpCode.Hash(order.Id, otp))
        {
            confirmation.FailedAttempts++;
            await orders.UpdateAsync(order, null, ct);
            throw new ValidationException($"Mã nhận hàng không đúng, còn {OtpCode.MaxFailedAttempts - confirmation.FailedAttempts} lần thử.");
        }

        confirmation.VerifiedAtUtc = now;
        confirmation.VerifiedBy = staffId;

        var history = Move(order, OrderStatus.Completed, staffId, "Khách đã nhận hàng (xác nhận OTP).", now);
        order.CompletedAtUtc = now;
        Notify(order, "Giao hàng thành công", $"Đơn {order.TrackingCode} đã giao xong. Cảm ơn bạn!", now);

        await orders.UpdateAsync(order, history, ct);
        return order;
    }

    public async Task<DeliveryOrder> FailAsync(Guid staffId, Guid orderId, string reason, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ValidationException("Phải nhập lý do giao thất bại.");

        var order = await GetOrderAsync(orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.Failed);

        var now = DateTime.UtcNow;
        var flight = await delivery.GetLatestFlightAsync(order.Id, ct);
        if (flight is not null && flight.FlightStatus == FlightStatus.Flying)
        {
            flight.FlightStatus = FlightStatus.Aborted;
            var drone = await drones.GetByIdAsync(flight.DroneId, ct);
            if (drone is not null)
            {
                drone.Status = nameof(DroneStatus.Idle);
                drone.UpdatedAtUtc = now;
            }
        }

        var history = Move(order, OrderStatus.Failed, staffId, reason.Trim(), now);
        order.FailureReason = reason.Trim();
        Notify(order, "Giao hàng thất bại", $"Đơn {order.TrackingCode} giao không thành công: {reason.Trim()}. Điều phối viên sẽ liên hệ giao lại hoặc hoàn trả.", now);

        await orders.UpdateAsync(order, history, ct);
        return order;
    }

    private async Task<DeliveryOrder> GetOrderAsync(Guid orderId, CancellationToken ct) =>
        await orders.GetAsync(orderId, null, ct) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");

    private async Task<DroneFlight> GetFlightAsync(Guid orderId, string expectedStatus, CancellationToken ct)
    {
        var flight = await delivery.GetLatestFlightAsync(orderId, ct);
        if (flight is null || flight.FlightStatus != expectedStatus)
            throw new ConflictException("Đơn chưa có chuyến bay ở trạng thái phù hợp.");
        return flight;
    }

    private static OrderStatusHistory Move(DeliveryOrder order, OrderStatus to, Guid by, string note, DateTime at)
    {
        var history = new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = order.Status,
            ToStatus = to.ToString(),
            ChangedBy = by,
            Note = note,
            ChangedAtUtc = at
        };
        order.Status = to.ToString();
        order.UpdatedAtUtc = at;
        return history;
    }

    private void Notify(DeliveryOrder order, string title, string body, DateTime at) =>
        notifications.Add(new Notification
        {
            UserId = order.CustomerId,
            OrderId = order.Id,
            Title = title,
            Body = body,
            CreatedAtUtc = at
        });
}
