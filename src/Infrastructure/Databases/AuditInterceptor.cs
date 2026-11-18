using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Databases;

// BR-25 / NFR "Truy vết" (v0.11): trước mỗi lần SaveChanges, ghi 1 dòng audit_logs cho mỗi bản ghi
// thêm / sửa / xóa: ai làm, lúc nào, IP, giá trị cũ và mới (dạng JSON).
// Chỉ ghi thao tác của người dùng qua API: bộ giả lập drone và seed lúc khởi động không có HttpContext nên bỏ qua.
public sealed class AuditInterceptor(IHttpContextAccessor http) : SaveChangesInterceptor
{
    // Bảng tự sinh số lượng lớn hoặc đã là lịch sử, ghi nữa chỉ thêm rác.
    private static readonly HashSet<string> SkippedEntities = new()
    {
        nameof(AuditLog), nameof(FlightTelemetry), nameof(Notification), nameof(OrderStatusHistory)
    };

    // Không bao giờ ghi các cột bí mật vào nhật ký.
    private static readonly HashSet<string> SecretProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "CodeHash", "KeyHash"
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditLogs(DbContext? db)
    {
        var context = http.HttpContext;
        if (db is null || context?.User.Identity?.IsAuthenticated != true)
            return;

        db.ChangeTracker.DetectChanges();
        Guid? userId = Guid.TryParse(context.User.FindFirst("sub")?.Value, out var id) ? id : null;
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var now = DateTime.UtcNow;
        var logs = new List<AuditLog>();

        foreach (var entry in db.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            var name = entry.Metadata.ClrType.Name;
            if (SkippedEntities.Contains(name))
                continue;

            var props = entry.Properties.Where(p => !SecretProperties.Contains(p.Metadata.Name)).ToList();
            Dictionary<string, object?>? oldValues = null;
            Dictionary<string, object?>? newValues = null;

            switch (entry.State)
            {
                case EntityState.Added:
                    newValues = props.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    break;
                case EntityState.Deleted:
                    oldValues = props.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    break;
                default:
                    var changed = props.Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue)).ToList();
                    if (changed.Count == 0)
                        continue;
                    oldValues = changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    newValues = changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    break;
            }

            var key = entry.Metadata.FindPrimaryKey();
            var entityId = key is null ? null : string.Join(",", key.Properties.Select(k => entry.Property(k.Name).CurrentValue));
            logs.Add(new AuditLog
            {
                UserId = userId,
                Action = entry.State.ToString(),
                EntityName = name.Length > 100 ? name[..100] : name,
                EntityId = entityId is { Length: > 64 } ? entityId[..64] : entityId,
                OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
                NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues),
                IpAddress = ip,
                CreatedAtUtc = now
            });
        }

        if (logs.Count > 0)
            db.Set<AuditLog>().AddRange(logs);
    }
}
