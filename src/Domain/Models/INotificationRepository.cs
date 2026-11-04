namespace SmartDrone.Domain.Models;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> ListByUserAsync(Guid userId, bool unreadOnly, CancellationToken ct = default);
    Task<Notification?> GetAsync(Guid id, Guid userId, CancellationToken ct = default);
    void Add(Notification notification);
    Task SaveAsync(CancellationToken ct = default);
}
