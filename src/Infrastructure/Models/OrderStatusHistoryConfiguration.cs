using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class OrderStatusHistoryConfiguration
    : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> b)
    {
        b.ToTable("order_status_history");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("history_id")
            .UseIdentityAlwaysColumn();

        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        b.Property(x => x.FromStatus)
            .HasColumnName("from_status")
            .HasMaxLength(20)
            .IsRequired(false);

        b.Property(x => x.ToStatus)
            .HasColumnName("to_status")
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.ChangedBy)
            .HasColumnName("changed_by");

        b.Property(x => x.Note)
            .HasColumnName("note")
            .HasColumnType("text")
            .IsRequired(false);

        b.Property(x => x.ChangedAtUtc)
            .HasColumnName("changed_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.ChangedBy)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => new
        {
            x.OrderId,
            x.ChangedAtUtc
        })
        .HasDatabaseName("ix_status_history_order");
    }
}
