namespace SmartDrone.Domain.Models;

public sealed record TrackingSnapshot(
    Guid OrderId,
    Guid CustomerId,
    string TrackingCode,
    string Status,
    string? DroneCode,
    double Latitude,
    double Longitude,
    double ProgressPct,
    string OriginName,
    double OriginLatitude,
    double OriginLongitude,
    string DestinationName,
    double DestinationLatitude,
    double DestinationLongitude,
    DateTime? DepartedAtUtc,
    DateTime? EtaUtc);
