using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record OrderRequest(
    [Required] Guid OriginStationId,
    [Required] Guid DestinationStationId,
    Guid? DeliveryAddressId,
    [MaxLength(1000)] string? Notes,
    [Required] PackageInput Package)
{
    public DeliveryOrder ToOrder() => new()
    {
        OriginStationId = OriginStationId,
        DestinationStationId = DestinationStationId,
        DeliveryAddressId = DeliveryAddressId,
        Notes = Notes
    };

    public Package ToPackage() => new()
    {
        Description = Package.Description,
        WeightKg = Package.WeightKg,
        LengthCm = Package.LengthCm,
        WidthCm = Package.WidthCm,
        HeightCm = Package.HeightCm,
        ImageKey = Package.ImageKey
    };
}

public sealed record PackageInput(
    [MaxLength(255)] string? Description,
    [Range(0.01, 50)] decimal WeightKg,
    [Range(0.1, 99999.9)] decimal LengthCm,
    [Range(0.1, 99999.9)] decimal WidthCm,
    [Range(0.1, 99999.9)] decimal HeightCm,
    [MaxLength(255)] string? ImageKey);

public sealed record OrderResponse(
    Guid Id,
    string TrackingCode,
    Guid CustomerId,
    Guid PackageId,
    Guid OriginStationId,
    Guid DestinationStationId,
    string Status,
    DateTime? Eta,
    DateTime CreatedAtUtc)
{
    public static OrderResponse From(DeliveryOrder o) =>
        new(o.Id, o.TrackingCode, o.CustomerId, o.PackageId, o.OriginStationId, o.DestinationStationId, o.Status, o.EtaUtc, o.CreatedAtUtc);
}

public sealed record OrderSummaryResponse(
    Guid Id,
    string TrackingCode,
    string Status,
    string OriginStationName,
    string DestinationStationName,
    DateTime? Eta,
    DateTime CreatedAtUtc)
{
    public static OrderSummaryResponse From(OrderSummary o) =>
        new(o.Id, o.TrackingCode, o.Status, o.OriginStationName, o.DestinationStationName, o.EtaUtc, o.CreatedAtUtc);
}

public sealed record OrderPageResponse(
    IReadOnlyList<OrderSummaryResponse> Items,
    int Page,
    int PageSize,
    int TotalItems)
{
    public static OrderPageResponse From(PagedResult<OrderSummary> p) =>
        new(p.Items.Select(OrderSummaryResponse.From).ToList(), p.Page, p.PageSize, p.TotalItems);
}

public sealed record OrderStatusHistoryResponse(
    string? FromStatus,
    string ToStatus,
    Guid? ChangedBy,
    string? Note,
    DateTime ChangedAtUtc)
{
    public static OrderStatusHistoryResponse From(OrderStatusHistory h) =>
        new(h.FromStatus, h.ToStatus, h.ChangedBy, h.Note, h.ChangedAtUtc);
}

public sealed record EtaResponse(double DistanceKm, double EstimatedMinutes)
{
    public static EtaResponse From(EtaEstimate e) => new(e.DistanceKm, e.EstimatedMinutes);
}

public sealed record NoteRequest([MaxLength(1000)] string? Note);

public sealed record RejectRequest([Required, MinLength(1), MaxLength(1000)] string Reason);
