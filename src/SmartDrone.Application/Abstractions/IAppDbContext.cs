using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<LandingStation> Stations { get; }
    DbSet<DeliveryOrder> DeliveryOrders { get; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; }
    DbSet<Package> Packages { get; }
    DbSet<Drone> Drones { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
