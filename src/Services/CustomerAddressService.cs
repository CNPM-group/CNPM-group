using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class CustomerAddressService(ICustomerAddressRepository addresses)
{
    public Task<IReadOnlyList<CustomerAddress>> ListAsync(Guid customerId, CancellationToken ct = default) =>
        addresses.ListByCustomerAsync(customerId, ct);

    public async Task<CustomerAddress> GetAsync(Guid customerId, Guid id, CancellationToken ct = default) =>
        await addresses.GetAsync(id, customerId, ct) ?? throw new NotFoundException("Không tìm thấy địa chỉ.");

    public async Task<CustomerAddress> CreateAsync(Guid customerId, CustomerAddress input, CancellationToken ct = default)
    {
        if (input.IsDefault && await addresses.HasOtherDefaultAsync(customerId, null, ct))
            throw new ConflictException("Bạn đã có địa chỉ mặc định.");

        var address = new CustomerAddress
        {
            CustomerId = customerId,
            Label = input.Label.Trim(),
            AddressLine = input.AddressLine.Trim(),
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            IsDefault = input.IsDefault
        };

        await addresses.AddAsync(address, ct);
        return address;
    }

    public async Task<CustomerAddress> UpdateAsync(Guid customerId, Guid id, CustomerAddress input, CancellationToken ct = default)
    {
        var address = await GetAsync(customerId, id, ct);

        if (input.IsDefault && await addresses.HasOtherDefaultAsync(customerId, id, ct))
            throw new ConflictException("Bạn đã có địa chỉ mặc định khác.");

        address.Label = input.Label.Trim();
        address.AddressLine = input.AddressLine.Trim();
        address.Latitude = input.Latitude;
        address.Longitude = input.Longitude;
        address.IsDefault = input.IsDefault;

        await addresses.UpdateAsync(address, ct);
        return address;
    }

    public async Task DeleteAsync(Guid customerId, Guid id, CancellationToken ct = default)
    {
        var address = await GetAsync(customerId, id, ct);
        await addresses.DeleteAsync(address, ct);
    }
}
