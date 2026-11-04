using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class ReportRepository(AppDbContext db) : IReportRepository
{
    public async Task<IReadOnlyList<(string Status, DateTime CreatedAtUtc, DateTime? CompletedAtUtc)>> OrdersInRangeAsync(
        DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        var rows = await db.DeliveryOrders.AsNoTracking()
            .Where(o => o.CreatedAtUtc >= fromUtc && o.CreatedAtUtc < toUtc)
            .Select(o => new { o.Status, o.CreatedAtUtc, o.CompletedAtUtc })
            .ToListAsync(ct);

        return rows.Select(r => (r.Status, r.CreatedAtUtc, r.CompletedAtUtc)).ToList();
    }

    public async Task<IReadOnlyList<OrderExportRow>> ExportRowsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default) =>
        await db.DeliveryOrders.AsNoTracking()
            .Where(o => o.CreatedAtUtc >= fromUtc && o.CreatedAtUtc < toUtc)
            .OrderBy(o => o.CreatedAtUtc)
            .Select(o => new OrderExportRow(
                o.TrackingCode,
                o.Status,
                db.Stations.Where(s => s.Id == o.OriginStationId).Select(s => s.Name).FirstOrDefault() ?? "",
                db.Stations.Where(s => s.Id == o.DestinationStationId).Select(s => s.Name).FirstOrDefault() ?? "",
                db.Packages.Where(p => p.Id == o.PackageId).Select(p => p.WeightKg).FirstOrDefault(),
                o.CreatedAtUtc,
                o.CompletedAtUtc,
                o.FailureReason))
            .ToListAsync(ct);
}
