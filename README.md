# SmartDroneDelivery

AI-powered Drone Delivery Management Platform – đồ án Công nghệ phần mềm, nhóm CNPM, Đại học Giao thông vận tải TP.HCM (UTH). GVHD: Nguyễn Văn Chiến.

Nền tảng quản lý nghiệp vụ giao hàng bằng drone: khách tạo đơn, chọn trạm gửi/nhận, nhân viên duyệt và điều phối, theo dõi trạng thái, xác nhận nhận hàng, có trợ lý AI. Hệ thống **không** điều khiển drone thật.

Backend viết bằng ASP.NET Core 8, tổ chức theo Clean Architecture giống repo mẫu [Flask-CleanArchitecture](https://github.com/ChienNguyensrdn/Flask-CleanArchitecture) của thầy: cùng tên tầng, cùng cách chia thư mục, chỉ đổi ngôn ngữ. Bảng đối chiếu từng file Flask ↔ C# nằm ở [docs/clean-architecture.md](docs/clean-architecture.md).

# Architecture

```bash
    ├── .github/workflows/ci.yml      # build tự động mỗi lần push / mở PR
    ├── docs                          # tài liệu: kiến trúc, API, CSDL, use case, AI
    ├── scripts
    │   ├── run_postgres.sh           # bật PostgreSQL bằng Docker (Linux/macOS)
    │   └── run_postgres.ps1          # bật PostgreSQL bằng Docker (Windows)
    ├── src
    │   ├── Api
    │   │   ├── Controllers
    │   │   │   └── ...               # controller cho API (nhận request, trả response)
    │   │   ├── Schemas
    │   │   │   └── ...               # request/response schema (DTO + validate)
    │   │   ├── Middleware.cs
    │   │   ├── Responses.cs
    │   │   ├── Requests.cs
    │   │   └── Swagger.cs
    │   ├── Infrastructure
    │   │   ├── Services
    │   │   │   └── ...               # dịch vụ dùng thư viện / bên thứ ba (JWT, LLM)
    │   │   ├── Databases
    │   │   │   └── ...               # DbContext, khởi tạo và seed CSDL
    │   │   ├── Repositories
    │   │   │   └── ...               # cài đặt các interface I*Repository bằng EF Core
    │   │   └── Models
    │   │       └── ...               # ánh xạ ORM: model ↔ bảng PostgreSQL
    │   ├── Domain
    │   │   ├── Constants.cs
    │   │   ├── Exceptions.cs
    │   │   └── Models
    │   │       └── ...               # model nghiệp vụ + interface I*Repository
    │   ├── Services
    │   │   └── ...                   # nghiệp vụ (business logic)
    │   ├── Migrations
    │   │   └── ...                   # EF Core migration (tương đương thư mục migrations của Flask)
    │   ├── Program.cs                # = app.py
    │   ├── CreateApp.cs              # = create_app.py
    │   ├── Config.cs                 # = config.py
    │   ├── Cors.cs                   # = cors.py
    │   ├── DependencyContainer.cs    # = dependency_container.py
    │   ├── ErrorHandler.cs           # = error_handler.py
    │   ├── AppLogging.cs             # = app_logging.py
    │   ├── appsettings.json
    │   └── SmartDrone.csproj
    ├── web                           # giao diện web (HTML/JS)
    ├── mobile                        # app Flutter cho khách hàng
    ├── docker-compose.yml            # PostgreSQL + MinIO
    └── chay-thu.bat                  # chạy thử 1 phát trên Windows
```

Mỗi thư mục trong `src/` có một `README.md` giải thích từng file trong đó làm gì, dùng thế nào. File quan trọng có thêm file `.md` cùng tên đặt ngay cạnh (ví dụ `src/Services/OrderService.md`).

## Domain Layer

`src/Domain` – lõi nghiệp vụ, không phụ thuộc framework hay CSDL.
- `Models/`: class nghiệp vụ (`DeliveryOrder`, `LandingStation`, `Package`, `User`...) và interface `I*Repository`, `ITokenService`, `ILlmClient`. Tầng trên chỉ biết interface, không biết EF Core hay PostgreSQL.
- `Constants.cs`: role, trạng thái đơn/trạm/drone, giới hạn phân trang.
- `Exceptions.cs`: lỗi nghiệp vụ (`NotFoundException`, `ValidationException`, `ConflictException`...). `ErrorHandler.cs` đổi chúng thành HTTP status.

Chi tiết: [src/Domain/README.md](src/Domain/README.md)

## Services Layer

`src/Services` – quy tắc nghiệp vụ: trạm gửi ≠ trạm nhận, kiện không vượt tải drone, chỉ hủy/sửa đơn khi `Pending`, mỗi khách chỉ một địa chỉ mặc định, chatbot chỉ được tra đơn của chính khách... Service nhận interface repository qua constructor, không đụng trực tiếp `DbContext`.

Chi tiết: [src/Services/README.md](src/Services/README.md)

## Infrastructure Layer

`src/Infrastructure` – phần "cắm vào" bên ngoài: PostgreSQL qua EF Core, ASP.NET Identity, JWT, LLM API. Repository cài đặt interface của Domain, nên muốn đổi CSDL chỉ sửa tầng này.

Chi tiết: [src/Infrastructure/README.md](src/Infrastructure/README.md)

## Luồng một request

```
Client (web/mobile) → Api/Controllers → Services → Domain (interface) → Infrastructure/Repositories → PostgreSQL
```

Sơ đồ sequence (PlantUML) và ví dụ "tạo đơn hàng" đi qua từng tầng: [docs/clean-architecture.md](docs/clean-architecture.md)

## Download source code (CMD)

    git clone https://github.com/nguyenmauhuynh/cnpm-group.git
    cd cnpm-group

## Kiểm tra máy đã cài .NET 8 SDK và Docker chưa

    dotnet --version
    docker --version

Chưa có thì cài [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) và [Docker Desktop](https://www.docker.com/products/docker-desktop/) (mở Docker Desktop lên trước khi chạy).

## Run app

- Bước 1: Tạo file `.env` từ file mẫu (chứa mật khẩu PostgreSQL, MinIO)
  ## Windows:
        copy .env.example .env
  ## Unix/MacOS:
        cp .env.example .env

- Bước 2: Bật PostgreSQL bằng Docker
  ## Windows (PowerShell):
        .\scripts\run_postgres.ps1
  ### Nếu PowerShell chặn chạy script, mở PowerShell (Administrator):
        Set-ExecutionPolicy RemoteSigned -Force
  ## Unix/MacOS:
        ./scripts/run_postgres.sh

- Bước 3: Cài thư viện (NuGet)
  ## Install:
        dotnet restore src/SmartDrone.csproj

- Bước 4: Chạy API
  ## Run:
        dotnet run --project src --launch-profile http

  Lần chạy đầu (môi trường Development) API tự chạy migration, tạo 5 role, 5 trạm, 20 tuyến bay, 3 drone và 5 tài khoản demo.

      Truy cập http://localhost:5080/swagger   (tài liệu API, bấm Authorize dán token)
      Truy cập http://localhost:5080/health    (kiểm tra API + CSDL)

Windows lười gõ thì nhấp đúp `chay-thu.bat` ở thư mục gốc: script làm hết các bước trên, test đăng nhập và ghi kết quả ra `ket-qua-test.txt`.

## Tài khoản demo

Mật khẩu chung `Demo@12345`:

| Email | Role |
|---|---|
| customer@drone.local | Customer |
| dispatcher@drone.local | Dispatcher |
| stationoperator@drone.local | StationOperator |
| manager@drone.local | Manager |
| admin@drone.local | Admin |

## Cấu hình

File `.env` ở thư mục gốc (dùng cho docker-compose):

    DB_PASSWORD=drone_dev
    MINIO_ROOT_USER=minioadmin
    MINIO_ROOT_PASSWORD=minioadmin123

File `src/appsettings.Development.json` (chỉ cho máy dev):

    ConnectionStrings:Default = Host=localhost;Port=5432;Database=drone;Username=drone;Password=drone_dev
    Jwt:Key                   = khóa ký JWT, tối thiểu 32 ký tự
    Seed:DemoUsers            = true (tạo 5 tài khoản demo)

Khóa thật (JWT, LLM API key) **không commit**, đặt qua biến môi trường, dấu `:` đổi thành `__`:

    # Windows (PowerShell)
    $env:Jwt__Key="chuoi-bi-mat-it-nhat-32-ky-tu..."
    $env:Llm__ApiKey="sk-..."
    # Unix/MacOS
    export Jwt__Key="chuoi-bi-mat-it-nhat-32-ky-tu..."
    export Llm__ApiKey="sk-..."

## Pull image PostgreSQL (nếu không dùng script)

```bash
docker pull postgres:16
docker compose up -d db
```

## Test kết nối PostgreSQL

```bash
docker compose exec db psql -U drone -d drone -c "\dt"
```

## Migration (EF Core = ORM giống SQLAlchemy bên Flask)

```bash
dotnet tool install --global dotnet-ef
# sửa model / cấu hình bảng xong thì tạo migration mới
dotnet ef migrations add <TenMigration> --project src -o Migrations
# áp migration vào CSDL (API cũng tự làm khi chạy Development)
dotnet ef database update --project src
```

## Chạy web và mobile

- Web: `npx serve -l 5173 web` rồi mở http://localhost:5173/login.html – xem [web/README.md](web/README.md)
- Mobile: xem [mobile/README.md](mobile/README.md)

## Tài liệu

Danh mục tài liệu: [docs/README.md](docs/README.md). Quy ước code, Git, API: [docs/CONVENTIONS.md](docs/CONVENTIONS.md).

## Nhóm

| Thành viên | Phụ trách |
|---|---|
| Nguyễn Ngọc Trần Hoàng Tiến (trưởng nhóm) | Kiến trúc, API spec, quản lý dự án |
| Nguyễn Thiện Vinh | Backend, cơ sở dữ liệu |
| Nguyễn Mậu Huỳnh | Web |
| Lê Công Anh Đức | Mobile |
| Lê Công Anh Tài | Dịch vụ AI, kiểm thử |
