using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class ApiKeyConfiguration
    : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> b)
    {
        b.ToTable("api_keys");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("api_key_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.KeyHash)
            .HasColumnName("key_hash")
            .HasMaxLength(255)
            .IsRequired();

        b.HasIndex(x => x.KeyHash)
            .IsUnique();

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.Property(x => x.LastUsedAtUtc)
            .HasColumnName("last_used_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}
