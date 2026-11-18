using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class OrderService(
    IOrderRepository orders,
    IStationRepository stations,
    ICustomerAddressRepository addresses,
    IDroneRepository drones,
    IUserRepository users,
    IOrderEvents events)
{

    public async Task<DeliveryOrder> CreateAsync(Guid customerId, DeliveryOrder input, Package package, CancellationToken ct = default)
    {
        await ValidateAsync(customerId, input, package, ct);
        var (recipientName, recipientPhone) = await RecipientAsync(customerId, input, ct);

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
            RecipientName = recipientName,
            RecipientPhone = recipientPhone,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var history = NewHistory(order.Id, null, OrderStatus.Pending, customerId, "Khách hàng tạo đơn hàng.", now);

        await orders.AddAsync(order, package, history, ct);
        await events.StatusChangedAsync(order, ct);
        return order;
    }

    public async Task<DeliveryOrder> UpdateAsync(Guid customerId, Guid orderId, DeliveryOrder input, Package package, CancellationToken ct = default)
    {
        var order = await GetOwnedPendingAsync(customerId, orderId, "Chỉ được sửa đơn hàng đang ở trạng thái Pending.", ct);

        await ValidateAsync(customerId, input, package, ct);
        var (recipientName, recipientPhone) = await RecipientAsync(customerId, input, ct);

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
        order.RecipientName = recipientName;
        order.RecipientPhone = recipientPhone;
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
        await events.StatusChangedAsync(order, ct);
        return order;
    }

    public async Task<DeliveryOrder> ApproveAsync(Guid staffId, Guid orderId, string? note, CancellationToken ct = default)
    {
        var order = await GetAsync(null, orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.Approved);

        var now = DateTime.UtcNow;
        var history = NewHistory(order.Id, order.Status, OrderStatus.Approved, staffId,
            string.IsNullOrWhiteSpace(note) ? "Đơn đã được duyệt." : note.Trim(), now);

        order.Status = nameof(OrderStatus.Approved);
        order.ApprovedBy = staffId;
        order.ApprovedAtUtc = now;
        order.UpdatedAtUtc = now;

        await orders.UpdateAsync(order, history, ct);
        await events.StatusChangedAsync(order, ct);
        return order;
    }

    public async Task<DeliveryOrder> RejectAsync(Guid staffId, Guid orderId, string reason, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ValidationException("Phải nhập lý do từ chối.");

        var order = await GetAsync(null, orderId, ct);
        OrderStateMachine.EnsureTransition(order.Status, OrderStatus.Rejected);

        var now = DateTime.UtcNow;
        var history = NewHistory(order.Id, order.Status, OrderStatus.Rejected, staffId, reason.Trim(), now);

        order.Status = nameof(OrderStatus.Rejected);
        order.UpdatedAtUtc = now;

        await orders.UpdateAsync(order, history, ct);
        await events.StatusChangedAsync(order, ct);
        return order;
    }

    public Task<PagedResult<OrderSummary>> PageAsync(Guid? customerScope, string? status, string? search, int page, int pageSize, CancellationToken ct = default)
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

        if (search is { Length: > 50 })
            throw new ValidationException("Từ khóa tìm kiếm tối đa 50 ký tự.");

        return orders.PageAsync(customerScope, statusValue, search, page, pageSize, ct);
    }

    public async Task<DeliveryOrder> GetAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default) =>
        await orders.GetAsync(orderId, customerScope, ct) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");

    public async Task<OrderDetail> GetDetailAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default)
    {
        var order = await GetAsync(customerScope, orderId, ct);
        var package = await orders.GetPackageAsync(order.PackageId, ct)
            ?? throw new NotFoundException("Không tìm thấy kiện hàng của đơn.");
        var origin = await stations.GetByIdAsync(order.OriginStationId, ct)
            ?? throw new NotFoundException("Không tìm thấy trạm gửi.");
        var destination = await stations.GetByIdAsync(order.DestinationStationId, ct)
            ?? throw new NotFoundException("Không tìm thấy trạm nhận.");
        return new OrderDetail(order, package, origin, destination);
    }

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

        var origin = await stations.GetByIdAsync(input.OriginStationId, ct);
        var destination = await stations.GetByIdAsync(input.DestinationStationId, ct);
        if (origin is null || destination is null)
            throw new ValidationException("Trạm gửi hoặc trạm nhận không tồn tại.");

        foreach (var s in new[] { origin, destination })
            if (s.Status == StationStatus.Maintenance)
                throw new ValidationException($"{s.Name} đang bảo trì, chọn trạm khác.");

        if (input.DeliveryAddressId is { } addressId && !await addresses.BelongsToAsync(addressId, customerId, ct))
            throw new ValidationException("Địa chỉ giao hàng không tồn tại hoặc không thuộc về bạn.");

        if (!await drones.AnyCanCarryAsync(package.WeightKg, ct))
            throw new ValidationException("Kiện hàng vượt quá tải trọng tối đa của đội drone.");
    }

    // Quy tắc nghiệp vụ số 2: đơn phải có người nhận. Khách bỏ trống thì người nhận là chính khách.
    private async Task<(string Name, string? Phone)> RecipientAsync(Guid customerId, DeliveryOrder input, CancellationToken ct)
    {
        var name = input.RecipientName?.Trim();
        var phone = ProfileRules.Phone(input.RecipientPhone);
        if (name is { Length: > 100 })
            throw new ValidationException("Tên người nhận tối đa 100 ký tự.");

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
        {
            var customer = await users.GetByIdAsync(customerId);
            if (string.IsNullOrEmpty(name)) name = customer?.FullName;
            if (string.IsNullOrEmpty(phone)) phone = customer?.Phone;
        }

        return (string.IsNullOrWhiteSpace(name) ? "Khách hàng" : name, string.IsNullOrWhiteSpace(phone) ? null : phone);
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
