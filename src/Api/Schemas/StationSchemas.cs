using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record StationRequest(
    [Required, MaxLength(200)] string Name,
    [Range(-90.0, 90.0)] decimal Latitude,
    [Range(-180.0, 180.0)] decimal Longitude,
    StationStatus Status = StationStatus.Available,
    [Range(1, 50)] short Capacity = 2);

public sealed record StationResponse(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    StationStatus Status,
    DateTime CreatedAtUtc,
    short Capacity)
{
    public static StationResponse From(LandingStation s) =>
        new(s.Id, s.Name, s.Latitude, s.Longitude, s.Status, s.CreatedAtUtc, s.Capacity);
}

// Tải trạm (v0.11): đang bay tới, kiện chờ khách nhận, đơn sắp đi.
public sealed record StationLoadResponse(Guid StationId, int Incoming, int Waiting, int Outgoing)
{
    public static StationLoadResponse From(StationLoad l) => new(l.StationId, l.Incoming, l.Waiting, l.Outgoing);
}
