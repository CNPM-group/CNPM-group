
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class CustomerAddressConfiguration
    : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> b)
    {
        // Tên bảng và các ràng buộc
        b.ToTable("customer_addresses", table =>
        {
            table.HasCheckConstraint(
                "ck_addr_lat",
                "latitude IS NULL OR latitude BETWEEN -90 AND 90");

            table.HasCheckConstraint(
                "ck_addr_lng",
                "longitude IS NULL OR longitude BETWEEN -180 AND 180");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("address_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Khóa ngoại tới khách hàng
        b.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        // Nhãn địa chỉ
        b.Property(x => x.Label)
            .HasColumnName("label")
            .HasMaxLength(50)
            .IsRequired();

        // Địa chỉ chi tiết
        b.Property(x => x.AddressLine)
            .HasColumnName("address_line")
            .HasMaxLength(255)
            .IsRequired();

        // Tọa độ
        b.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6);

        b.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6);

        // Địa chỉ mặc định
        b.Property(x => x.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false)
            .IsRequired();

        // Ngày tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Liên kết với người dùng ASP.NET Identity
        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index theo khách hàng
        b.HasIndex(x => x.CustomerId)
            .HasDatabaseName("ix_addresses_customer");

        // Mỗi khách chỉ có tối đa 1 địa chỉ mặc định
        b.HasIndex(x => x.CustomerId)
            .IsUnique()
            .HasFilter("is_default = true")
            .HasDatabaseName("ux_addresses_one_default");
    }
}
