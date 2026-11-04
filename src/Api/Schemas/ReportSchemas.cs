using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record ReportResponse(
    DateTime FromUtc,
    DateTime ToUtc,
    int TotalOrders,
    int CompletedOrders,
    int FailedOrders,
    double SuccessRatePct,
    double? AvgDeliveryMinutes,
    IReadOnlyList<StatusCount> ByStatus,
    IReadOnlyList<DailyCount> ByDay)
{
    public static ReportResponse From(OrderReport r) =>
        new(r.FromUtc, r.ToUtc, r.TotalOrders, r.CompletedOrders, r.FailedOrders, r.SuccessRatePct, r.AvgDeliveryMinutes, r.ByStatus, r.ByDay);
}
