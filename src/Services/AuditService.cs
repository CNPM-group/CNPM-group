using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

// UC-29 (v0.11): Admin xem nhật ký thao tác. Ghi nhật ký do AuditInterceptor ở Infrastructure tự làm.
public sealed class AuditService(IAuditLogRepository logs)
{
    public Task<PagedResult<AuditEntry>> PageAsync(string? entityName, int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, ApiConstants.MaxPageSize);
        return logs.PageAsync(entityName?.Trim(), page, pageSize, ct);
    }
}
