namespace SmartDrone.Domain.Models;

public interface IDroneRepository
{
    Task<IReadOnlyList<Drone>> ListAsync(CancellationToken ct = default);
    Task<Drone?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string droneCode, Guid? exceptId, CancellationToken ct = default);
    Task<bool> AnyCanCarryAsync(decimal weightKg, CancellationToken ct = default);
    Task AddAsync(Drone drone, CancellationToken ct = default);
    Task UpdateAsync(Drone drone, CancellationToken ct = default);
    Task DeleteAsync(Drone drone, CancellationToken ct = default);
}
