using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Models;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone.Infrastructure.Repositories;

public sealed class CustomerAddressRepository(AppDbContext db) : ICustomerAddressRepository
{
    public async Task<IReadOnlyList<CustomerAddress>> ListByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
        await db.CustomerAddresses.AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync(ct);

    public Task<CustomerAddress?> GetAsync(Guid id, Guid customerId, CancellationToken ct = default) =>
        db.CustomerAddresses.FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId, ct);

    public Task<bool> BelongsToAsync(Guid id, Guid customerId, CancellationToken ct = default) =>
        db.CustomerAddresses.AnyAsync(a => a.Id == id && a.CustomerId == customerId, ct);

    public Task<bool> HasOtherDefaultAsync(Guid customerId, Guid? exceptId, CancellationToken ct = default) =>
        db.CustomerAddresses.AnyAsync(a => a.CustomerId == customerId && a.IsDefault && a.Id != exceptId, ct);

    public async Task AddAsync(CustomerAddress address, CancellationToken ct = default)
    {
        db.CustomerAddresses.Add(address);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CustomerAddress address, CancellationToken ct = default)
    {
        if (db.Entry(address).State == EntityState.Detached)
            db.CustomerAddresses.Update(address);

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(CustomerAddress address, CancellationToken ct = default)
    {
        db.CustomerAddresses.Remove(address);
        await db.SaveChangesAsync(ct);
    }
}
