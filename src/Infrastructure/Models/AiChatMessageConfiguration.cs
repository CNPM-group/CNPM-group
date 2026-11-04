using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class AiChatMessageConfiguration
    : IEntityTypeConfiguration<AiChatMessage>
{
    public void Configure(EntityTypeBuilder<AiChatMessage> b)
    {
        b.ToTable("ai_chat_messages", table =>
        {
            table.HasCheckConstraint(
                "ck_chat_sender",
                "sender IN ('User', 'Assistant')");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("message_id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        b.Property(x => x.Sender)
            .HasColumnName("sender")
            .HasMaxLength(10)
            .IsRequired();

        b.Property(x => x.Content)
            .HasColumnName("content")
            .HasColumnType("text")
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

        b.HasIndex(x => new
        {
            x.UserId,
            x.SessionId,
            x.CreatedAtUtc
        })
        .HasDatabaseName("ix_chat_session");
    }
}
