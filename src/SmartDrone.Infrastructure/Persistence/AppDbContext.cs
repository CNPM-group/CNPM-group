using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartDrone.Application.Abstractions;
using SmartDrone.Domain.Entities;
using SmartDrone.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace SmartDrone.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options), IAppDbContext
{
    public DbSet<LandingStation> Stations => Set<LandingStation>();

    public DbSet<StationRoute> StationRoutes => Set<StationRoute>();

    public DbSet<Drone> Drones => Set<Drone>();

    public DbSet<Package> Packages => Set<Package>();

    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();

    public DbSet<OrderStatusHistory> OrderStatusHistories
    => Set<OrderStatusHistory>();

    public DbSet<DroneFlight> DroneFlights
    => Set<DroneFlight>();

    public DbSet<FlightTelemetry> FlightTelemetries
    => Set<FlightTelemetry>();

    public DbSet<DeliveryConfirmation> DeliveryConfirmations
    => Set<DeliveryConfirmation>();

    public DbSet<Notification> Notifications
    => Set<Notification>();

    public DbSet<AuditLog> AuditLogs
    => Set<AuditLog>();

    public DbSet<AiChatMessage> AiChatMessages
    => Set<AiChatMessage>();

    public DbSet<ApiKey> ApiKeys
    => Set<ApiKey>();

    public DbSet<StationOperator> StationOperators
    => Set<StationOperator>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        // Đặt tên các bảng ASP.NET Identity theo snake_case
        builder.Entity<AppRole>()
            .ToTable("asp_net_roles");

        builder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("asp_net_role_claims");

        builder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("asp_net_user_claims");

        builder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("asp_net_user_logins");

        builder.Entity<IdentityUserRole<Guid>>()
            .ToTable("asp_net_user_roles");

        builder.Entity<IdentityUserToken<Guid>>()
            .ToTable("asp_net_user_tokens");


        // Cấu hình bảng users theo schema v2
        builder.Entity<AppUser>(b =>
        {
            b.ToTable("users");

            b.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            b.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("ux_users_email");


            b.Property(x => x.Id)
                .HasColumnName("user_id");

            b.Property(x => x.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.PhoneNumber)
                .HasColumnName("phone")
                .HasMaxLength(20);

            b.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()")
                .IsRequired();

            b.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

        });

        // Áp dụng cấu hình các Entity nghiệp vụ
        builder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }

}
