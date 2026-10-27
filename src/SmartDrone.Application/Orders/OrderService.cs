
using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Application.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(
        Guid customerId,
        CreateOrderRequest request,
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

        var package = new Package
        {
            Description = request.Description,
            WeightKg = request.WeightKg,
            LengthCm = request.LengthCm,
            WidthCm = request.WidthCm,
            HeightCm = request.HeightCm,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Packages.Add(package);

        var now = DateTime.UtcNow;
        var order = new DeliveryOrder
        {
            TrackingCode = $"SDD-{now:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..29],
            CustomerId = customerId,
            PackageId = package.Id,
            OriginStationId = request.OriginStationId,
            DestinationStationId = request.DestinationStationId,
            Status = "Pending",
            Notes = request.Notes,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        db.DeliveryOrders.Add(order);
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
