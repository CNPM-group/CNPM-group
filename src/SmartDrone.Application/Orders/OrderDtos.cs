
using System.ComponentModel.DataAnnotations;

namespace SmartDrone.Application.Orders;

public sealed record CreateOrderRequest(
    [Required] Guid OriginStationId,
    [Required] Guid DestinationStationId,
    [Required, Range(0.01, 20)] decimal WeightKg,
    [Required, Range(0.1, 100)] decimal LengthCm,
    [Required, Range(0.1, 100)] decimal WidthCm,
    [Required, Range(0.1, 100)] decimal HeightCm,
    [MaxLength(500)] string? Description,
    [MaxLength(1000)] string? Notes);

public sealed record OrderDto(
    Guid Id,
    string TrackingCode,
    Guid CustomerId,
    Guid PackageId,
    Guid OriginStationId,
    Guid DestinationStationId,
    string Status,
    DateTime CreatedAtUtc);
