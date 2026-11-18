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

    public async Task<UserWithRoles> GetProfileAsync(Guid userId)
    {
        var user = await users.GetByIdAsync(userId) ?? throw new NotFoundException("Không tìm thấy tài khoản.");
        return new UserWithRoles(user, await users.GetRolesAsync(userId));
    }

    // FR01: tự cập nhật họ tên và số điện thoại. Email và vai trò không tự đổi được.
    public async Task<UserWithRoles> UpdateProfileAsync(Guid userId, string fullName, string? phone)
    {
        var user = await users.UpdateProfileAsync(userId, ProfileRules.Name(fullName), ProfileRules.Phone(phone));
        return new UserWithRoles(user, await users.GetRolesAsync(userId));
    }

    // UC-03: đổi mật khẩu, phải nhập đúng mật khẩu cũ. Token đang dùng vẫn còn hạn tới khi hết giờ.
    public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            throw new ValidationException("Mật khẩu mới tối thiểu 8 ký tự.");
        if (newPassword == currentPassword)
            throw new ValidationException("Mật khẩu mới phải khác mật khẩu cũ.");
        await users.ChangePasswordAsync(userId, currentPassword, newPassword);
    }

    private async Task<AuthSession> CreateSessionAsync(User user)
    {
        var roles = await users.GetRolesAsync(user.Id);
        var (token, expires) = tokens.Create(user, roles);
        return new AuthSession(token, expires, user, roles);
    }
}
