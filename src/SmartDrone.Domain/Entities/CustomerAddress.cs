
namespace SmartDrone.Domain.Entities;

public class CustomerAddress
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }

    public string Label { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; } = false;

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
