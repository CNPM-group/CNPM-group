using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class AuthService(IUserRepository users, ITokenService tokens)
{
    public async Task<AuthSession> RegisterAsync(string email, string password, string fullName)
    {
        var user = await users.CreateAsync(
            new User { Email = email.Trim(), FullName = fullName.Trim() },
            password,
            Roles.Customer);

        return await CreateSessionAsync(user);
    }

    public async Task<AuthSession> LoginAsync(string email, string password)
    {
        var user = await users.GetByEmailAsync(email.Trim());
        if (user is null || !await users.CheckPasswordAsync(user.Id, password))
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");

        if (!user.IsActive)
            throw new ForbiddenException("Tài khoản đã bị khóa.");

        return await CreateSessionAsync(user);
    }

    private async Task<AuthSession> CreateSessionAsync(User user)
    {
        var roles = await users.GetRolesAsync(user.Id);
        var (token, expires) = tokens.Create(user, roles);
        return new AuthSession(token, expires, user, roles);
    }
}
