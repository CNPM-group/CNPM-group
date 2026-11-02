using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Models;

public class StationOperatorConfiguration
    : IEntityTypeConfiguration<StationOperator>
{
    public void Configure(EntityTypeBuilder<StationOperator> b)
    {
        b.ToTable("station_operators");

        b.HasKey(x => new
        {
            x.StationId,
            x.UserId
        });

        b.Property(x => x.StationId)
            .HasColumnName("station_id")
            .IsRequired();

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.HasOne<LandingStation>()
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_station_operators_user");
    }
}
