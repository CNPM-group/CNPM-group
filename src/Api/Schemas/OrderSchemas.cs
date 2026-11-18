using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record OrderRequest(
    [Required] Guid OriginStationId,
    [Required] Guid DestinationStationId,
    Guid? DeliveryAddressId,
    [MaxLength(1000)] string? Notes,
    [Required] PackageInput Package,
    [MaxLength(100)] string? RecipientName = null,
    [MaxLength(20)] string? RecipientPhone = null)
{
    public DeliveryOrder ToOrder() => new()
    {
        OriginStationId = OriginStationId,
        DestinationStationId = DestinationStationId,
        DeliveryAddressId = DeliveryAddressId,
        Notes = Notes,
        RecipientName = RecipientName,
        RecipientPhone = RecipientPhone
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
    [Range(0.1, 100)] decimal LengthCm,
    [Range(0.1, 100)] decimal WidthCm,
    [Range(0.1, 100)] decimal HeightCm,
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

public sealed record StationRef(Guid Id, string Name);

public sealed record PackageResponse(
    Guid Id,
    string? Description,
    decimal WeightKg,
    decimal LengthCm,
    decimal WidthCm,
    decimal HeightCm,
    string? ImageKey)
{
    public static PackageResponse From(Package p) =>
        new(p.Id, p.Description, p.WeightKg, p.LengthCm, p.WidthCm, p.HeightCm, p.ImageKey);
}

public sealed record OrderDetailResponse(
    Guid Id,
    string TrackingCode,
    string Status,
    Guid CustomerId,
    StationRef OriginStation,
    StationRef DestinationStation,
    PackageResponse Package,
    DateTime? Eta,
    DateTime? ScheduledAtUtc,
    DateTime? ApprovedAtUtc,
    DateTime? CompletedAtUtc,
    string? FailureReason,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    string? RecipientName,
    string? RecipientPhone,
    Guid? DeliveryAddressId)
{
    public static OrderDetailResponse From(OrderDetail d)
    {
        var o = d.Order;
        return new(
            o.Id, o.TrackingCode, o.Status, o.CustomerId,
            new StationRef(d.Origin.Id, d.Origin.Name),
            new StationRef(d.Destination.Id, d.Destination.Name),
            PackageResponse.From(d.Package),
            o.EtaUtc, o.ScheduledAtUtc, o.ApprovedAtUtc, o.CompletedAtUtc,
            o.FailureReason, o.Notes, o.CreatedAtUtc, o.UpdatedAtUtc,
            o.RecipientName, o.RecipientPhone, o.DeliveryAddressId);
    }
}

public sealed record TrackingResponse(
    Guid OrderId,
    string TrackingCode,
    string Status,
    string? DroneCode,
    double Latitude,
    double Longitude,
    double ProgressPct,
    StationPoint Origin,
    StationPoint Destination,
    DateTime? DepartedAtUtc,
    DateTime? EtaUtc)
{
    public static TrackingResponse From(TrackingSnapshot s) => new(
        s.OrderId, s.TrackingCode, s.Status, s.DroneCode, s.Latitude, s.Longitude, s.ProgressPct,
        new StationPoint(s.OriginName, s.OriginLatitude, s.OriginLongitude),
        new StationPoint(s.DestinationName, s.DestinationLatitude, s.DestinationLongitude),
        s.DepartedAtUtc, s.EtaUtc);
}

public sealed record StationPoint(string Name, double Latitude, double Longitude);

public sealed record OrderSummaryResponse(
    Guid Id,
    string TrackingCode,
    string Status,
    string OriginStationName,
    string DestinationStationName,
    DateTime? Eta,
    DateTime CreatedAtUtc,
    string? RecipientName,
    string? RecipientPhone)
{
    public static OrderSummaryResponse From(OrderSummary o) =>
        new(o.Id, o.TrackingCode, o.Status, o.OriginStationName, o.DestinationStationName, o.EtaUtc, o.CreatedAtUtc,
            o.RecipientName, o.RecipientPhone);
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

public sealed record ScheduleRequest([Required] Guid DroneId, DateTime? ScheduledAtUtc);

public sealed record ConfirmRequest([Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Mã nhận hàng gồm 6 chữ số.")] string Otp);

public sealed record FailRequest([Required, MinLength(1), MaxLength(1000)] string Reason);
