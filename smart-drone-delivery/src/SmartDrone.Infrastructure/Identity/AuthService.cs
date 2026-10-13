using Microsoft.AspNetCore.Identity;
using SmartDrone.Application.Abstractions;
using SmartDrone.Application.Auth;
using SmartDrone.Domain.Common;

namespace SmartDrone.Infrastructure.Identity;

public sealed class AuthService(UserManager<AppUser> users, JwtTokenService tokens) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var user = new AppUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            FullName = request.FullName.Trim()
        };

        var created = await users.CreateAsync(user, request.Password);
        if (!created.Succeeded)
            return AuthResult.Fail(created.Errors.Select(e => e.Description));

        // Tự đăng ký chỉ được làm Customer. Role khác do Admin cấp.
        await users.AddToRoleAsync(user, Roles.Customer);
        return AuthResult.Ok(await BuildResponseAsync(user));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await users.CheckPasswordAsync(user, request.Password))
            return AuthResult.Fail("Email hoặc mật khẩu không đúng.");

        return AuthResult.Ok(await BuildResponseAsync(user));
    }

    private async Task<AuthResponse> BuildResponseAsync(AppUser user)
    {
        var roles = (await users.GetRolesAsync(user)).ToList();
        var (token, expires) = tokens.Create(user, roles);
        return new AuthResponse(token, expires, user.Id, user.Email ?? string.Empty, user.FullName, roles);
    }
}
