
namespace SmartDrone.Domain.Entities;

public class DeliveryOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TrackingCode { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public Guid PackageId { get; set; }

    public Guid OriginStationId { get; set; }

    public Guid DestinationStationId { get; set; }

    public Guid? DeliveryAddressId { get; set; }

    public string Status { get; set; } = "Pending";

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAtUtc { get; set; }

    public DateTime? ScheduledAtUtc { get; set; }

    public DateTime? EtaUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public string? FailureReason { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
