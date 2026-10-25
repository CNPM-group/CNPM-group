
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class FlightTelemetryConfiguration
    : IEntityTypeConfiguration<FlightTelemetry>
{
    public void Configure(EntityTypeBuilder<FlightTelemetry> b)
    {
        // Tên bảng và ràng buộc
        b.ToTable("flight_telemetry", table =>
        {
            table.HasCheckConstraint(
                "ck_telemetry_battery",
                "battery_pct IS NULL OR battery_pct BETWEEN 0 AND 100");
        });

        // Khóa chính tự tăng
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("telemetry_id")
            .UseIdentityAlwaysColumn();

        // Khóa ngoại tới chuyến bay
        b.Property(x => x.FlightId)
            .HasColumnName("flight_id")
            .IsRequired();

        // Vĩ độ
        b.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6)
            .IsRequired();

        // Kinh độ
        b.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6)
            .IsRequired();

        // Độ cao (mét)
        b.Property(x => x.AltitudeM)
            .HasColumnName("altitude_m")
            .HasPrecision(6, 1)
            .IsRequired(false);

        // Phần trăm pin
        b.Property(x => x.BatteryPct)
            .HasColumnName("battery_pct")
            .IsRequired(false);

        // Thời gian ghi nhận
        b.Property(x => x.RecordedAtUtc)
            .HasColumnName("recorded_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Quan hệ với bảng drone_flights
        b.HasOne<DroneFlight>()
            .WithMany()
            .HasForeignKey(x => x.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index theo chuyến bay và thời gian mới nhất
        b.HasIndex(x => new
        {
            x.FlightId,
            x.RecordedAtUtc
        })
        .IsDescending(false, true)
        .HasDatabaseName("ix_telemetry_flight_time");
    }
}
