namespace SmartDrone.Domain.Models;

// Số đơn của 1 khách (v0.11). Active = đơn chưa kết thúc (chưa Completed, Cancelled, Rejected).
public sealed record CustomerOrderStats(int Total, int Completed, int Active, DateTime? LastOrderAtUtc)
{
    public static readonly CustomerOrderStats Empty = new(0, 0, 0, null);
}

public sealed record CustomerSummary(User User, CustomerOrderStats Stats);

public sealed record CustomerDetail(
    User User,
    CustomerOrderStats Stats,
    IReadOnlyList<CustomerAddress> Addresses,
    IReadOnlyList<OrderSummary> RecentOrders);
