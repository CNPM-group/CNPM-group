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
}
