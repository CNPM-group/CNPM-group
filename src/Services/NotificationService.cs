using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class NotificationService(INotificationRepository notifications)
{
    public Task<IReadOnlyList<Notification>> ListAsync(Guid userId, bool unreadOnly, CancellationToken ct = default) =>
        notifications.ListByUserAsync(userId, unreadOnly, ct);

    public async Task<Notification> MarkReadAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var n = await notifications.GetAsync(id, userId, ct) ?? throw new NotFoundException("Không tìm thấy thông báo.");
        n.IsRead = true;
        await notifications.SaveAsync(ct);
        return n;
    }
}
