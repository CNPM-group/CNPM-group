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
        Guid? customerId, string? status, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.DeliveryOrders.AsNoTracking();

        if (customerId is not null)
            query = query.Where(o => o.CustomerId == customerId);

        if (status is not null)
            query = query.Where(o => o.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var code = search.Trim().ToUpper();
            query = query.Where(o => o.TrackingCode.ToUpper().Contains(code));
        }

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
                o.CreatedAtUtc,
                o.RecipientName,
                o.RecipientPhone))
            .ToListAsync(ct);

        return new PagedResult<OrderSummary>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyDictionary<Guid, CustomerOrderStats>> StatsByCustomerAsync(CancellationToken ct = default)
    {
        var rows = await db.DeliveryOrders.AsNoTracking()
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Total = g.Count(),
                Completed = g.Count(o => o.Status == "Completed"),
                Active = g.Count(o => o.Status != "Completed" && o.Status != "Cancelled" && o.Status != "Rejected"),
                Last = g.Max(o => (DateTime?)o.CreatedAtUtc)
            })
            .ToListAsync(ct);

        return rows.ToDictionary(r => r.CustomerId, r => new CustomerOrderStats(r.Total, r.Completed, r.Active, r.Last));
    }

    public Task<int> CountIncomingAsync(Guid destinationStationId, Guid exceptOrderId, CancellationToken ct = default) =>
        db.DeliveryOrders.CountAsync(o =>
            o.DestinationStationId == destinationStationId &&
            o.Id != exceptOrderId &&
            (o.Status == "Scheduled" || o.Status == "InTransit" || o.Status == "ArrivedAtStation"), ct);

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
