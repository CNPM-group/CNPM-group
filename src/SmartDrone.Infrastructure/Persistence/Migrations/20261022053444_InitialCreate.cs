using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartDrone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    api_key_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    key_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_keys", x => x.api_key_id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "landing_stations",
                columns: table => new
                {
                    station_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    station_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Available"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_landing_stations", x => x.station_id);
                    table.CheckConstraint("ck_station_cap", "capacity > 0");
                    table.CheckConstraint("ck_station_lat", "latitude BETWEEN -90 AND 90");
                    table.CheckConstraint("ck_station_lng", "longitude BETWEEN -180 AND 180");
                    table.CheckConstraint("ck_station_status", "status IN ('Available', 'Occupied', 'Maintenance')");
                });

            migrationBuilder.CreateTable(
                name: "packages",
                columns: table => new
                {
                    package_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    weight_kg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    length_cm = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: false),
                    width_cm = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: false),
                    height_cm = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: false),
                    image_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_packages", x => x.package_id);
                    table.CheckConstraint("ck_pkg_dims", "length_cm > 0 AND width_cm > 0 AND height_cm > 0");
                    table.CheckConstraint("ck_pkg_weight", "weight_kg > 0");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "drones",
                columns: table => new
                {
                    drone_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    drone_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    max_payload_kg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    max_range_km = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    cruise_speed_kmh = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false),
                    battery_pct = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)100),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Idle"),
                    current_station_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drones", x => x.drone_id);
                    table.CheckConstraint("ck_drone_battery", "battery_pct BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_drone_payload", "max_payload_kg > 0");
                    table.CheckConstraint("ck_drone_range", "max_range_km > 0");
                    table.CheckConstraint("ck_drone_speed", "cruise_speed_kmh > 0");
                    table.CheckConstraint("ck_drone_status", "status IN ('Idle', 'Assigned', 'InFlight', 'Charging', 'Maintenance')");
                    table.ForeignKey(
                        name: "fk_drones_stations_current_station_id",
                        column: x => x.current_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "station_routes",
                columns: table => new
                {
                    route_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    from_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    distance_km = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_routes", x => x.route_id);
                    table.UniqueConstraint("uq_route_pair", x => new { x.from_station_id, x.to_station_id });
                    table.CheckConstraint("ck_route_diff", "from_station_id <> to_station_id");
                    table.CheckConstraint("ck_route_dist", "distance_km > 0");
                    table.ForeignKey(
                        name: "fk_station_routes_landing_stations_from_station_id",
                        column: x => x.from_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_station_routes_landing_stations_to_station_id",
                        column: x => x.to_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ai_chat_messages",
                columns: table => new
                {
                    message_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_chat_messages", x => x.message_id);
                    table.CheckConstraint("ck_chat_sender", "sender IN ('User', 'Assistant')");
                    table.ForeignKey(
                        name: "fk_ai_chat_messages_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    audit_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.audit_id);
                    table.ForeignKey(
                        name: "fk_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "customer_addresses",
                columns: table => new
                {
                    address_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address_line = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_addresses", x => x.address_id);
                    table.CheckConstraint("ck_addr_lat", "latitude IS NULL OR latitude BETWEEN -90 AND 90");
                    table.CheckConstraint("ck_addr_lng", "longitude IS NULL OR longitude BETWEEN -180 AND 180");
                    table.ForeignKey(
                        name: "fk_customer_addresses_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "station_operators",
                columns: table => new
                {
                    station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_operators", x => new { x.station_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_station_operators_landing_stations_station_id",
                        column: x => x.station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_station_operators_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "delivery_orders",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tracking_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "('DR' || upper(substr(md5(gen_random_uuid()::text), 1, 8)))"),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origin_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    eta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failure_reason = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_orders", x => x.order_id);
                    table.CheckConstraint("ck_order_failure", "status <> 'Failed' OR failure_reason IS NOT NULL");
                    table.CheckConstraint("ck_order_stations", "origin_station_id <> destination_station_id");
                    table.CheckConstraint("ck_order_status", "status IN ('Pending', 'Approved', 'Rejected', 'Scheduled', 'InTransit', 'ArrivedAtStation', 'Completed', 'Failed', 'Cancelled')");
                    table.ForeignKey(
                        name: "fk_delivery_orders_customer_addresses_delivery_address_id",
                        column: x => x.delivery_address_id,
                        principalTable: "customer_addresses",
                        principalColumn: "address_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_delivery_orders_packages_package_id",
                        column: x => x.package_id,
                        principalTable: "packages",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_delivery_orders_stations_destination_station_id",
                        column: x => x.destination_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_delivery_orders_stations_origin_station_id",
                        column: x => x.origin_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_delivery_orders_users_approved_by",
                        column: x => x.approved_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_delivery_orders_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "delivery_confirmations",
                columns: table => new
                {
                    confirmation_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "OTP"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    failed_attempts = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    verified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_confirmations", x => x.confirmation_id);
                    table.CheckConstraint("ck_confirm_attempts", "failed_attempts >= 0");
                    table.CheckConstraint("ck_confirm_method", "method IN ('OTP', 'QR')");
                    table.ForeignKey(
                        name: "fk_delivery_confirmations_delivery_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "delivery_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_delivery_confirmations_users_verified_by",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "drone_flights",
                columns: table => new
                {
                    flight_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    drone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    leg_index = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    departure_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    arrival_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_departure_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_departure_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_arrival_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    current_latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    current_longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    flight_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Scheduled"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drone_flights", x => x.flight_id);
                    table.UniqueConstraint("uq_flight_leg", x => new { x.order_id, x.leg_index });
                    table.CheckConstraint("ck_flight_leg", "leg_index > 0");
                    table.CheckConstraint("ck_flight_stations", "departure_station_id <> arrival_station_id");
                    table.CheckConstraint("ck_flight_status", "flight_status IN ('Scheduled', 'Flying', 'Landed', 'Aborted')");
                    table.ForeignKey(
                        name: "fk_drone_flights_delivery_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "delivery_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_drone_flights_drones_drone_id",
                        column: x => x.drone_id,
                        principalTable: "drones",
                        principalColumn: "drone_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drone_flights_stations_arrival_station_id",
                        column: x => x.arrival_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drone_flights_stations_departure_station_id",
                        column: x => x.departure_station_id,
                        principalTable: "landing_stations",
                        principalColumn: "station_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    body = table.Column<string>(type: "text", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "fk_notifications_delivery_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "delivery_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_status_history",
                columns: table => new
                {
                    history_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    to_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_status_history", x => x.history_id);
                    table.ForeignKey(
                        name: "fk_order_status_history_delivery_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "delivery_orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_status_history_users_changed_by",
                        column: x => x.changed_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "flight_telemetry",
                columns: table => new
                {
                    telemetry_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    flight_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    altitude_m = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: true),
                    battery_pct = table.Column<short>(type: "smallint", nullable: true),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flight_telemetry", x => x.telemetry_id);
                    table.CheckConstraint("ck_telemetry_battery", "battery_pct IS NULL OR battery_pct BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "fk_flight_telemetry_drone_flights_flight_id",
                        column: x => x.flight_id,
                        principalTable: "drone_flights",
                        principalColumn: "flight_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_session",
                table: "ai_chat_messages",
                columns: new[] { "user_id", "session_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_key_hash",
                table: "api_keys",
                column: "key_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "asp_net_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "asp_net_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "asp_net_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "asp_net_user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "asp_net_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_created",
                table: "audit_logs",
                column: "created_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_audit_entity",
                table: "audit_logs",
                columns: new[] { "entity_name", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_user",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_addresses_one_default",
                table: "customer_addresses",
                column: "customer_id",
                unique: true,
                filter: "is_default = true");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_confirmations_order_id",
                table: "delivery_confirmations",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_delivery_confirmations_verified_by",
                table: "delivery_confirmations",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_orders_approved_by",
                table: "delivery_orders",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_orders_delivery_address_id",
                table: "delivery_orders",
                column: "delivery_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_orders_package_id",
                table: "delivery_orders",
                column: "package_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_delivery_orders_tracking_code",
                table: "delivery_orders",
                column: "tracking_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_created",
                table: "delivery_orders",
                column: "created_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer",
                table: "delivery_orders",
                columns: new[] { "customer_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_orders_destination",
                table: "delivery_orders",
                column: "destination_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_origin",
                table: "delivery_orders",
                column: "origin_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_status",
                table: "delivery_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_flights_arrival",
                table: "drone_flights",
                column: "arrival_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_departure",
                table: "drone_flights",
                column: "departure_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_drone",
                table: "drone_flights",
                column: "drone_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_order",
                table: "drone_flights",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_status",
                table: "drone_flights",
                column: "flight_status");

            migrationBuilder.CreateIndex(
                name: "ix_drones_drone_code",
                table: "drones",
                column: "drone_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_drones_station",
                table: "drones",
                column: "current_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_drones_status",
                table: "drones",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_telemetry_flight_time",
                table: "flight_telemetry",
                columns: new[] { "flight_id", "recorded_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_landing_stations_station_name",
                table: "landing_stations",
                column: "station_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stations_status",
                table: "landing_stations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_order_id",
                table: "notifications",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user",
                table: "notifications",
                columns: new[] { "user_id", "is_read", "created_at" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "ix_order_status_history_changed_by",
                table: "order_status_history",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "ix_status_history_order",
                table: "order_status_history",
                columns: new[] { "order_id", "changed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_station_operators_user",
                table: "station_operators",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_to",
                table: "station_routes",
                column: "to_station_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "normalized_user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_chat_messages");

            migrationBuilder.DropTable(
                name: "api_keys");

            migrationBuilder.DropTable(
                name: "asp_net_role_claims");

            migrationBuilder.DropTable(
                name: "asp_net_user_claims");

            migrationBuilder.DropTable(
                name: "asp_net_user_logins");

            migrationBuilder.DropTable(
                name: "asp_net_user_roles");

            migrationBuilder.DropTable(
                name: "asp_net_user_tokens");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "delivery_confirmations");

            migrationBuilder.DropTable(
                name: "flight_telemetry");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "order_status_history");

            migrationBuilder.DropTable(
                name: "station_operators");

            migrationBuilder.DropTable(
                name: "station_routes");

            migrationBuilder.DropTable(
                name: "asp_net_roles");

            migrationBuilder.DropTable(
                name: "drone_flights");

            migrationBuilder.DropTable(
                name: "delivery_orders");

            migrationBuilder.DropTable(
                name: "drones");

            migrationBuilder.DropTable(
                name: "customer_addresses");

            migrationBuilder.DropTable(
                name: "packages");

            migrationBuilder.DropTable(
                name: "landing_stations");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
