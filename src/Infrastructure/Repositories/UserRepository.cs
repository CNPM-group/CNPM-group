using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Models;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class UserRepository(UserManager<AppUser> users) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        var entity = await users.FindByEmailAsync(email);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        var entity = await users.FindByIdAsync(userId.ToString());
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<User>> ListAsync()
    {
        var entities = await users.Users.AsNoTracking().OrderBy(u => u.Email).ToListAsync();
        return entities.Select(ToDomain).ToList();
    }

    public async Task SetRolesAsync(Guid userId, IReadOnlyCollection<string> roles)
    {
        var entity = await FindAsync(userId);
        var current = await users.GetRolesAsync(entity);

        var removed = await users.RemoveFromRolesAsync(entity, current.Except(roles));
        EnsureSucceeded(removed);

        var added = await users.AddToRolesAsync(entity, roles.Except(current));
        EnsureSucceeded(added);
    }

    public async Task SetActiveAsync(Guid userId, bool isActive)
    {
        var entity = await FindAsync(userId);
        entity.IsActive = isActive;
        EnsureSucceeded(await users.UpdateAsync(entity));
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password)
    {
        var entity = await users.FindByIdAsync(userId.ToString());
        return entity is not null && await users.CheckPasswordAsync(entity, password);
    }

    public async Task<User> CreateAsync(User user, string password, string role)
    {
        var entity = new AppUser
        {
            UserName = user.Email,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.Phone,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc
        };

        var created = await users.CreateAsync(entity, password);
        if (!created.Succeeded)
            throw new ValidationException(string.Join(" ", created.Errors.Select(e => e.Description)));

        await users.AddToRoleAsync(entity, role);
        return ToDomain(entity);
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId)
    {
        var entity = await users.FindByIdAsync(userId.ToString());
        if (entity is null)
            return Array.Empty<string>();

        return (await users.GetRolesAsync(entity)).ToList();
    }

    private async Task<AppUser> FindAsync(Guid userId) =>
        await users.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("Không tìm thấy tài khoản.");

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new ValidationException(string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    private static User ToDomain(AppUser e) => new()
    {
        Id = e.Id,
        Email = e.Email ?? string.Empty,
        FullName = e.FullName,
        Phone = e.PhoneNumber,
        IsActive = e.IsActive,
        CreatedAtUtc = e.CreatedAtUtc
    };
}
