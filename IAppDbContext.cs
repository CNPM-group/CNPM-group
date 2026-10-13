using Microsoft.EntityFrameworkCore;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Application.Abstractions;

/// <summary>Thêm DbSet mới vào đây mỗi khi có entity mới (Orders, Packages, Drones...).</summary>
public interface IAppDbContext
{
    DbSet<LandingStation> Stations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
