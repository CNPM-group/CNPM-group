# CreateApp.cs

Tương đương `create_app.py` của repo mẫu: một hàm dựng toàn bộ ứng dụng, `Program.cs` chỉ việc gọi.

## Hàm `BuildAsync(string[] args)`

Phần 1 – đăng ký (trước `builder.Build()`):

| Dòng | Làm gì | Định nghĩa ở |
|---|---|---|
| `builder.AddAppLogging()` | Bật Serilog | `AppLogging.cs` |
| `builder.AddAppConfig()` | Nạp `JwtOptions`, `LlmOptions`, kiểm tra JWT key + connection string, trả về `JwtOptions` để dùng ngay | `Config.cs` |
| `AddDependencies(...)` | DbContext, Identity, repository, service | `DependencyContainer.cs` |
| `AddJwtAuthentication(jwt)` | Xác thực JWT (hàm private ngay trong file này) | `CreateApp.cs` |
| `AddAppCors(...)` | Policy CORS `web` | `Cors.cs` |
| `AddControllers().AddApiResponses()` | Bật controller, enum trả dạng chuỗi, format lỗi validate | `Api/Responses.cs` |
| `AddProblemDetails()` + `AddExceptionHandler<ErrorHandler>()` | Lỗi trả theo chuẩn ProblemDetails | `ErrorHandler.cs` |
| `AddHealthChecks().AddDbContextCheck<AppDbContext>()` | `/health` trả 200 khi kết nối được PostgreSQL | |
| `AddAppSwagger()` | Swagger + nút Authorize | `Api/Swagger.cs` |

Phần 2 – pipeline (sau `builder.Build()`), **thứ tự quan trọng**:

1. `UseAppMiddleware()` – log request, header `X-Request-Id`, bắt exception (phải đứng đầu để bắt được lỗi của mọi bước sau).
2. Chỉ ở Development: Swagger UI và `DbSeeder.SeedAsync` (migrate + dữ liệu mẫu). Bỏ qua seed khi chạy bằng công cụ `dotnet ef`.
3. `UseCors` → `UseAuthentication` → `UseAuthorization` – CORS phải trước auth, auth phải trước authorization.
4. `MapControllers()` – thay cho `routes.py`: ASP.NET tự tìm mọi controller có `[Route]`.
5. `MapHealthChecks("/health")`.

## JWT

- Token ký HMAC-SHA256 bằng `Jwt:Key`; kiểm tra issuer, audience, hạn dùng (lệch giờ cho phép 30 giây).
- `MapInboundClaims = false` để giữ nguyên tên claim ngắn `sub`, `email`, `name`, `role` (khớp `JwtTokenService`).
- Token lấy từ header `Authorization: Bearer ...`; nếu có cookie `smartdrone_auth` (hằng `Config.AuthCookie`) thì lấy từ cookie.
