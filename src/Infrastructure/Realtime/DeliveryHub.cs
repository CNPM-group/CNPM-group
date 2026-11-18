using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartDrone.Domain;

namespace SmartDrone.Infrastructure.Realtime;

[Authorize]
public sealed class DeliveryHub : Hub
{
    public const string Path = "/hubs/delivery";
    public const string StaffGroup = "staff";

    public static string UserGroup(Guid userId) => $"user-{userId}";

    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user is not null && Roles.Staff.Any(user.IsInRole))
            await Groups.AddToGroupAsync(Context.ConnectionId, StaffGroup);

        if (Guid.TryParse(user?.FindFirst("sub")?.Value, out var userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

        await base.OnConnectedAsync();
    }
}
