using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class DeliveryConfirmationConfiguration
    : IEntityTypeConfiguration<DeliveryConfirmation>
{
    public void Configure(EntityTypeBuilder<DeliveryConfirmation> b)
    {
        b.ToTable("delivery_confirmations", table =>
        {
            table.HasCheckConstraint(
                "ck_confirm_method",
                "method IN ('OTP', 'QR')");

            table.HasCheckConstraint(
                "ck_confirm_attempts",
                "failed_attempts >= 0");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("confirmation_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        b.Property(x => x.CodeHash)
            .HasColumnName("code_hash")
            .HasMaxLength(255)
            .IsRequired();

        b.Property(x => x.Method)
            .HasColumnName("method")
            .HasMaxLength(10)
            .HasDefaultValue("OTP")
            .IsRequired();

        b.Property(x => x.ExpiresAtUtc)
            .HasColumnName("expires_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        b.Property(x => x.FailedAttempts)
            .HasColumnName("failed_attempts")
            .HasDefaultValue((short)0)
            .IsRequired();

        b.Property(x => x.VerifiedAtUtc)
            .HasColumnName("verified_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.VerifiedBy)
            .HasColumnName("verified_by");

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.HasOne<DeliveryOrder>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.OrderId)
            .IsUnique();

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.VerifiedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
