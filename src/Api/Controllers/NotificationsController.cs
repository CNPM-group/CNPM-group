using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/notifications")]
[Authorize]
public class NotificationsController(NotificationService notifications) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> GetMine([FromQuery] bool unreadOnly = false, CancellationToken ct = default) =>
        Ok((await notifications.ListAsync(User.UserId(), unreadOnly, ct)).Select(NotificationResponse.From).ToList());

    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult<NotificationResponse>> MarkRead(Guid id, CancellationToken ct) =>
        Ok(NotificationResponse.From(await notifications.MarkReadAsync(User.UserId(), id, ct)));
}
