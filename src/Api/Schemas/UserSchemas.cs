using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record CreateUserRequest(
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8)] string Password,
    [Required, MaxLength(100)] string FullName,
    [Required] string Role);

public sealed record SetRolesRequest([Required, MinLength(1)] List<string> Roles);

public sealed record SetActiveRequest(bool IsActive);

public sealed record UserResponse(Guid Id, string Email, string FullName, string? Phone, bool IsActive, DateTime CreatedAtUtc, IReadOnlyList<string> Roles)
{
    public static UserResponse From(UserWithRoles u) =>
        new(u.User.Id, u.User.Email, u.User.FullName, u.User.Phone, u.User.IsActive, u.User.CreatedAtUtc, u.Roles);
}
