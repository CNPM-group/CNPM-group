
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class DroneFlightConfiguration
    : IEntityTypeConfiguration<DroneFlight>
{
    public void Configure(EntityTypeBuilder<DroneFlight> b)
    {
        // Tên bảng và các ràng buộc
        b.ToTable("drone_flights", table =>
        {
            table.HasCheckConstraint(
                "ck_flight_status",
                "flight_status IN ('Scheduled', 'Flying', 'Landed', 'Aborted')");

            table.HasCheckConstraint(
                "ck_flight_stations",
                "departure_station_id <> arrival_station_id");

            table.HasCheckConstraint(
                "ck_flight_leg",
                "leg_index > 0");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("flight_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Mã đơn hàng
        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        // Mã Drone
        b.Property(x => x.DroneId)
            .HasColumnName("drone_id")
            .IsRequired();

        // Thứ tự chặng bay
        b.Property(x => x.LegIndex)
            .HasColumnName("leg_index")
            .HasDefaultValue((short)1)
            .IsRequired();

        // Trạm xuất phát
        b.Property(x => x.DepartureStationId)
            .HasColumnName("departure_station_id")
            .IsRequired();

        // Trạm đích
        b.Property(x => x.ArrivalStationId)
            .HasColumnName("arrival_station_id")
            .IsRequired();

        // Thời gian dự kiến xuất phát
        b.Property(x => x.ScheduledDepartureAtUtc)
            .HasColumnName("scheduled_departure_at")
            .HasColumnType("timestamp with time zone");

        // Thời gian xuất phát thực tế
        b.Property(x => x.ActualDepartureAtUtc)
            .HasColumnName("actual_departure_at")
            .HasColumnType("timestamp with time zone");

        // Thời gian đến thực tế
        b.Property(x => x.ActualArrivalAtUtc)
            .HasColumnName("actual_arrival_at")
            .HasColumnType("timestamp with time zone");

        // Vị trí hiện tại
        b.Property(x => x.CurrentLatitude)
            .HasColumnName("current_latitude")
            .HasPrecision(9, 6);

        b.Property(x => x.CurrentLongitude)
            .HasColumnName("current_longitude")
            .HasPrecision(9, 6);

        // Trạng thái chuyến bay
        b.Property(x => x.FlightStatus)
            .HasColumnName("flight_status")
            .HasMaxLength(20)
            .HasDefaultValue("Scheduled")
            .IsRequired();

        // Ngày tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Quan hệ với đơn hàng
        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ với Drone
        b.HasOne<Drone>()
            .WithMany()
            .HasForeignKey(x => x.DroneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quan hệ với trạm xuất phát
        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.DepartureStationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quan hệ với trạm đích
        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.ArrivalStationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Mỗi đơn hàng không được trùng số thứ tự chặng
        b.HasAlternateKey(x => new
        {
            x.OrderId,
            x.LegIndex
        })
        .HasName("uq_flight_leg");

        // Các index theo schema v2
        b.HasIndex(x => x.OrderId)
            .HasDatabaseName("ix_flights_order");

        b.HasIndex(x => x.DroneId)
            .HasDatabaseName("ix_flights_drone");

        b.HasIndex(x => x.FlightStatus)
            .HasDatabaseName("ix_flights_status");

        b.HasIndex(x => x.DepartureStationId)
            .HasDatabaseName("ix_flights_departure");

        b.HasIndex(x => x.ArrivalStationId)
            .HasDatabaseName("ix_flights_arrival");
    }
}
