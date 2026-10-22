
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class OrderStatusHistoryConfiguration
    : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> b)
    {
        // Tên bảng
        b.ToTable("order_status_history");

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("history_id")
            .UseIdentityAlwaysColumn();

        // Mã đơn hàng
        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        // Trạng thái trước
        b.Property(x => x.FromStatus)
            .HasColumnName("from_status")
            .HasMaxLength(20)
            .IsRequired(false);

        // Trạng thái sau
        b.Property(x => x.ToStatus)
            .HasColumnName("to_status")
            .HasMaxLength(20)
            .IsRequired();

        // Người thực hiện thay đổi
        b.Property(x => x.ChangedBy)
            .HasColumnName("changed_by");

        // Ghi chú
        b.Property(x => x.Note)
            .HasColumnName("note")
            .HasColumnType("text")
            .IsRequired(false);

        // Thời gian thay đổi
        b.Property(x => x.ChangedAtUtc)
            .HasColumnName("changed_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Khóa ngoại tới đơn hàng
        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Khóa ngoại tới người dùng
        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.ChangedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Index theo đơn hàng và thời gian
        b.HasIndex(x => new
        {
            x.OrderId,
            x.ChangedAtUtc
        })
        .HasDatabaseName("ix_status_history_order");
    }
}
