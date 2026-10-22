
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Domain.Enums;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class LandingStationConfiguration
    : IEntityTypeConfiguration<LandingStation>
{
    public void Configure(EntityTypeBuilder<LandingStation> b)
    {
        // Tên bảng PostgreSQL
        b.ToTable("landing_stations", table =>
        {
            table.HasCheckConstraint(
                "ck_station_status",
                "status IN ('Available', 'Occupied', 'Maintenance')");

            table.HasCheckConstraint(
                "ck_station_lat",
                "latitude BETWEEN -90 AND 90");

            table.HasCheckConstraint(
                "ck_station_lng",
                "longitude BETWEEN -180 AND 180");

            table.HasCheckConstraint(
                "ck_station_cap",
                "capacity > 0");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("station_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Tên trạm
        b.Property(x => x.Name)
            .HasColumnName("station_name")
            .HasMaxLength(100)
            .IsRequired();

        b.HasIndex(x => x.Name)
            .IsUnique();

        // Tọa độ
        b.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6)
            .IsRequired();

        b.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6)
            .IsRequired();

        // Sức chứa
        b.Property(x => x.Capacity)
            .HasColumnName("capacity")
            .HasDefaultValue((short)2)
            .IsRequired();

        // Trạng thái
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(StationStatus.Available)
            .IsRequired();

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_stations_status");

        // Ngày tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}
