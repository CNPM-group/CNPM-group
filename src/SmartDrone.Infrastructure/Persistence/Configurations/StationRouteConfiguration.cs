
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class StationRouteConfiguration
    : IEntityTypeConfiguration<StationRoute>
{
    public void Configure(EntityTypeBuilder<StationRoute> b)
    {
        // Tên bảng và các ràng buộc
        b.ToTable("station_routes", table =>
        {
            table.HasCheckConstraint(
                "ck_route_diff",
                "from_station_id <> to_station_id");

            table.HasCheckConstraint(
                "ck_route_dist",
                "distance_km > 0");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("route_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Trạm xuất phát
        b.Property(x => x.FromStationId)
            .HasColumnName("from_station_id")
            .IsRequired();

        // Trạm đích
        b.Property(x => x.ToStationId)
            .HasColumnName("to_station_id")
            .IsRequired();

        // Khoảng cách (km)
        b.Property(x => x.DistanceKm)
            .HasColumnName("distance_km")
            .HasPrecision(8, 3)
            .IsRequired();

        // Trạng thái hoạt động
        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // Mỗi cặp trạm chỉ có một tuyến có hướng
        b.HasAlternateKey(x => new
        {
            x.FromStationId,
            x.ToStationId
        })
        .HasName("uq_route_pair");

        // Khóa ngoại: Trạm xuất phát
        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.FromStationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Khóa ngoại: Trạm đích
        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.ToStationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index hỗ trợ truy vấn theo trạm đích
        b.HasIndex(x => x.ToStationId)
            .HasDatabaseName("ix_routes_to");
    }
}
