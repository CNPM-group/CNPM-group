namespace SmartDrone.Domain.Models;

public sealed record StatusCount(string Status, int Count);

public sealed record DailyCount(DateOnly Day, int Count);

public sealed record OrderReport(
    DateTime FromUtc,
    DateTime ToUtc,
    int TotalOrders,
    int CompletedOrders,
    int FailedOrders,
    double SuccessRatePct,
    double? AvgDeliveryMinutes,
    IReadOnlyList<StatusCount> ByStatus,
    IReadOnlyList<DailyCount> ByDay);

public sealed record OrderExportRow(
    string TrackingCode,
    string Status,
    string OriginStation,
    string DestinationStation,
    decimal WeightKg,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc,
    string? FailureReason);

public interface IReportRepository
{
    Task<IReadOnlyList<(string Status, DateTime CreatedAtUtc, DateTime? CompletedAtUtc)>> OrdersInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
    Task<IReadOnlyList<OrderExportRow>> ExportRowsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
}
