
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class PackageConfiguration
    : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> b)
    {
        // Tên bảng và ràng buộc
        b.ToTable("packages", table =>
        {
            table.HasCheckConstraint(
                "ck_pkg_weight",
                "weight_kg > 0");

            table.HasCheckConstraint(
                "ck_pkg_dims",
                "length_cm > 0 AND width_cm > 0 AND height_cm > 0");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("package_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Mô tả kiện hàng
        b.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(255)
            .IsRequired(false);

        // Trọng lượng kiện hàng
        b.Property(x => x.WeightKg)
            .HasColumnName("weight_kg")
            .HasPrecision(6, 2)
            .IsRequired();

        // Chiều dài
        b.Property(x => x.LengthCm)
            .HasColumnName("length_cm")
            .HasPrecision(6, 1)
            .IsRequired();

        // Chiều rộng
        b.Property(x => x.WidthCm)
            .HasColumnName("width_cm")
            .HasPrecision(6, 1)
            .IsRequired();

        // Chiều cao
        b.Property(x => x.HeightCm)
            .HasColumnName("height_cm")
            .HasPrecision(6, 1)
            .IsRequired();

        // Khóa ảnh lưu trong MinIO
        b.Property(x => x.ImageKey)
            .HasColumnName("image_key")
            .HasMaxLength(255)
            .IsRequired(false);

        // Ngày tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}
