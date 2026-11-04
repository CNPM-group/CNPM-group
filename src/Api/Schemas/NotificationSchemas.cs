using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record NotificationResponse(Guid Id, Guid? OrderId, string Title, string? Body, bool IsRead, DateTime CreatedAtUtc)
{
    public static NotificationResponse From(Notification n) =>
        new(n.Id, n.OrderId, n.Title, n.Body, n.IsRead, n.CreatedAtUtc);
}
