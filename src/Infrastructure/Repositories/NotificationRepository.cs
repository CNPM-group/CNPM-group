using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class NotificationRepository(AppDbContext db) : INotificationRepository
{
    public async Task<IReadOnlyList<Notification>> ListByUserAsync(Guid userId, bool unreadOnly, CancellationToken ct = default) =>
        await db.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId && (!unreadOnly || !n.IsRead))
            .OrderByDescending(n => n.CreatedAtUtc)
            .Take(100)
            .ToListAsync(ct);

    public Task<Notification?> GetAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, ct);

    public void Add(Notification notification) => db.Notifications.Add(notification);

    public Task SaveAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
