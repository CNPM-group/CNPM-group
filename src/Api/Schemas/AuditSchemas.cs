using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

// oldValues / newValues là chuỗi JSON đúng như lưu trong cột jsonb.
public sealed record AuditLogResponse(
    long Id, DateTime CreatedAtUtc, Guid? UserId, string? UserEmail, string Action,
    string EntityName, string? EntityId, string? OldValues, string? NewValues, string? IpAddress)
{
    public static AuditLogResponse From(AuditEntry e) => new(
        e.Log.Id, e.Log.CreatedAtUtc, e.Log.UserId, e.UserEmail, e.Log.Action,
        e.Log.EntityName, e.Log.EntityId, e.Log.OldValues, e.Log.NewValues, e.Log.IpAddress);
}

public sealed record AuditPageResponse(IReadOnlyList<AuditLogResponse> Items, int Page, int PageSize, int TotalItems)
{
    public static AuditPageResponse From(PagedResult<AuditEntry> p) =>
        new(p.Items.Select(AuditLogResponse.From).ToList(), p.Page, p.PageSize, p.TotalItems);
}
