namespace SmartDrone.Domain.Models;

public interface IOrderRepository
{
    Task<DeliveryOrder?> GetAsync(Guid id, Guid? customerId, CancellationToken ct = default);
    Task<Package?> GetPackageAsync(Guid packageId, CancellationToken ct = default);
    Task<PagedResult<OrderSummary>> PageAsync(Guid? customerId, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<OrderStatusHistory>> GetHistoryAsync(Guid orderId, CancellationToken ct = default);
    Task AddAsync(DeliveryOrder order, Package package, OrderStatusHistory history, CancellationToken ct = default);
    Task UpdateAsync(DeliveryOrder order, OrderStatusHistory? history = null, CancellationToken ct = default);
}
