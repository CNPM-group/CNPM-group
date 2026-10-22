
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration
    : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> b)
    {
        // Tên bảng PostgreSQL
        b.ToTable("api_keys");

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("api_key_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Tên khóa API
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // Giá trị hash của API key
        b.Property(x => x.KeyHash)
            .HasColumnName("key_hash")
            .HasMaxLength(255)
            .IsRequired();

        // Không cho phép trùng key_hash
        b.HasIndex(x => x.KeyHash)
            .IsUnique();

        // Trạng thái hoạt động
        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // Thời điểm tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Thời điểm sử dụng gần nhất
        b.Property(x => x.LastUsedAtUtc)
            .HasColumnName("last_used_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}
