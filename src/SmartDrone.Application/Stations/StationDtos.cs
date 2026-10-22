using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain.Enums;

namespace SmartDrone.Application.Stations;

public sealed record StationDto(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    StationStatus Status,
    DateTime CreatedAtUtc);

public sealed record StationRequest(
    [Required, MaxLength(200)] string Name,
    [Range(-90.0, 90.0)] decimal Latitude,
    [Range(-180.0, 180.0)] decimal Longitude,
    StationStatus Status = StationStatus.Available);
