# DependencyContainer.cs

Tương đương `dependency_container.py`. Đây là chỗ duy nhất nối **interface** (Domain) với **class cài đặt** (Infrastructure). Các tầng khác chỉ xin interface qua constructor, ASP.NET tự đưa object đúng vào.

## Đăng ký hiện có

| Đăng ký | Vòng đời | Ý nghĩa |
|---|---|---|
| `AppDbContext` (Npgsql + snake_case) | Scoped | Một DbContext cho mỗi request; tên cột C# `CreatedAtUtc` thành `created_at_utc` (trừ cột đặt tên riêng trong `Infrastructure/Models`) |
| `AddIdentityCore<AppUser>().AddRoles<AppRole>()` | | Quản lý user/role/mật khẩu (băm mật khẩu, mật khẩu ≥ 8 ký tự, email không trùng) |
| `IUserRepository → UserRepository` | Scoped | |
| `IStationRepository → StationRepository` | Scoped | |
| `IOrderRepository → OrderRepository` | Scoped | |
| `ICustomerAddressRepository → CustomerAddressRepository` | Scoped | |
| `IDroneRepository → DroneRepository` | Scoped | |
| `IDeliveryRepository → DeliveryRepository` | Scoped | |
| `INotificationRepository → NotificationRepository` | Scoped | |
| `IReportRepository → ReportRepository` | Scoped | |
| `ITokenService → JwtTokenService` | Scoped | |
| `AddHttpClient<ILlmClient, LlmClient>()` | Transient | HttpClient do framework quản lý, tránh cạn socket |
| `AddSingleton<IOrderEvents, SignalROrderEvents>()` | Singleton | Chỉ giữ `IHubContext` (vốn là singleton), không giữ dữ liệu theo request |
| `AddSingleton<IFileStorage, MinioFileStorage>()` | Singleton | Dùng chung 1 `MinioClient` và cờ "đã tạo bucket" cho cả app |
| `AddScoped<PackageImageService>()` | Scoped | Giống các service nghiệp vụ khác |
| `AddHttpContextAccessor()` + `AddScoped<AuditInterceptor>()` | Singleton / Scoped | (v0.11) Interceptor cần biết người đang gọi API để ghi nhật ký; `AddDbContext((sp, o) => ... .AddInterceptors(...))` gắn nó vào mọi `DbContext` |
| `AddScoped<IAuditLogRepository, AuditLogRepository>()`, `AddScoped<AuditService>()`, `AddScoped<CustomerService>()` | Scoped | (v0.11) Nhật ký thao tác, màn Khách hàng |
| `AddScoped<AiInsightService>()` | Scoped | AI tóm tắt + nhận xét vận hành (v0.10) |
| `AddScoped<TrackingService>()` | Scoped | Dùng repository (DbContext Scoped) |
| `AddHostedService<DroneSimulator>()` | Hosted (sống suốt đời app) | Chạy nền giả lập drone; mỗi lượt tự tạo scope mới để lấy `TrackingService`, vì dịch vụ sống lâu không được giữ DbContext |
| `AuthService`, `StationService`, `OrderService`, `CustomerAddressService`, `AiService`, `EtaService`, `DroneService`, `DeliveryService`, `NotificationService`, `ReportService`, `UserService` | Scoped | Service đăng ký bằng class luôn, không cần interface vì không có bản cài đặt thứ hai |

Scoped = sống trong một request. Mọi repository dùng chung **một** `AppDbContext` trong cùng request, nên `OrderService` gọi nhiều repository rồi `SaveChangesAsync` một lần vẫn nằm trong một transaction.

## Thêm repository / service mới

```csharp
services.AddScoped<IDroneRepository, DroneRepository>();
services.AddScoped<DroneService>();
```

Quên dòng này thì app chạy được nhưng gọi endpoint sẽ lỗi 500 `Unable to resolve service for type ...`.
