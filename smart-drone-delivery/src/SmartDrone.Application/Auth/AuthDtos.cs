using System.ComponentModel.DataAnnotations;

namespace SmartDrone.Application.Auth;

public sealed record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(8)] string Password,
    [property: Required, MaxLength(200)] string FullName);

public sealed record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles);

public sealed record AuthResult(bool Succeeded, AuthResponse? Data, IReadOnlyList<string> Errors)
{
    public static AuthResult Ok(AuthResponse data) => new(true, data, Array.Empty<string>());
    public static AuthResult Fail(params string[] errors) => new(false, null, errors);
    public static AuthResult Fail(IEnumerable<string> errors) => new(false, null, errors.ToArray());
}
