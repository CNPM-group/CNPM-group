namespace SmartDrone.Domain.Models;

public interface ICustomerAddressRepository
{
    Task<IReadOnlyList<CustomerAddress>> ListByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<CustomerAddress?> GetAsync(Guid id, Guid customerId, CancellationToken ct = default);
    Task<bool> BelongsToAsync(Guid id, Guid customerId, CancellationToken ct = default);
    Task<bool> HasOtherDefaultAsync(Guid customerId, Guid? exceptId, CancellationToken ct = default);
    Task AddAsync(CustomerAddress address, CancellationToken ct = default);
    Task UpdateAsync(CustomerAddress address, CancellationToken ct = default);
    Task DeleteAsync(CustomerAddress address, CancellationToken ct = default);
}
