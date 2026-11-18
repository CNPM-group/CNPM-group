namespace SmartDrone.Domain.Models;

// Một dòng nhật ký kèm email người thao tác (v0.11).
public sealed record AuditEntry(AuditLog Log, string? UserEmail);

public interface IAuditLogRepository
{
    Task<PagedResult<AuditEntry>> PageAsync(string? entityName, int page, int pageSize, CancellationToken ct = default);
}
