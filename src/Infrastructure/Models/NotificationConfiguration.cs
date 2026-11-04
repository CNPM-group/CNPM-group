using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("notifications");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("notification_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(x => x.OrderId)
            .HasColumnName("order_id");

        b.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Body)
            .HasColumnName("body")
            .HasColumnType("text")
            .IsRequired(false);

        b.Property(x => x.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

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
