using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class OrderRepository(AppDbContext db) : IOrderRepository
{
    public Task<DeliveryOrder?> GetAsync(Guid id, Guid? customerId, CancellationToken ct = default) =>
        db.DeliveryOrders.FirstOrDefaultAsync(
            o => o.Id == id && (customerId == null || o.CustomerId == customerId), ct);

    public Task<Package?> GetPackageAsync(Guid packageId, CancellationToken ct = default) =>
        db.Packages.FirstOrDefaultAsync(p => p.Id == packageId, ct);

    public async Task<PagedResult<OrderSummary>> PageAsync(
        Guid? customerId, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.DeliveryOrders.AsNoTracking();

        if (customerId is not null)
            query = query.Where(o => o.CustomerId == customerId);

        if (status is not null)
            query = query.Where(o => o.Status == status);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .ThenByDescending(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummary(
                o.Id,
                o.TrackingCode,
                o.Status,
                db.Stations.Where(s => s.Id == o.OriginStationId).Select(s => s.Name).FirstOrDefault() ?? "",
                db.Stations.Where(s => s.Id == o.DestinationStationId).Select(s => s.Name).FirstOrDefault() ?? "",
                o.EtaUtc,
                o.CreatedAtUtc))
            .ToListAsync(ct);

        return new PagedResult<OrderSummary>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<OrderStatusHistory>> GetHistoryAsync(Guid orderId, CancellationToken ct = default) =>
        await db.OrderStatusHistories.AsNoTracking()
            .Where(h => h.OrderId == orderId)
            .OrderBy(h => h.ChangedAtUtc)
            .ThenBy(h => h.Id)
            .ToListAsync(ct);

    public async Task AddAsync(DeliveryOrder order, Package package, OrderStatusHistory history, CancellationToken ct = default)
    {
        db.Packages.Add(package);
        db.DeliveryOrders.Add(order);
        db.OrderStatusHistories.Add(history);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DeliveryOrder order, OrderStatusHistory? history = null, CancellationToken ct = default)
    {
        if (db.Entry(order).State == EntityState.Detached)
            db.DeliveryOrders.Update(order);

        if (history is not null)
            db.OrderStatusHistories.Add(history);

        await db.SaveChangesAsync(ct);
    }
}
