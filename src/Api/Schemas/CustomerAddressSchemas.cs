using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed class CustomerAddressRequest
{
    [Required, MaxLength(50)]
    public string Label { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string AddressLine { get; set; } = string.Empty;

    [Range(typeof(decimal), "-90", "90")]
    public decimal? Latitude { get; set; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; }

    public CustomerAddress ToDomain() => new()
    {
        Label = Label,
        AddressLine = AddressLine,
        Latitude = Latitude,
        Longitude = Longitude,
        IsDefault = IsDefault
    };
}

public sealed record CustomerAddressResponse(
    Guid Id,
    string Label,
    string AddressLine,
    decimal? Latitude,
    decimal? Longitude,
    bool IsDefault,
    DateTime CreatedAtUtc)
{
    public static CustomerAddressResponse From(CustomerAddress a) =>
        new(a.Id, a.Label, a.AddressLine, a.Latitude, a.Longitude, a.IsDefault, a.CreatedAtUtc);
}
