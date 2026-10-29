
using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Domain.Entities;
using SmartDrone.Domain.Enums;

namespace SmartDrone.Application.Orders;



public interface IOrderService
{
    Task<OrderDto> CreateAsync(
        Guid customerId,
        CreateOrderRequest request,
        CancellationToken ct = default);

    Task<OrderDto?> CancelAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken ct = default);

    Task<OrderDto?> UpdateAsync(
        Guid customerId,
        Guid orderId,
        UpdateOrderRequest request,
        CancellationToken ct = default);
}



public sealed class OrderService(IAppDbContext db) : IOrderService
{
    public async Task<OrderDto> CreateAsync(
        Guid customerId,
        CreateOrderRequest request,
        CancellationToken ct = default)
    {
        if (request.OriginStationId == request.DestinationStationId)
            throw new ArgumentException("Trạm gửi và trạm nhận phải khác nhau.");

        var originExists = await db.Stations
            .AnyAsync(s => s.Id == request.OriginStationId, ct);

        var destinationExists = await db.Stations
            .AnyAsync(s => s.Id == request.DestinationStationId, ct);

        if (!originExists || !destinationExists)
            throw new ArgumentException("Trạm gửi hoặc trạm nhận không tồn tại.");


        if (request.DeliveryAddressId.HasValue)
        {
            var addressExists = await db.CustomerAddresses
                .AnyAsync(a =>
                    a.Id == request.DeliveryAddressId.Value &&
                    a.CustomerId == customerId, ct);

            if (!addressExists)
                throw new ArgumentException(
                    "Địa chỉ giao hàng không tồn tại hoặc không thuộc về bạn.");
        }


        if (request.Package is null)
            throw new ArgumentException("Thông tin kiện hàng là bắt buộc.");


        var canCarryPackage = await db.Drones
            .AnyAsync(d =>
                d.MaxPayloadKg >= request.Package.WeightKg, ct);

        if (!canCarryPackage)
        {
            throw new ArgumentException(
                "Kiện hàng vượt quá tải trọng tối đa của đội drone.");
        }


        var package = new Package
        {
            Description = request.Package.Description,
            WeightKg = request.Package.WeightKg,
            LengthCm = request.Package.LengthCm,
            WidthCm = request.Package.WidthCm,
            HeightCm = request.Package.HeightCm,
            ImageKey = request.Package.ImageKey,
            CreatedAtUtc = DateTime.UtcNow
        };


        db.Packages.Add(package);

        var now = DateTime.UtcNow;
        var order = new DeliveryOrder
        {
            TrackingCode = $"SDD-{Guid.NewGuid():N}"[..20],
            CustomerId = customerId,
            PackageId = package.Id,
            OriginStationId = request.OriginStationId,
            DestinationStationId = request.DestinationStationId,
            DeliveryAddressId = request.DeliveryAddressId,
            Status = OrderStatus.Pending.ToString(),
            Notes = request.Notes,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };


        db.DeliveryOrders.Add(order);

        db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = null,
            ToStatus = OrderStatus.Pending.ToString(),
            ChangedBy = customerId,
            Note = "Khách hàng tạo đơn hàng.",
            ChangedAtUtc = now
        });

        await db.SaveChangesAsync(ct);


        return new OrderDto(
            order.Id,
            order.TrackingCode,
            order.CustomerId,
            order.PackageId,
            order.OriginStationId,
            order.DestinationStationId,
            order.Status,
            order.CreatedAtUtc);
    }

    public async Task<OrderDto?> CancelAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken ct = default)
    {
        var order = await db.DeliveryOrders
            .FirstOrDefaultAsync(o =>
                o.Id == orderId &&
                o.CustomerId == customerId, ct);

        if (order is null)
            return null;

        if (order.Status != OrderStatus.Pending.ToString())
            throw new InvalidOperationException(
                "Chỉ được hủy đơn hàng đang ở trạng thái Pending.");

        var now = DateTime.UtcNow;
        var previousStatus = order.Status;

        order.Status = OrderStatus.Cancelled.ToString();
        order.UpdatedAtUtc = now;

        db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = previousStatus,
            ToStatus = OrderStatus.Cancelled.ToString(),
            ChangedBy = customerId,
            Note = "Khách hàng hủy đơn hàng.",
            ChangedAtUtc = now
        });

        await db.SaveChangesAsync(ct);

        return new OrderDto(
            order.Id,
            order.TrackingCode,
            order.CustomerId,
            order.PackageId,
            order.OriginStationId,
            order.DestinationStationId,
            order.Status,
            order.CreatedAtUtc);
    }

    public async Task<OrderDto?> UpdateAsync(
        Guid customerId,
        Guid orderId,
        UpdateOrderRequest request,
        CancellationToken ct = default)
    {
        // 1. Tìm đơn hàng thuộc Customer
        var order = await db.DeliveryOrders
            .FirstOrDefaultAsync(o =>
                o.Id == orderId &&
                o.CustomerId == customerId, ct);

        if (order is null)
            return null;

        // 2. Chỉ được sửa khi đơn đang Pending
        if (order.Status != OrderStatus.Pending.ToString())
            throw new InvalidOperationException(
                "Chỉ được sửa đơn hàng đang ở trạng thái Pending.");

        // 3. Kiểm tra trạm gửi và trạm nhận
        if (request.OriginStationId == request.DestinationStationId)
            throw new ArgumentException(
                "Trạm gửi và trạm nhận phải khác nhau.");

        var originExists = await db.Stations
            .AnyAsync(s => s.Id == request.OriginStationId, ct);

        var destinationExists = await db.Stations
            .AnyAsync(s => s.Id == request.DestinationStationId, ct);

        if (!originExists || !destinationExists)
            throw new ArgumentException(
                "Trạm gửi hoặc trạm nhận không tồn tại.");

        // 4. Kiểm tra địa chỉ thuộc Customer
        if (request.DeliveryAddressId.HasValue)
        {
            var addressExists = await db.CustomerAddresses
                .AnyAsync(a =>
                    a.Id == request.DeliveryAddressId.Value &&
                    a.CustomerId == customerId, ct);

            if (!addressExists)
                throw new ArgumentException(
                    "Địa chỉ giao hàng không tồn tại hoặc không thuộc về bạn.");
        }

        // 5. Kiểm tra kiện hàng
        if (request.Package is null)
            throw new ArgumentException(
                "Thông tin kiện hàng là bắt buộc.");

        // 6. Kiểm tra tải trọng Drone
        var canCarryPackage = await db.Drones
            .AnyAsync(d =>
                d.MaxPayloadKg >= request.Package.WeightKg, ct);

        if (!canCarryPackage)
            throw new ArgumentException(
                "Kiện hàng vượt quá tải trọng tối đa của đội drone.");

        // 7. Lấy kiện hàng hiện tại
        var package = await db.Packages
            .FirstOrDefaultAsync(p => p.Id == order.PackageId, ct);

        if (package is null)
            throw new InvalidOperationException(
                "Không tìm thấy kiện hàng của đơn.");

        // 8. Cập nhật kiện hàng
        package.Description = request.Package.Description;
        package.WeightKg = request.Package.WeightKg;
        package.LengthCm = request.Package.LengthCm;
        package.WidthCm = request.Package.WidthCm;
        package.HeightCm = request.Package.HeightCm;
        package.ImageKey = request.Package.ImageKey;

        // 9. Cập nhật đơn hàng
        order.OriginStationId = request.OriginStationId;
        order.DestinationStationId = request.DestinationStationId;
        order.DeliveryAddressId = request.DeliveryAddressId;
        order.Notes = request.Notes;
        order.UpdatedAtUtc = DateTime.UtcNow;

        // 10. Lưu thay đổi vào PostgreSQL
        await db.SaveChangesAsync(ct);

        return new OrderDto(
            order.Id,
            order.TrackingCode,
            order.CustomerId,
            order.PackageId,
            order.OriginStationId,
            order.DestinationStationId,
            order.Status,
            order.CreatedAtUtc);
    }
}
