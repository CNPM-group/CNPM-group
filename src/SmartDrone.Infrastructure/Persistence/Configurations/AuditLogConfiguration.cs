
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        // Tên bảng
        b.ToTable("audit_logs");

        // Khóa chính tự tăng
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("audit_id")
            .UseIdentityAlwaysColumn();

        // Người thực hiện hành động
        b.Property(x => x.UserId)
            .HasColumnName("user_id");

        // Hành động
        b.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        // Tên đối tượng được thao tác
        b.Property(x => x.EntityName)
            .HasColumnName("entity_name")
            .HasMaxLength(100)
            .IsRequired();

        // ID đối tượng
        b.Property(x => x.EntityId)
            .HasColumnName("entity_id")
            .HasMaxLength(64)
            .IsRequired(false);

        // Dữ liệu trước khi thay đổi
        b.Property(x => x.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("jsonb")
            .IsRequired(false);

        // Dữ liệu sau khi thay đổi
        b.Property(x => x.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("jsonb")
            .IsRequired(false);

        // Địa chỉ IP
        b.Property(x => x.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45)
            .IsRequired(false);

        // Thời điểm ghi log
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Khóa ngoại tới người dùng
        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index theo đối tượng
        b.HasIndex(x => new
        {
            x.EntityName,
            x.EntityId
        })
        .HasDatabaseName("ix_audit_entity");

        // Index theo người dùng
        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_audit_user");

        // Index theo thời gian
        b.HasIndex(x => x.CreatedAtUtc)
            .IsDescending()
            .HasDatabaseName("ix_audit_created");
    }
}
