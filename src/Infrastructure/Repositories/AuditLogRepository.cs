using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class AuditLogRepository(AppDbContext db) : IAuditLogRepository
{
    public async Task<PagedResult<AuditEntry>> PageAsync(string? entityName, int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CreatedAtUtc)
            .ThenByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditEntry(a, db.Users.Where(u => u.Id == a.UserId).Select(u => u.Email).FirstOrDefault()))
            .ToListAsync(ct);

        return new PagedResult<AuditEntry>(items, page, pageSize, total);
    }
}
