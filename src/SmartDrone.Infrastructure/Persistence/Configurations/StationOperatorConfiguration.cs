
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;

namespace SmartDrone.Infrastructure.Persistence.Configurations;

public class StationOperatorConfiguration
    : IEntityTypeConfiguration<StationOperator>
{
    public void Configure(EntityTypeBuilder<StationOperator> b)
    {
        // Tên bảng PostgreSQL
        b.ToTable("station_operators");

        // Khóa chính kép
        b.HasKey(x => new
        {
            x.StationId,
            x.UserId
        });

        // Mã trạm đáp
        b.Property(x => x.StationId)
            .HasColumnName("station_id")
            .IsRequired();

        // Mã nhân viên vận hành
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // Khóa ngoại tới bảng landing_stations
        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Khóa ngoại tới người dùng ASP.NET Identity
        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index theo nhân viên vận hành
        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_station_operators_user");
    }
}
