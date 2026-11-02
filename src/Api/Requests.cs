using System.Security.Claims;
using SmartDrone.Domain;

namespace SmartDrone.Api;

public static class Requests
{
    public static Guid UserId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue("sub"), out var id)
            ? id
            : throw new UnauthorizedException("Không xác định được tài khoản.");

    public static bool IsStaff(this ClaimsPrincipal user) =>
        Roles.Staff.Any(user.IsInRole);

    public static Guid? CustomerScope(this ClaimsPrincipal user) =>
        user.IsStaff() ? null : user.UserId();
}
