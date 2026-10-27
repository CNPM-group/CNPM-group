using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Application.Abstractions;

/// <summary>
—</summary>
public interface IAppDbContext
{
    DbSet<LandingStation> Stations { get; }
    DbSet<DeliveryOrder> DeliveryOrders { get; }
    DbSet<Package> Packages { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
