using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class DroneFlightConfiguration
    : IEntityTypeConfiguration<DroneFlight>
{
    public void Configure(EntityTypeBuilder<DroneFlight> b)
    {
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

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("flight_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        b.Property(x => x.DroneId)
            .HasColumnName("drone_id")
            .IsRequired();

        b.Property(x => x.LegIndex)
            .HasColumnName("leg_index")
            .HasDefaultValue((short)1)
            .IsRequired();

        b.Property(x => x.DepartureStationId)
            .HasColumnName("departure_station_id")
            .IsRequired();

        b.Property(x => x.ArrivalStationId)
            .HasColumnName("arrival_station_id")
            .IsRequired();

        b.Property(x => x.ScheduledDepartureAtUtc)
            .HasColumnName("scheduled_departure_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.ActualDepartureAtUtc)
            .HasColumnName("actual_departure_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.ActualArrivalAtUtc)
            .HasColumnName("actual_arrival_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.CurrentLatitude)
            .HasColumnName("current_latitude")
            .HasPrecision(9, 6);

        b.Property(x => x.CurrentLongitude)
            .HasColumnName("current_longitude")
            .HasPrecision(9, 6);

        b.Property(x => x.FlightStatus)
            .HasColumnName("flight_status")
            .HasMaxLength(20)
            .HasDefaultValue("Scheduled")
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<Drone>()
            .WithMany()
            .HasForeignKey(x => x.DroneId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.DepartureStationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.ArrivalStationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasAlternateKey(x => new
        {
            x.OrderId,
            x.LegIndex
        })
        .HasName("uq_flight_leg");

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
