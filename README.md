# SmartDroneDelivery

AI-powered Drone Delivery Management Platform – đồ án Công nghệ phần mềm, nhóm CNPM, Đại học Giao thông vận tải TP.HCM (UTH). GVHD: Nguyễn Văn Chiến.

Nền tảng quản lý nghiệp vụ giao hàng bằng drone giữa các trạm cố định: khách tạo đơn, chọn trạm gửi/nhận; điều phối viên duyệt đơn, gán drone, theo dõi chuyến; nhân viên trạm xác nhận hàng tới và giao cho khách bằng mã OTP; quản lý xem thống kê. Hệ thống **không** điều khiển drone thật – trạng thái bay do nhân viên (sau này là bộ giả lập) cập nhật.

Backend viết bằng ASP.NET Core 8, tổ chức theo Clean Architecture giống repo mẫu [Flask-CleanArchitecture](https://github.com/ChienNguyensrdn/Flask-CleanArchitecture) của thầy: cùng tên tầng, cùng cách chia thư mục, chỉ đổi ngôn ngữ. Bảng đối chiếu từng file Flask ↔ C# nằm ở [docs/clean-architecture.md](docs/clean-architecture.md).

Thay đổi qua từng lần cập nhật: [CHANGELOG.md](CHANGELOG.md).

## Mục lục

- [Chức năng](#chức-năng)
- [Phạm vi AI thực tế](#phạm-vi-ai-thực-tế)
- [Tình trạng hiện tại](#tình-trạng-hiện-tại)
- [Architecture](#architecture)
- [Run app](#run-app)
- [Tài khoản demo](#tài-khoản-demo)
- [Cấu hình](#cấu-hình)
- [Tài liệu](#tài-liệu)

## Chức năng

Tất cả API có tiền tố `/api/v1`, thử trực tiếp trên Swagger `http://localhost:5080/swagger`.

| Nhóm | Ai dùng | Làm được gì | API |
|---|---|---|---|
| Tài khoản | Mọi người | Đăng ký (role Customer), đăng nhập nhận JWT, xem thông tin mình | `/auth/register`, `/auth/login`, `/auth/me` |
| Quản lý tài khoản | Admin | Xem danh sách, tạo tài khoản nhân viên, gán role, khóa/mở tài khoản | `/users`, `/users/{id}/roles`, `/users/{id}/active` |
| Trạm hạ cánh | Xem: mọi người · Sửa: Admin, Manager | Thêm/sửa/xóa trạm, trạng thái Available / Occupied / Maintenance | `/stations` |
| Drone | Xem: nhân viên · Sửa: Admin, Manager | Thêm/sửa/xóa drone, tải tối đa, pin, trạng thái; không xóa được drone đang bay | `/drones` |
| Địa chỉ giao | Customer | Sổ địa chỉ, tối đa 1 địa chỉ mặc định | `/customers/me/addresses` |
| Đơn hàng | Customer | Tạo đơn (2 trạm phải khác nhau, kiện không vượt tải drone), sửa/hủy khi còn Pending, xem danh sách + chi tiết + lịch sử | `/orders`, `/orders/{id}`, `/orders/{id}/cancel`, `/orders/{id}/history` |
| Ước tính ETA | Customer | Quãng đường và thời gian dự kiến giữa 2 trạm trước khi đặt | `/orders/eta` |
| Duyệt đơn | Dispatcher, Manager, Admin | Duyệt hoặc từ chối (bắt buộc lý do) | `/orders/{id}/approve`, `/reject` |
| Giao hàng | Dispatcher, Station Operator | Gán drone + giờ bay → cất cánh → tới trạm (sinh OTP gửi khách) → nhập OTP hoàn tất; báo giao thất bại kèm lý do, giao lại bằng drone khác | `/orders/{id}/schedule`, `/start`, `/arrive`, `/confirm`, `/fail` |
| Thông báo | Mọi người | Thông báo đổi trạng thái đơn, mã OTP nhận hàng; đánh dấu đã đọc | `/notifications`, `/notifications/{id}/read` |
| Thống kê | Admin, Manager | Tổng đơn, tỷ lệ thành công, thời gian giao trung bình, số đơn theo trạng thái/ngày; xuất CSV | `/reports/orders`, `/reports/orders.csv` |
| Trợ lý AI | Mọi người | Chatbot hỏi đáp về đơn, trạm, ETA | `/ai/chat`, `/ai/feasibility-test` |

Vòng đời đơn (mọi lần đổi trạng thái đều đi qua `OrderStateMachine`, sai luật trả 409):

```
Pending ──duyệt──> Approved ──gán drone──> Scheduled ──cất cánh──> InTransit ──tới trạm──> ArrivedAtStation ──OTP đúng──> Completed
   │                                           ▲                       │                        │
   ├──từ chối──> Rejected                      └──────giao lại─────── Failed <──────sự cố───────┘
   └──khách hủy──> Cancelled
```

## Phạm vi AI thực tế

Ghi đúng những gì code đang làm, không hứa trước:

| Tính năng | Tình trạng | Cách làm |
|---|---|---|
| Ước tính ETA | **Đã có** | Không dùng mô hình học máy. Khoảng cách Haversine giữa 2 trạm ÷ tốc độ drone (mặc định 40 km/h) + 0,5 phút/kg kiện. Thông số chỉnh ở section `Eta` trong `appsettings.json` |
| Chatbot hỗ trợ khách | **Đã có** | Gọi LLM API (chuẩn OpenAI) có function calling 3 tool: `get_order_status`, `get_station_status`, `get_delivery_eta`. Bot chỉ trả lời từ dữ liệu tool trả về, khách chỉ tra được đơn của chính mình. Không có `Llm:ApiKey` thì API báo lỗi 503, phần còn lại vẫn chạy. Prompt: [docs/ai/ai-chatbot-prompt.md](docs/ai/ai-chatbot-prompt.md) |
| Tóm tắt chuyến giao bằng LLM | Chưa làm (KAN-62) | |
| Phân tích vận hành / dashboard | Có API số liệu, chưa có AI | `/reports/orders` tính bằng SQL, không phải AI |
| Gợi ý tuyến bay tối ưu, cảnh báo trễ | **Không làm trong đồ án này** | Hướng phát triển sau |

## Tình trạng hiện tại

Cập nhật 05/11/2026. Chi tiết từng lần sửa: [CHANGELOG.md](CHANGELOG.md).

**Backend – chạy được:**
- Đăng nhập JWT 5 role, quản lý tài khoản, trạm, drone, địa chỉ giao.
- Đơn hàng trọn vòng đời: tạo → duyệt → gán drone → bay → tới trạm → OTP → hoàn tất / thất bại → giao lại.
- Thông báo trong app, thống kê + CSV, chatbot AI, ETA.
- `docker compose up -d --build` dựng cả CSDL, MinIO, API, web.

**Còn làm:**

| Việc | Người | Jira |
|---|---|---|
| Upload ảnh kiện / ảnh bằng chứng lên MinIO (`POST /packages/images` đã có trong `openapi.yaml`, app đã gọi, **API chưa có** → chọn ảnh khi tạo đơn sẽ lỗi 404) | Vinh | KAN-33 |
| SignalR hub + bộ giả lập drone phát tọa độ (app đã có `signalr_service.dart`, chờ hub) | Vinh | KAN-55 |
| Hoàn trả đơn Failed → Cancelled cho Dispatcher | Tiến | KAN-57 |
| Web: hiện chỉ `login.html`, `register.html` gọi API thật; các trang đơn hàng, địa chỉ còn dữ liệu mẫu. Còn thiếu màn Dispatcher, Station Operator, dashboard | Huỳnh | KAN-41, 42, 59, 60, 65 |
| Mobile: đăng nhập, danh sách đơn, chi tiết, tạo đơn đã gọi API; còn màn địa chỉ, thông báo/OTP, chat AI | Đức | KAN-47, 61, 67 |
| Postman collection + test toàn luồng | Tài | KAN-37, 68 |

# Architecture

```bash
    ├── .github/workflows/ci.yml      # build tự động mỗi lần push / mở PR
    ├── docs                          # tài liệu: báo cáo (report/), sơ đồ (diagrams/), kiến trúc, API, CSDL, AI
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
    │   ├── Dockerfile                # build image API cho docker compose
    │   └── SmartDrone.csproj
    ├── web                           # giao diện web (HTML/JS)
    ├── mobile                        # app Flutter cho khách hàng
    ├── docker-compose.yml            # PostgreSQL + MinIO + API + web
    ├── CHANGELOG.md                  # nhật ký thay đổi từng lần cập nhật
    └── chay-thu.bat                  # chạy thử 1 phát trên Windows
```

Mỗi thư mục trong `src/` có một `README.md` giải thích từng file trong đó là gì, tác dụng gì, khi nào sửa. File quan trọng có thêm file `.md` cùng tên đặt ngay cạnh (ví dụ `src/Services/DeliveryService.md`).

| Thư mục | Là gì | Tác dụng | Đọc thêm |
|---|---|---|---|
| `src/Api` | Tầng giao tiếp HTTP | Nhận request, kiểm tra dữ liệu vào, gọi service, trả JSON/ProblemDetails | [src/Api/README.md](src/Api/README.md) |
| `src/Services` | Tầng nghiệp vụ | Chứa mọi quy tắc: trạm gửi ≠ trạm nhận, kiện không vượt tải, chỉ hủy khi Pending, OTP sai 5 lần thì khóa... | [src/Services/README.md](src/Services/README.md) |
| `src/Domain` | Lõi | Model, hằng số, exception, interface repository, `OrderStateMachine`. Không phụ thuộc EF Core hay framework | [src/Domain/README.md](src/Domain/README.md) |
| `src/Infrastructure` | Phần cắm vào bên ngoài | PostgreSQL (EF Core), ASP.NET Identity, JWT, LLM API. Đổi CSDL chỉ sửa tầng này | [src/Infrastructure/README.md](src/Infrastructure/README.md) |
| `src/Migrations` | Lịch sử CSDL | Tạo/sửa bảng theo từng phiên bản | [src/Migrations/README.md](src/Migrations/README.md) |
| `web` | Giao diện web | Trang cho nhân viên và khách trên trình duyệt | [web/README.md](web/README.md) |
| `mobile` | App Flutter | App cho khách: đăng nhập, tạo đơn, theo dõi đơn | [mobile/README.md](mobile/README.md) |
| `docs` | Tài liệu | Kiến trúc, hợp đồng API, CSDL, AI, phân tích nghiệp vụ | [docs/README.md](docs/README.md) |
| `scripts` | Script tiện ích | Bật PostgreSQL bằng Docker | [scripts/README.md](scripts/README.md) |

## Domain Layer

`src/Domain` – lõi nghiệp vụ, không phụ thuộc framework hay CSDL.
- `Models/`: class nghiệp vụ (`DeliveryOrder`, `LandingStation`, `Package`, `Drone`, `DroneFlight`, `DeliveryConfirmation`, `Notification`, `User`...) và interface `I*Repository`, `ITokenService`, `ILlmClient`. Tầng trên chỉ biết interface, không biết EF Core hay PostgreSQL.
- `Models/OrderStateMachine.cs`: bảng chuyển trạng thái hợp lệ của đơn.
- `Constants.cs`: role, trạng thái đơn/trạm/drone/chuyến bay, giới hạn phân trang.
- `Exceptions.cs`: lỗi nghiệp vụ (`NotFoundException`, `ValidationException`, `ConflictException`...). `ErrorHandler.cs` đổi chúng thành HTTP status.

Chi tiết: [src/Domain/README.md](src/Domain/README.md)

## Services Layer

`src/Services` – quy tắc nghiệp vụ. Service nhận interface repository qua constructor, không đụng trực tiếp `DbContext`.

| Service | Lo việc gì |
|---|---|
| `AuthService` | Đăng ký, đăng nhập, cấp JWT |
| `UserService` | Admin quản lý tài khoản, gán role, khóa/mở |
| `StationService`, `DroneService` | CRUD trạm, drone và ràng buộc |
| `CustomerAddressService` | Sổ địa chỉ của khách |
| `OrderService` | Tạo, sửa, hủy, duyệt, từ chối, xem đơn |
| `DeliveryService` | Gán drone, cất cánh, tới trạm, OTP, giao thất bại |
| `NotificationService` | Thông báo của người dùng |
| `ReportService` | Thống kê, xuất CSV |
| `EtaService` | Ước tính ETA |
| `AiService` | Chatbot AI |

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

    git clone https://github.com/CNPM-group/CNPM-group.git
    cd CNPM-group

## Kiểm tra máy đã cài .NET 8 SDK và Docker chưa

    dotnet --version
    docker --version

Chưa có thì cài [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) và [Docker Desktop](https://www.docker.com/products/docker-desktop/) (mở Docker Desktop lên trước khi chạy).

## Run app

### Cách 1: chạy cả hệ thống bằng Docker (khỏi cài .NET)

    copy .env.example .env          # Windows
    cp .env.example .env            # Unix/MacOS
    docker compose up -d --build

- API + Swagger: http://localhost:5080/swagger
- Web: http://localhost:5173/login.html
- MinIO console: http://localhost:9001
- Tắt: `docker compose down` (thêm `-v` nếu muốn xóa sạch dữ liệu)

### Cách 2: chạy API trên máy (để code, debug)

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

Build báo `Duplicate 'AssemblyVersionAttribute'` → xóa `src/obj`, `src/bin` và các thư mục cũ `src/SmartDrone.Api`, `src/SmartDrone.Application`, `src/SmartDrone.Domain`, `src/SmartDrone.Infrastructure` rồi build lại.

### Thử nhanh luồng giao hàng trên Swagger

1. `customer@drone.local` đăng nhập → `POST /orders` tạo đơn.
2. `dispatcher@drone.local` → `POST /orders/{id}/approve` → `GET /drones` lấy 1 drone Idle → `POST /orders/{id}/schedule` → `POST /orders/{id}/start`.
3. `stationoperator@drone.local` → `POST /orders/{id}/arrive`.
4. Customer → `GET /notifications` đọc mã OTP 6 số.
5. Station operator → `POST /orders/{id}/confirm` với mã OTP → đơn `Completed`.
6. `manager@drone.local` → `GET /reports/orders` xem số liệu.

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
    JWT_KEY=khoa-ky-jwt-toi-thieu-32-ky-tu
    LLM_API_KEY=                     # để trống thì chatbot tắt, phần khác vẫn chạy

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

- Web: `npx serve -l 5173 web` rồi mở http://localhost:5173/login.html (hoặc dùng Docker ở Cách 1) – xem [web/README.md](web/README.md)
- Mobile: xem [mobile/README.md](mobile/README.md)

## Tài liệu

Danh mục tài liệu: [docs/README.md](docs/README.md). Quy ước code, Git, API: [docs/CONVENTIONS.md](docs/CONVENTIONS.md). Hợp đồng API: [docs/openapi.yaml](docs/openapi.yaml). Báo cáo chính (SPMP/SRS/SDD/Test/Release) nằm trên Confluence của nhóm.

## Nhóm

| Thành viên | Phụ trách |
|---|---|
| Nguyễn Ngọc Trần Hoàng Tiến (trưởng nhóm) | Kiến trúc, API spec, quản lý dự án |
| Nguyễn Thiện Vinh | Backend, cơ sở dữ liệu |
| Nguyễn Mậu Huỳnh | Web |
| Lê Công Anh Đức | Mobile |
| Lê Công Anh Tài | Dịch vụ AI, kiểm thử |
