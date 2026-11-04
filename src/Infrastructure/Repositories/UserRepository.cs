using Microsoft.AspNetCore.Identity;
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
