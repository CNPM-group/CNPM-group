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

public sealed record MeResponse(string? Id, string? Email, string? Name, IReadOnlyList<string> Roles);
