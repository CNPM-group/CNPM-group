# src

Toàn bộ backend, một project ASP.NET Core 8 (`SmartDrone.csproj`). Bốn thư mục con là bốn tầng; các file `.cs` nằm ngay ở `src/` là phần khởi động app, giống các file `app.py`, `create_app.py`, `config.py`... ở gốc repo mẫu Flask.

## Thư mục

| Thư mục | Tầng | Xem |
|---|---|---|
| `Api/` | Giao tiếp HTTP: controller, schema, middleware, Swagger | [Api/README.md](Api/README.md) |
| `Services/` | Nghiệp vụ | [Services/README.md](Services/README.md) |
| `Domain/` | Model nghiệp vụ, interface, hằng số, exception | [Domain/README.md](Domain/README.md) |
| `Infrastructure/` | CSDL, repository, JWT, LLM | [Infrastructure/README.md](Infrastructure/README.md) |
| `Migrations/` | EF Core migration | [Migrations/README.md](Migrations/README.md) |
| `Properties/` | `launchSettings.json`: profile `http` chạy ở `http://localhost:5080`, môi trường `Development`, tự mở `/swagger` | |

## File ở gốc `src/`

| File | Tương đương Flask | Tác dụng | Khi nào sửa |
|---|---|---|---|
| `Program.cs` | `app.py` | Điểm vào: gọi `CreateApp.BuildAsync(args)` rồi `app.Run()`. Chỉ 3 dòng. | Gần như không bao giờ |
| `CreateApp.cs` | `create_app.py` | Dựng app theo thứ tự: log → config → DI → JWT → CORS → controller → error handler → health check → Swagger; rồi gắn middleware, seed CSDL (Development), map route. Chi tiết: [CreateApp.md](CreateApp.md) | Thêm middleware / thay đổi pipeline |
| `Config.cs` | `config.py` | Class cấu hình `JwtOptions`, `LlmOptions`; hàm `AddAppConfig` đọc section `Jwt`, `Llm` và **dừng app ngay** nếu thiếu `Jwt:Key` (≥ 32 ký tự) hoặc `ConnectionStrings:Default`. | Thêm nhóm cấu hình mới |
| `Cors.cs` | `cors.py` | Policy `web`: cho phép origin trong `Cors:Origins` (mặc định `http://localhost:5173`) gọi API, có gửi cookie. | Web chạy ở cổng/khác domain |
| `DependencyContainer.cs` | `dependency_container.py` | Đăng ký DbContext, Identity, repository (interface → class), JWT, LLM client, service. Chi tiết: [DependencyContainer.md](DependencyContainer.md) | Mỗi lần thêm repository / service |
| `ErrorHandler.cs` | `error_handler.py` | Bắt mọi exception, đổi thành ProblemDetails (RFC 7807). Chi tiết: [ErrorHandler.md](ErrorHandler.md) | Thêm loại exception mới |
| `AppLogging.cs` | `app_logging.py` | Bật Serilog, đọc mức log từ section `Serilog`, ghi ra console. | Muốn ghi log ra file |
| `appsettings.json` | `.env` + `config.py` | Cấu hình chung mọi môi trường: JWT issuer/audience, LLM base URL/model, CORS, mức log. **Không** chứa khóa bí mật. | Đổi giá trị mặc định |
| `appsettings.Development.json` | `DevelopmentConfig` | Chỉ dùng khi `ASPNETCORE_ENVIRONMENT=Development`: connection string tới PostgreSQL trong Docker, JWT key dev, bật seed tài khoản demo. | Đổi mật khẩu DB dev |
| `SmartDrone.csproj` | `requirements.txt` | Khai báo .NET 8 và các package: EF Core + Npgsql, Identity, JWT Bearer, Serilog, Swashbuckle, HealthChecks. | Thêm thư viện |

## Thứ tự đọc config

ASP.NET đọc lần lượt, cái sau đè cái trước:

1. `appsettings.json`
2. `appsettings.{Environment}.json` (ví dụ `appsettings.Development.json`)
3. Biến môi trường (`Jwt__Key`, `Llm__ApiKey`, `ConnectionStrings__Default`...)

Thay cho `FactoryConfig.get_config(env)` bên Flask: muốn môi trường mới (ví dụ `Testing`) chỉ cần thêm `appsettings.Testing.json` và đặt `ASPNETCORE_ENVIRONMENT=Testing`.

## Build / chạy

```bash
dotnet build src/SmartDrone.csproj
dotnet run --project src --launch-profile http
```
