using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

// UC-29 (v0.11): nhật ký thao tác, chỉ Admin.
[ApiController]
[Route(ApiConstants.Prefix + "/audit-logs")]
[Authorize(Roles = Roles.Admin)]
public class AuditLogsController(AuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AuditPageResponse>> GetPage(
        [FromQuery] string? entity = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = ApiConstants.DefaultPageSize,
        CancellationToken ct = default) =>
        Ok(AuditPageResponse.From(await audit.PageAsync(entity, page, pageSize, ct)));
}
