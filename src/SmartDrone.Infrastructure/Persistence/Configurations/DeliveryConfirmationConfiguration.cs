
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class DeliveryConfirmationConfiguration
    : IEntityTypeConfiguration<DeliveryConfirmation>
{
    public void Configure(EntityTypeBuilder<DeliveryConfirmation> b)
    {
        // Tên bảng và các ràng buộc
        b.ToTable("delivery_confirmations", table =>
        {
            table.HasCheckConstraint(
                "ck_confirm_method",
                "method IN ('OTP', 'QR')");

            table.HasCheckConstraint(
                "ck_confirm_attempts",
                "failed_attempts >= 0");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("confirmation_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Mã đơn hàng
        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        // Mã xác nhận đã được hash
        b.Property(x => x.CodeHash)
            .HasColumnName("code_hash")
            .HasMaxLength(255)
            .IsRequired();

        // Phương thức xác nhận
        b.Property(x => x.Method)
            .HasColumnName("method")
            .HasMaxLength(10)
            .HasDefaultValue("OTP")
            .IsRequired();

        // Thời gian hết hạn
        b.Property(x => x.ExpiresAtUtc)
            .HasColumnName("expires_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Số lần xác nhận thất bại
        b.Property(x => x.FailedAttempts)
            .HasColumnName("failed_attempts")
            .HasDefaultValue((short)0)
            .IsRequired();

        // Thời gian xác nhận thành công
        b.Property(x => x.VerifiedAtUtc)
            .HasColumnName("verified_at")
            .HasColumnType("timestamp with time zone");

        // Người xác nhận
        b.Property(x => x.VerifiedBy)
            .HasColumnName("verified_by");

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

        // Mỗi đơn hàng chỉ có tối đa 1 bản ghi xác nhận
        b.HasIndex(x => x.OrderId)
            .IsUnique();

        // Quan hệ với người dùng xác nhận
        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.VerifiedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
