namespace SmartDrone.Application.Stations;

public interface IStationService
{
    Task<IReadOnlyList<StationDto>> GetAllAsync(CancellationToken ct = default);
    Task<StationDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<StationDto> CreateAsync(StationRequest request, CancellationToken ct = default);
    Task<StationDto?> UpdateAsync(Guid id, StationRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
