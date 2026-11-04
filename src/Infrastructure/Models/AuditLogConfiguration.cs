using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("audit_logs");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("audit_id")
            .UseIdentityAlwaysColumn();

        b.Property(x => x.UserId)
            .HasColumnName("user_id");

        b.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.EntityName)
            .HasColumnName("entity_name")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.EntityId)
            .HasColumnName("entity_id")
            .HasMaxLength(64)
            .IsRequired(false);

        b.Property(x => x.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("jsonb")
            .IsRequired(false);

        b.Property(x => x.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("jsonb")
            .IsRequired(false);

        b.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45)
            .IsRequired(false);

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => new
        {
            x.EntityName,
            x.EntityId
        })
        .HasDatabaseName("ix_audit_entity");

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_audit_user");

        b.HasIndex(x => x.CreatedAtUtc)
            .IsDescending()
            .HasDatabaseName("ix_audit_created");
    }
}
