using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class FlightTelemetryConfiguration
    : IEntityTypeConfiguration<FlightTelemetry>
{
    public void Configure(EntityTypeBuilder<FlightTelemetry> b)
    {
        b.ToTable("flight_telemetry", table =>
        {
            table.HasCheckConstraint(
                "ck_telemetry_battery",
                "battery_pct IS NULL OR battery_pct BETWEEN 0 AND 100");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("telemetry_id")
            .UseIdentityAlwaysColumn();

        b.Property(x => x.FlightId)
            .HasColumnName("flight_id")
            .IsRequired();

        b.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6)
            .IsRequired();

        b.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6)
            .IsRequired();

        b.Property(x => x.AltitudeM)
            .HasColumnName("altitude_m")
            .HasPrecision(6, 1)
            .IsRequired(false);

        b.Property(x => x.BatteryPct)
            .HasColumnName("battery_pct")
            .IsRequired(false);

        b.Property(x => x.RecordedAtUtc)
            .HasColumnName("recorded_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<DroneFlight>()
            .WithMany()
            .HasForeignKey(x => x.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new
        {
            x.FlightId,
            x.RecordedAtUtc
        })
        .IsDescending(false, true)
        .HasDatabaseName("ix_telemetry_flight_time");
    }
}
