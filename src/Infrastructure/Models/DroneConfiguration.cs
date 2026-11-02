using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class DroneConfiguration
    : IEntityTypeConfiguration<Drone>
{
    public void Configure(EntityTypeBuilder<Drone> b)
    {
        b.ToTable("drones", table =>
        {
            table.HasCheckConstraint(
                "ck_drone_status",
                "status IN ('Idle', 'Assigned', 'InFlight', 'Charging', 'Maintenance')");

            table.HasCheckConstraint(
                "ck_drone_battery",
                "battery_pct BETWEEN 0 AND 100");

            table.HasCheckConstraint(
                "ck_drone_payload",
                "max_payload_kg > 0");

            table.HasCheckConstraint(
                "ck_drone_range",
                "max_range_km > 0");

            table.HasCheckConstraint(
                "ck_drone_speed",
                "cruise_speed_kmh > 0");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("drone_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.DroneCode)
            .HasColumnName("drone_code")
            .HasMaxLength(30)
            .IsRequired();

        b.HasIndex(x => x.DroneCode)
            .IsUnique();

        b.Property(x => x.Model)
            .HasColumnName("model")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.MaxPayloadKg)
            .HasColumnName("max_payload_kg")
            .HasPrecision(5, 2)
            .IsRequired();

        b.Property(x => x.MaxRangeKm)
            .HasColumnName("max_range_km")
            .HasPrecision(6, 2)
            .IsRequired();

        b.Property(x => x.CruiseSpeedKmh)
            .HasColumnName("cruise_speed_kmh")
            .HasPrecision(5, 1)
            .IsRequired();

        b.Property(x => x.BatteryPct)
            .HasColumnName("battery_pct")
            .HasDefaultValue((short)100)
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasDefaultValue("Idle")
            .IsRequired();

        b.Property(x => x.CurrentStationId)
            .HasColumnName("current_station_id");

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.CurrentStationId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_drones_status");

        b.HasIndex(x => x.CurrentStationId)
            .HasDatabaseName("ix_drones_station");
    }
}
