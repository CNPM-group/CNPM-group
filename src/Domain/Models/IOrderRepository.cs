namespace SmartDrone.Domain.Models;

public interface IOrderRepository
{
    Task<DeliveryOrder?> GetAsync(Guid id, Guid? customerId, CancellationToken ct = default);
    Task<Package?> GetPackageAsync(Guid packageId, CancellationToken ct = default);
    // search: tìm theo mã đơn (không phân biệt hoa thường), null = không lọc.
    Task<PagedResult<OrderSummary>> PageAsync(Guid? customerId, string? status, string? search, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyDictionary<Guid, CustomerOrderStats>> StatsByCustomerAsync(CancellationToken ct = default);
    // Số đơn đang đi tới (Scheduled, InTransit) và đang nằm chờ khách (ArrivedAtStation) ở trạm nhận.
    Task<int> CountIncomingAsync(Guid destinationStationId, Guid exceptOrderId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderStatusHistory>> GetHistoryAsync(Guid orderId, CancellationToken ct = default);
    Task AddAsync(DeliveryOrder order, Package package, OrderStatusHistory history, CancellationToken ct = default);
    Task UpdateAsync(DeliveryOrder order, OrderStatusHistory? history = null, CancellationToken ct = default);
}
