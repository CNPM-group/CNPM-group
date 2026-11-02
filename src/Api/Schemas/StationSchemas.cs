using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record StationRequest(
    [Required, MaxLength(200)] string Name,
    [Range(-90.0, 90.0)] decimal Latitude,
    [Range(-180.0, 180.0)] decimal Longitude,
    StationStatus Status = StationStatus.Available);

public sealed record StationResponse(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    StationStatus Status,
    DateTime CreatedAtUtc)
{
    public static StationResponse From(LandingStation s) =>
        new(s.Id, s.Name, s.Latitude, s.Longitude, s.Status, s.CreatedAtUtc);
}
