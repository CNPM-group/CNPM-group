using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class UserService(IUserRepository users)
{
    public async Task<IReadOnlyList<UserWithRoles>> ListAsync()
    {
        var list = await users.ListAsync();
        var result = new List<UserWithRoles>(list.Count);
        foreach (var u in list)
            result.Add(new UserWithRoles(u, await users.GetRolesAsync(u.Id)));
        return result;
    }

    public async Task<UserWithRoles> CreateAsync(string email, string password, string fullName, string role)
    {
        var normalized = NormalizeRoles(new[] { role })[0];
        var user = await users.CreateAsync(new User { Email = email.Trim(), FullName = fullName.Trim() }, password, normalized);
        return new UserWithRoles(user, new[] { normalized });
    }

    public async Task<UserWithRoles> SetRolesAsync(Guid adminId, Guid userId, IReadOnlyCollection<string> roles)
    {
        var normalized = NormalizeRoles(roles);
        if (adminId == userId && !normalized.Contains(Roles.Admin))
            throw new ConflictException("Không tự gỡ quyền Admin của chính mình.");

        var user = await users.GetByIdAsync(userId) ?? throw new NotFoundException("Không tìm thấy tài khoản.");
        await users.SetRolesAsync(userId, normalized);
        return new UserWithRoles(user, normalized);
    }

    public async Task<UserWithRoles> SetActiveAsync(Guid adminId, Guid userId, bool isActive)
    {
        if (adminId == userId && !isActive)
            throw new ConflictException("Không tự khóa tài khoản của chính mình.");

        await users.SetActiveAsync(userId, isActive);
        var user = await users.GetByIdAsync(userId) ?? throw new NotFoundException("Không tìm thấy tài khoản.");
        return new UserWithRoles(user, await users.GetRolesAsync(userId));
    }

    private static string[] NormalizeRoles(IEnumerable<string> roles)
    {
        var result = new List<string>();
        foreach (var r in roles)
        {
            var match = Roles.All.FirstOrDefault(x => string.Equals(x, r?.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? throw new ValidationException($"Vai trò '{r}' không hợp lệ. Chọn trong: {string.Join(", ", Roles.All)}.");
            if (!result.Contains(match))
                result.Add(match);
        }

        if (result.Count == 0)
            throw new ValidationException("Tài khoản phải có ít nhất một vai trò.");

        return result.ToArray();
    }
}
