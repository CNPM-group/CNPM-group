namespace SmartDrone.Domain.Models;

public interface IStationRepository
{
    Task<IReadOnlyList<LandingStation>> ListAsync(CancellationToken ct = default);
    Task<LandingStation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(LandingStation station, CancellationToken ct = default);
    Task UpdateAsync(LandingStation station, CancellationToken ct = default);
    Task DeleteAsync(LandingStation station, CancellationToken ct = default);
}
