namespace SmartDrone.Domain.Models;

public interface IDroneRepository
{
    Task<bool> AnyCanCarryAsync(decimal weightKg, CancellationToken ct = default);
}
