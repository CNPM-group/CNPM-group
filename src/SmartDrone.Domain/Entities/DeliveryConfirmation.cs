
namespace SmartDrone.Domain.Entities;

public class DeliveryConfirmation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrderId { get; set; }

    public string CodeHash { get; set; } = string.Empty;

    public string Method { get; set; } = "OTP";

    public DateTime ExpiresAtUtc { get; set; }

    public short FailedAttempts { get; set; } = 0;

    public DateTime? VerifiedAtUtc { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
