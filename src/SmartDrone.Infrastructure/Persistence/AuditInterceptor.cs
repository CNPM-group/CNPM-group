using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartDrone.Application.Abstractions;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence;

public sealed class AuditInterceptor(ICurrentUser user) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        var db = eventData.Context;
        if (db is null) return ValueTask.FromResult(result);

        var logs = db.ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditLog
                     && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(ToLog)
            .ToList();

        db.Set<AuditLog>().AddRange(logs);
        return ValueTask.FromResult(result);
    }

    private AuditLog ToLog(EntityEntry e)
    {
        var old = e.State == EntityState.Added ? null
            : e.Properties.Where(p => e.State == EntityState.Deleted || p.IsModified)
                          .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
        var now = e.State == EntityState.Deleted ? null
            : e.Properties.Where(p => e.State == EntityState.Added || p.IsModified)
                          .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

        return new AuditLog
        {
            UserId = user.UserId,
            Action = e.State.ToString(),
            EntityName = e.Metadata.ClrType.Name,
            EntityId = e.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
            OldValues = old is null ? null : JsonSerializer.Serialize(old),
            NewValues = now is null ? null : JsonSerializer.Serialize(now)
        };
    }
}
