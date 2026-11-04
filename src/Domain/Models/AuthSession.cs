namespace SmartDrone.Domain.Models;

public sealed record AuthSession(string Token, DateTime ExpiresAtUtc, User User, IReadOnlyList<string> Roles);
