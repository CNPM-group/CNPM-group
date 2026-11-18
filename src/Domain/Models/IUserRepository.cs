namespace SmartDrone.Domain.Models;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task<IReadOnlyList<User>> ListAsync();
    Task<bool> CheckPasswordAsync(Guid userId, string password);
    Task<User> CreateAsync(User user, string password, string role);
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
    Task SetRolesAsync(Guid userId, IReadOnlyCollection<string> roles);
    Task SetActiveAsync(Guid userId, bool isActive);
    Task<IReadOnlyList<User>> ListInRoleAsync(string role);
    Task<User> UpdateProfileAsync(Guid userId, string fullName, string? phone);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}
