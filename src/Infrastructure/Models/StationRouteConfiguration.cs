using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class StationRouteConfiguration
    : IEntityTypeConfiguration<StationRoute>
{
    public void Configure(EntityTypeBuilder<StationRoute> b)
    {
        b.ToTable("station_routes", table =>
        {
            table.HasCheckConstraint(
                "ck_route_diff",
                "from_station_id <> to_station_id");

            table.HasCheckConstraint(
                "ck_route_dist",
                "distance_km > 0");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("route_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.FromStationId)
            .HasColumnName("from_station_id")
            .IsRequired();

        b.Property(x => x.ToStationId)
            .HasColumnName("to_station_id")
            .IsRequired();

        b.Property(x => x.DistanceKm)
            .HasColumnName("distance_km")
            .HasPrecision(8, 3)
            .IsRequired();

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        b.HasAlternateKey(x => new
        {
            x.FromStationId,
            x.ToStationId
        })
        .HasName("uq_route_pair");

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.FromStationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.ToStationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.ToStationId)
            .HasDatabaseName("ix_routes_to");
    }
}
