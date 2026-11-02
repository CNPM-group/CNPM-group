using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class CustomerAddressConfiguration
    : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> b)
    {
        b.ToTable("customer_addresses", table =>
        {
            table.HasCheckConstraint(
                "ck_addr_lat",
                "latitude IS NULL OR latitude BETWEEN -90 AND 90");

            table.HasCheckConstraint(
                "ck_addr_lng",
                "longitude IS NULL OR longitude BETWEEN -180 AND 180");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("address_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        b.Property(x => x.Label)
            .HasColumnName("label")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.AddressLine)
            .HasColumnName("address_line")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6);

        b.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6);

        b.Property(x => x.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.CustomerId)
            .HasDatabaseName("ix_addresses_customer");

        b.HasIndex(x => x.CustomerId)
            .IsUnique()
            .HasFilter("is_default = true")
            .HasDatabaseName("ux_addresses_one_default");
    }
}
