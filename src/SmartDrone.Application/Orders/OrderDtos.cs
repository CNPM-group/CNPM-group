
using System.ComponentModel.DataAnnotations;

namespace SmartDrone.Application.Orders;

public sealed record CreateOrderRequest(
    [Required] Guid OriginStationId,
    [Required] Guid DestinationStationId,
    Guid? DeliveryAddressId,
    [MaxLength(1000)] string? Notes,
    [Required] PackageInput Package
);

public sealed record UpdateOrderRequest(
    [Required] Guid OriginStationId,
    [Required] Guid DestinationStationId,
    Guid? DeliveryAddressId,
    [MaxLength(1000)] string? Notes,
    [Required] PackageInput Package
);

public sealed record PackageInput(
    [MaxLength(255)] string? Description,
    [Range(0.01, 50)] decimal WeightKg,
    [Range(0.1, 99999.9)] decimal LengthCm,
    [Range(0.1, 99999.9)] decimal WidthCm,
    [Range(0.1, 99999.9)] decimal HeightCm,
    [MaxLength(255)] string? ImageKey
);

public sealed record OrderDto(
    Guid Id,
    string TrackingCode,
    Guid CustomerId,
    Guid PackageId,
    Guid OriginStationId,
    Guid DestinationStationId,
    string Status,
    DateTime CreatedAtUtc
);

public sealed record OrderStatusHistoryDto(
    string? FromStatus,
    string ToStatus,
    string? ChangedByName,
    string? Note,
    DateTime ChangedAtUtc
);


public sealed record OrderSummaryDto(
    Guid Id,
    string TrackingCode,
    string Status,
    string OriginStationName,
    string DestinationStationName,
    DateTime? Eta,
    DateTime CreatedAtUtc
);

public sealed record OrderPageDto(
    List<OrderSummaryDto> Items,
    int Page,
    int PageSize,
    int TotalItems
);