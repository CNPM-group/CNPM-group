# AppDbContext.cs

Lớp EF Core đại diện cho CSDL. Chỉ Repository được dùng nó; Services và Controllers không được inject `AppDbContext`.

## DbSet

| DbSet | Bảng |
|---|---|
| `Stations` | `landing_stations` |
| `StationRoutes` | `station_routes` |
| `Drones` | `drones` |
| `Packages` | `packages` |
| `CustomerAddresses` | `customer_addresses` |
| `DeliveryOrders` | `delivery_orders` |
| `OrderStatusHistories` | `order_status_history` |
| `DroneFlights` | `drone_flights` |
| `FlightTelemetries` | `flight_telemetry` |
| `DeliveryConfirmations` | `delivery_confirmations` |
| `Notifications` | `notifications` |
| `AuditLogs` | `audit_logs` |
| `AiChatMessages` | `ai_chat_messages` |
| `ApiKeys` | `api_keys` |
| `StationOperators` | `station_operators` |
| (Identity) `Users`, `Roles`... | `users`, `asp_net_roles`, `asp_net_user_roles`... |

## OnModelCreating

1. `base.OnModelCreating` – Identity tạo model user/role mặc định.
2. Đổi tên 6 bảng phụ của Identity sang snake_case.
3. Map `AppUser` → `users`: `user_id`, `email` (unique, ≤ 255), `full_name` (≤ 100), `phone` (≤ 20), `created_at` (mặc định `now()`), `is_active` (mặc định `true`).
4. `ApplyConfigurationsFromAssembly` – tự nạp mọi class `IEntityTypeConfiguration<T>` trong project (các file `Infrastructure/Models/*Configuration.cs`), khỏi phải đăng ký từng cái.

Sửa file này hoặc bất kỳ `*Configuration.cs` nào làm thay đổi bảng → phải tạo migration mới (xem `src/Migrations/README.md`).
