using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class DeliveryOrderConfiguration
    : IEntityTypeConfiguration<DeliveryOrder>
{
    public void Configure(EntityTypeBuilder<DeliveryOrder> b)
    {
        b.ToTable("delivery_orders", table =>
        {
            table.HasCheckConstraint(
                "ck_order_status",
                "status IN ('Pending', 'Approved', 'Rejected', " +
                "'Scheduled', 'InTransit', 'ArrivedAtStation', " +
                "'Completed', 'Failed', 'Cancelled')");

            table.HasCheckConstraint(
                "ck_order_stations",
                "origin_station_id <> destination_station_id");

            table.HasCheckConstraint(
                "ck_order_failure",
                "status <> 'Failed' OR failure_reason IS NOT NULL");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("order_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.TrackingCode)
            .HasColumnName("tracking_code")
            .HasMaxLength(20)
            .HasDefaultValueSql(
                "('DR' || upper(substr(md5(gen_random_uuid()::text), 1, 8)))")
            .IsRequired();

        b.HasIndex(x => x.TrackingCode)
            .IsUnique();

        b.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        b.Property(x => x.PackageId)
            .HasColumnName("package_id")
            .IsRequired();

        b.Property(x => x.OriginStationId)
            .HasColumnName("origin_station_id")
            .IsRequired();

        b.Property(x => x.DestinationStationId)
            .HasColumnName("destination_station_id")
            .IsRequired();

        b.Property(x => x.DeliveryAddressId)
            .HasColumnName("delivery_address_id");

        b.Property(x => x.ApprovedBy)
            .HasColumnName("approved_by");

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasDefaultValue("Pending")
            .IsRequired();

        b.Property(x => x.ApprovedAtUtc)
            .HasColumnName("approved_at");

        b.Property(x => x.ScheduledAtUtc)
            .HasColumnName("scheduled_at");

        b.Property(x => x.EtaUtc)
            .HasColumnName("eta");

        b.Property(x => x.CompletedAtUtc)
            .HasColumnName("completed_at");

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

        b.Property(x => x.FailureReason)
            .HasColumnName("failure_reason")
            .HasColumnType("text");

        b.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne<Package>()
            .WithMany()
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.PackageId)
            .IsUnique();

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.OriginStationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.DestinationStationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<CustomerAddress>()
            .WithMany()
            .HasForeignKey(x => x.DeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => new
        {
            x.CustomerId,
            x.CreatedAtUtc
        })
        .IsDescending(false, true)
        .HasDatabaseName("ix_orders_customer");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_orders_status");

        b.HasIndex(x => x.CreatedAtUtc)
            .IsDescending()
            .HasDatabaseName("ix_orders_created");

        b.HasIndex(x => x.OriginStationId)
            .HasDatabaseName("ix_orders_origin");

        b.HasIndex(x => x.DestinationStationId)
            .HasDatabaseName("ix_orders_destination");
    }
}
