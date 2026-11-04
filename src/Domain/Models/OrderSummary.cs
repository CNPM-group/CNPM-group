namespace SmartDrone.Domain.Models;

public sealed record OrderSummary(
    Guid Id,
    string TrackingCode,
    string Status,
    string OriginStationName,
    string DestinationStationName,
    DateTime? EtaUtc,
    DateTime CreatedAtUtc);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems);
