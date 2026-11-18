using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

// v0.11: thông tin khách cho nhân viên (FR03), kèm số đơn để biết khách nào hay đặt.
public sealed record CustomerResponse(
    Guid Id,
    string Email,
    string FullName,
    string? Phone,
    bool IsActive,
    DateTime CreatedAtUtc,
    int TotalOrders,
    int CompletedOrders,
    int ActiveOrders,
    DateTime? LastOrderAtUtc)
{
    public static CustomerResponse From(CustomerSummary c) => new(
        c.User.Id, c.User.Email, c.User.FullName, c.User.Phone, c.User.IsActive, c.User.CreatedAtUtc,
        c.Stats.Total, c.Stats.Completed, c.Stats.Active, c.Stats.LastOrderAtUtc);
}

public sealed record CustomerDetailResponse(
    CustomerResponse Customer,
    IReadOnlyList<CustomerAddressResponse> Addresses,
    IReadOnlyList<OrderSummaryResponse> RecentOrders)
{
    public static CustomerDetailResponse From(CustomerDetail d) => new(
        CustomerResponse.From(new CustomerSummary(d.User, d.Stats)),
        d.Addresses.Select(CustomerAddressResponse.From).ToList(),
        d.RecentOrders.Select(OrderSummaryResponse.From).ToList());
}
