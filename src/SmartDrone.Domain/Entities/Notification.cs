
namespace SmartDrone.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public Guid? OrderId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Body { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAtUtc { get; set; } =
        DateTime.UtcNow;
}
