
namespace SmartDrone.Domain.Entities;

public class Package
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Description { get; set; }

    public decimal WeightKg { get; set; }

    public decimal LengthCm { get; set; }

    public decimal WidthCm { get; set; }

    public decimal HeightCm { get; set; }

    public string? ImageKey { get; set; }

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
