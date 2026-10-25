
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class DroneConfiguration
    : IEntityTypeConfiguration<Drone>
{
    public void Configure(EntityTypeBuilder<Drone> b)
    {
        // Tên bảng và các ràng buộc
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

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("drone_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Mã Drone
        b.Property(x => x.DroneCode)
            .HasColumnName("drone_code")
            .HasMaxLength(30)
            .IsRequired();

        b.HasIndex(x => x.DroneCode)
            .IsUnique();

        // Model
        b.Property(x => x.Model)
            .HasColumnName("model")
            .HasMaxLength(100)
            .IsRequired();

        // Tải trọng tối đa
        b.Property(x => x.MaxPayloadKg)
            .HasColumnName("max_payload_kg")
            .HasPrecision(5, 2)
            .IsRequired();

        // Phạm vi bay tối đa
        b.Property(x => x.MaxRangeKm)
            .HasColumnName("max_range_km")
            .HasPrecision(6, 2)
            .IsRequired();

        // Tốc độ bay
        b.Property(x => x.CruiseSpeedKmh)
            .HasColumnName("cruise_speed_kmh")
            .HasPrecision(5, 1)
            .IsRequired();

        // Phần trăm pin
        b.Property(x => x.BatteryPct)
            .HasColumnName("battery_pct")
            .HasDefaultValue((short)100)
            .IsRequired();

        // Trạng thái Drone
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasDefaultValue("Idle")
            .IsRequired();

        // Trạm hiện tại
        b.Property(x => x.CurrentStationId)
            .HasColumnName("current_station_id");

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.CurrentStationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ngày tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Ngày cập nhật
        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Index
        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_drones_status");

        b.HasIndex(x => x.CurrentStationId)
            .HasDatabaseName("ix_drones_station");
    }
}
