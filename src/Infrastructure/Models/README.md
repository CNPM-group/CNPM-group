# Infrastructure/Models

"ORM model" của tầng Infrastructure (tương đương `infrastructure/models` dùng SQLAlchemy bên Flask).

Khác Flask ở chỗ: EF Core map thẳng class Domain xuống bảng bằng **Fluent API**, nên ở đây không có bản sao của từng class, chỉ có file mô tả "class này nằm ở bảng nào, cột nào, ràng buộc gì". Domain nhờ vậy giữ được sạch (không attribute, không `using` EF Core). Lý do chi tiết: `docs/clean-architecture.md` mục 3.

## Tài khoản (ASP.NET Identity)

| File | Tác dụng |
|---|---|
| `AppUser.cs` | User lưu trong CSDL, kế thừa `IdentityUser<Guid>` (có sẵn email, mật khẩu băm, SĐT...) + `FullName`, `IsActive`, `CreatedAtUtc`. Domain dùng bản thuần `Domain/Models/User.cs`; `UserRepository` chuyển đổi giữa hai bản. |
| `AppRole.cs` | Role, kế thừa `IdentityRole<Guid>`. |

## Cấu hình bảng (`IEntityTypeConfiguration<T>`)

Mỗi file cấu hình một model trong `Domain/Models`: tên bảng, tên cột snake_case, kiểu cột (`numeric(…)`, `timestamp with time zone`), độ dài, giá trị mặc định, khóa ngoại + hành vi khi xóa, index, check constraint. Bám theo `docs/schema.sql`.

| File | Bảng | Ràng buộc đáng chú ý |
|---|---|---|
| `LandingStationConfiguration.cs` | `landing_stations` | trạng thái lưu chuỗi, chỉ nhận `Available/Occupied/Maintenance` |
| `StationRouteConfiguration.cs` | `station_routes` | tuyến từ trạm → trạm, khoảng cách |
| `DroneConfiguration.cs` | `drones` | mã drone, giới hạn trạng thái, khóa ngoại tới trạm hiện tại |
| `PackageConfiguration.cs` | `packages` | cân nặng, kích thước |
| `CustomerAddressConfiguration.cs` | `customer_addresses` | khóa ngoại tới `users` |
| `DeliveryOrderConfiguration.cs` | `delivery_orders` | `ck_order_status` (9 trạng thái), `ck_order_stations` (trạm gửi ≠ nhận), `ck_order_failure` (Failed phải có lý do), mã tra cứu unique |
| `OrderStatusHistoryConfiguration.cs` | `order_status_history` | khóa ngoại tới đơn |
| `DroneFlightConfiguration.cs` | `drone_flights` | khóa ngoại tới đơn, drone, 2 trạm |
| `FlightTelemetryConfiguration.cs` | `flight_telemetry` | khóa ngoại tới chuyến bay |
| `DeliveryConfirmationConfiguration.cs` | `delivery_confirmations` | OTP lưu hash, khóa ngoại tới đơn và người xác nhận |
| `NotificationConfiguration.cs` | `notifications` | khóa ngoại tới user, đơn |
| `StationOperatorConfiguration.cs` | `station_operators` | gán user vào trạm |
| `AuditLogConfiguration.cs` | `audit_logs` | khóa ngoại tới user |
| `AiChatMessageConfiguration.cs` | `ai_chat_messages` | khóa ngoại tới user |
| `ApiKeyConfiguration.cs` | `api_keys` | key lưu hash |

Ràng buộc đặt ở cả hai nơi: Service kiểm tra để báo lỗi tiếng Việt dễ hiểu; CSDL kiểm tra lần nữa để dữ liệu không bao giờ sai dù có ai ghi thẳng vào bảng.
