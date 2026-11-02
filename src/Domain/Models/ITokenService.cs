namespace SmartDrone.Domain.Models;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) Create(User user, IEnumerable<string> roles);
}
