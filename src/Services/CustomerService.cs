using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

// FR03: nhân viên xem danh sách khách, chi tiết 1 khách (địa chỉ + đơn gần đây), sửa họ tên / số điện thoại.
public sealed class CustomerService(
    IUserRepository users,
    IOrderRepository orders,
    ICustomerAddressRepository addresses)
{
    private const int RecentOrders = 20;

    public async Task<IReadOnlyList<CustomerSummary>> ListAsync(string? search, CancellationToken ct = default)
    {
        var customers = await users.ListInRoleAsync(Roles.Customer);
        var stats = await orders.StatsByCustomerAsync(ct);

        var keyword = search?.Trim();
        return customers
            .Where(u => string.IsNullOrEmpty(keyword) ||
                        u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        u.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (u.Phone ?? "").Contains(keyword))
            .Select(u => new CustomerSummary(u, stats.GetValueOrDefault(u.Id) ?? CustomerOrderStats.Empty))
            .ToList();
    }

    public async Task<CustomerDetail> GetAsync(Guid customerId, CancellationToken ct = default)
    {
        var user = await GetCustomerAsync(customerId);
        var stats = (await orders.StatsByCustomerAsync(ct)).GetValueOrDefault(customerId) ?? CustomerOrderStats.Empty;
        var list = await addresses.ListByCustomerAsync(customerId, ct);
        var recent = await orders.PageAsync(customerId, null, null, 1, RecentOrders, ct);
        return new CustomerDetail(user, stats, list, recent.Items);
    }

    public async Task<User> UpdateAsync(Guid customerId, string fullName, string? phone, CancellationToken ct = default)
    {
        await GetCustomerAsync(customerId);
        return await users.UpdateProfileAsync(customerId, ProfileRules.Name(fullName), ProfileRules.Phone(phone));
    }

    // Chỉ tài khoản có vai trò Customer; id của nhân viên → 404 để màn này không thành chỗ sửa tài khoản nội bộ.
    private async Task<User> GetCustomerAsync(Guid customerId)
    {
        var user = await users.GetByIdAsync(customerId);
        if (user is null || !(await users.GetRolesAsync(customerId)).Contains(Roles.Customer))
            throw new NotFoundException("Không tìm thấy khách hàng.");
        return user;
    }
}
