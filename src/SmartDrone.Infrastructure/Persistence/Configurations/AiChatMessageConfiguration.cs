
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class AiChatMessageConfiguration
    : IEntityTypeConfiguration<AiChatMessage>
{
    public void Configure(EntityTypeBuilder<AiChatMessage> b)
    {
        // Tên bảng và ràng buộc
        b.ToTable("ai_chat_messages", table =>
        {
            table.HasCheckConstraint(
                "ck_chat_sender",
                "sender IN ('User', 'Assistant')");
        });

        // Khóa chính
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("message_id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Người dùng
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Phiên hội thoại
        b.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        // Người gửi: User hoặc Assistant
        b.Property(x => x.Sender)
            .HasColumnName("sender")
            .HasMaxLength(10)
            .IsRequired();

        // Nội dung tin nhắn
        b.Property(x => x.Content)
            .HasColumnName("content")
            .HasColumnType("text")
            .IsRequired();

        // Thời gian tạo
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

        // Index phục vụ truy vấn lịch sử hội thoại
        b.HasIndex(x => new
        {
            x.UserId,
            x.SessionId,
            x.CreatedAtUtc
        })
        .HasDatabaseName("ix_chat_session");
    }
}
