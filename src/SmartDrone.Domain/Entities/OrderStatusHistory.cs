
namespace SmartDrone.Domain.Entities;

public class OrderStatusHistory
{
    public long Id { get; set; }

    public Guid OrderId { get; set; }

    public string? FromStatus { get; set; }

    public string ToStatus { get; set; } = string.Empty;

    public Guid? ChangedBy { get; set; }

    public string? Note { get; set; }

    public DateTime ChangedAtUtc { get; set; } =
        DateTime.UtcNow;
}
