namespace SmartDrone.Domain.Models;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(Guid userId, string password);
    Task<User> CreateAsync(User user, string password, string role);
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
}
