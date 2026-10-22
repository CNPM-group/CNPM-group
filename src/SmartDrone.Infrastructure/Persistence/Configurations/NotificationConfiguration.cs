
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        // Tên bảng
        b.ToTable("notifications");

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("notification_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Người nhận thông báo
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Đơn hàng liên quan (có thể NULL)
        b.Property(x => x.OrderId)
            .HasColumnName("order_id");

        // Tiêu đề thông báo
        b.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(150)
            .IsRequired();

        // Nội dung thông báo
        b.Property(x => x.Body)
            .HasColumnName("body")
            .HasColumnType("text")
            .IsRequired(false);

        // Trạng thái đã đọc
        b.Property(x => x.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();

        // Ngày tạo
        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Khóa ngoại tới người dùng
        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Khóa ngoại tới đơn hàng
        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index theo người dùng, trạng thái đọc và thời gian
        b.HasIndex(x => new
        {
            x.UserId,
            x.IsRead,
            x.CreatedAtUtc
        })
        .IsDescending(false, false, true)
        .HasDatabaseName("ix_notifications_user");
    }
}
