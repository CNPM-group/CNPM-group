# SmartDroneDelivery

Repo: https://github.com/nguyenmauhuynh/CNPM-group

Nền tảng quản lý giao hàng bằng drone tích hợp AI. Backend: ASP.NET Core 8 (Clean Architecture) + PostgreSQL + MinIO.

## Chạy lần đầu

Yêu cầu: .NET 8 SDK, Docker Desktop.

```bash
# 1. Tạo solution (chỉ làm 1 lần, người dựng repo commit file .sln lên)
dotnet new sln -n SmartDrone
dotnet sln add src/SmartDrone.Domain/SmartDrone.Domain.csproj src/SmartDrone.Application/SmartDrone.Application.csproj src/SmartDrone.Infrastructure/SmartDrone.Infrastructure.csproj src/SmartDrone.Api/SmartDrone.Api.csproj

# 2. DB + MinIO
cp .env.example .env        # Windows: copy .env.example .env
docker compose up -d

# 3. Migration (chỉ làm 1 lần, người dựng repo commit thư mục Migrations lên)
dotnet tool install --global dotnet-ef
dotnet ef migrations add Initial -p src/SmartDrone.Infrastructure -s src/SmartDrone.Api -o Persistence/Migrations

# 4. Chạy API (tự migrate + seed ở môi trường Development)
dotnet run --project src/SmartDrone.Api
```

Swagger: http://localhost:5080/swagger — Health: http://localhost:5080/health

## Tài khoản demo (chỉ Development)

Mật khẩu chung `Demo@12345`:
`customer@drone.local`, `dispatcher@drone.local`, `stationoperator@drone.local`, `manager@drone.local`, `admin@drone.local`

## Test nhanh

1. `POST /api/v1/auth/login` bằng `admin@drone.local` → copy `token`.
2. Bấm **Authorize** trên Swagger, dán token.
3. `GET /api/v1/stations` → thấy 5 trạm. `POST` bằng tài khoản Customer phải ra **403**.

## Cấu trúc

```
src/
  SmartDrone.Domain          Entity, Enum, hằng số (không phụ thuộc gì)
  SmartDrone.Application     DTO, interface, service nghiệp vụ
  SmartDrone.Infrastructure  EF Core, Identity, JWT, seed
  SmartDrone.Api             Controller, Program.cs, config
web/       React (chưa làm)
mobile/    Flutter (chưa làm)
docs/      ERD, BPMN, SRS
```
