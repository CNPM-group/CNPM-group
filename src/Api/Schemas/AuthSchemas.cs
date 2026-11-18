using System.ComponentModel.DataAnnotations;
using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8)] string Password,
    [Required, MaxLength(100)] string FullName);

public sealed record LoginRequest(
    [Required] string Email,
    [Required] string Password);

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles)
{
    public static AuthResponse From(AuthSession s) =>
        new(s.Token, s.ExpiresAtUtc, s.User.Id, s.User.Email, s.User.FullName, s.Roles);
}


// v0.11: hồ sơ cá nhân (GET/PUT /auth/me).
public sealed record ProfileRequest([Required, MaxLength(100)] string FullName, [MaxLength(20)] string? Phone);

public sealed record ProfileResponse(Guid Id, string Email, string FullName, string? Phone, IReadOnlyList<string> Roles)
{
    public static ProfileResponse From(UserWithRoles u) => new(u.User.Id, u.User.Email, u.User.FullName, u.User.Phone, u.Roles);
}

public sealed record ChangePasswordRequest([Required] string CurrentPassword, [Required, MinLength(8)] string NewPassword);
