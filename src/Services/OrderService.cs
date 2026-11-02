using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class OrderService(
    IOrderRepository orders,
    IStationRepository stations,
    ICustomerAddressRepository addresses,
    IDroneRepository drones)
{
    public async Task<DeliveryOrder> CreateAsync(Guid customerId, DeliveryOrder input, Package package, CancellationToken ct = default)
    {
        await ValidateAsync(customerId, input, package, ct);

        var now = DateTime.UtcNow;
        package.CreatedAtUtc = now;

        var order = new DeliveryOrder
        {
            TrackingCode = $"SDD-{Guid.NewGuid():N}"[..20],
            CustomerId = customerId,
            PackageId = package.Id,
            OriginStationId = input.OriginStationId,
            DestinationStationId = input.DestinationStationId,
            DeliveryAddressId = input.DeliveryAddressId,
            Status = nameof(OrderStatus.Pending),
            Notes = input.Notes,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var history = NewHistory(order.Id, null, OrderStatus.Pending, customerId, "Khách hàng tạo đơn hàng.", now);

        await orders.AddAsync(order, package, history, ct);
        return order;
    }

    public async Task<DeliveryOrder> UpdateAsync(Guid customerId, Guid orderId, DeliveryOrder input, Package package, CancellationToken ct = default)
    {
        var order = await GetOwnedPendingAsync(customerId, orderId, "Chỉ được sửa đơn hàng đang ở trạng thái Pending.", ct);

        await ValidateAsync(customerId, input, package, ct);

        var current = await orders.GetPackageAsync(order.PackageId, ct)
            ?? throw new NotFoundException("Không tìm thấy kiện hàng của đơn.");

        current.Description = package.Description;
        current.WeightKg = package.WeightKg;
        current.LengthCm = package.LengthCm;
        current.WidthCm = package.WidthCm;
        current.HeightCm = package.HeightCm;
        current.ImageKey = package.ImageKey;

        order.OriginStationId = input.OriginStationId;
        order.DestinationStationId = input.DestinationStationId;
        order.DeliveryAddressId = input.DeliveryAddressId;
        order.Notes = input.Notes;
        order.UpdatedAtUtc = DateTime.UtcNow;

        await orders.UpdateAsync(order, null, ct);
        return order;
    }

    public async Task<DeliveryOrder> CancelAsync(Guid customerId, Guid orderId, CancellationToken ct = default)
    {
        var order = await GetOwnedPendingAsync(customerId, orderId, "Chỉ được hủy đơn hàng đang ở trạng thái Pending.", ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.Cancelled);

        var now = DateTime.UtcNow;
        var history = NewHistory(order.Id, order.Status, OrderStatus.Cancelled, customerId, "Khách hàng hủy đơn hàng.", now);

        order.Status = nameof(OrderStatus.Cancelled);
        order.UpdatedAtUtc = now;

        await orders.UpdateAsync(order, history, ct);
        return order;
    }

    public Task<PagedResult<OrderSummary>> PageAsync(Guid? customerScope, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        string? statusValue = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsed) || !Enum.IsDefined(parsed))
                throw new ValidationException("Trạng thái đơn hàng không hợp lệ.");
            statusValue = parsed.ToString();
        }

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, ApiConstants.MaxPageSize);

        return orders.PageAsync(customerScope, statusValue, page, pageSize, ct);
    }

    public async Task<DeliveryOrder> GetAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default) =>
        await orders.GetAsync(orderId, customerScope, ct) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");

    public async Task<IReadOnlyList<OrderStatusHistory>> GetHistoryAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default)
    {
        await GetAsync(customerScope, orderId, ct);
        return await orders.GetHistoryAsync(orderId, ct);
    }

    private async Task<DeliveryOrder> GetOwnedPendingAsync(Guid customerId, Guid orderId, string message, CancellationToken ct)
    {
        var order = await GetAsync(customerId, orderId, ct);
        if (order.Status != nameof(OrderStatus.Pending))
            throw new ConflictException(message);
        return order;
    }

    private async Task ValidateAsync(Guid customerId, DeliveryOrder input, Package package, CancellationToken ct)
    {
        if (input.OriginStationId == input.DestinationStationId)
            throw new ValidationException("Trạm gửi và trạm nhận phải khác nhau.");

        if (!await stations.ExistsAsync(input.OriginStationId, ct) || !await stations.ExistsAsync(input.DestinationStationId, ct))
            throw new ValidationException("Trạm gửi hoặc trạm nhận không tồn tại.");

        if (input.DeliveryAddressId is { } addressId && !await addresses.BelongsToAsync(addressId, customerId, ct))
            throw new ValidationException("Địa chỉ giao hàng không tồn tại hoặc không thuộc về bạn.");

        if (!await drones.AnyCanCarryAsync(package.WeightKg, ct))
            throw new ValidationException("Kiện hàng vượt quá tải trọng tối đa của đội drone.");
    }

    private static OrderStatusHistory NewHistory(Guid orderId, string? from, OrderStatus to, Guid by, string note, DateTime at) => new()
    {
        OrderId = orderId,
        FromStatus = from,
        ToStatus = to.ToString(),
        ChangedBy = by,
        Note = note,
        ChangedAtUtc = at
    };
}
