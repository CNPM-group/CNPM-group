# Services

Nghiệp vụ của hệ thống (tương đương `src/services` của repo mẫu, ví dụ `TodoService(ITodoRepository)`). Mỗi service nhận interface repository qua constructor, kiểm tra luật, gọi repository, trả model Domain. Sai luật thì ném exception trong `Domain/Exceptions.cs`.

Service **không** biết HTTP (không có `IActionResult`, không đọc header) và **không** biết EF Core (không có `AppDbContext`).

| File | Phụ thuộc | Chức năng | Chi tiết |
|---|---|---|---|
| `AuthService.cs` | `IUserRepository`, `ITokenService` | Đăng ký (luôn role Customer), đăng nhập, tạo JWT | [AuthService.md](AuthService.md) |
| `StationService.cs` | `IStationRepository` | CRUD trạm; không tìm thấy → `NotFoundException` | bên dưới |
| `OrderService.cs` | `IOrderRepository`, `IStationRepository`, `ICustomerAddressRepository`, `IDroneRepository`, `IOrderEvents` | Tạo, sửa, hủy, duyệt, từ chối, xem (cả bản chi tiết), phân trang, lịch sử đơn; đổi trạng thái xong thì gửi event realtime | [OrderService.md](OrderService.md) |
| `CustomerAddressService.cs` | `ICustomerAddressRepository` | CRUD địa chỉ của khách; đặt 1 địa chỉ làm mặc định thì tự bỏ mặc định ở địa chỉ cũ (trước v0.8 báo lỗi 409) | bên dưới |
| `DroneService.cs` | `IDroneRepository`, `IStationRepository` | CRUD drone; mã drone không trùng, trạm hiện tại phải tồn tại, không xóa drone đang gán/đang bay | bên dưới |
| `DeliveryService.cs` | `IOrderRepository`, `IDroneRepository`, `IDeliveryRepository`, `INotificationRepository`, `EtaService`, `IOrderEvents` | Luồng giao: gán drone, cất cánh, tới trạm + sinh OTP, xác nhận OTP, giao thất bại, hủy và hoàn đơn thất bại (KAN-57); mỗi bước gửi event realtime | [DeliveryService.md](DeliveryService.md) |
| `TrackingService.cs` | `IOrderRepository`, `IDeliveryRepository`, `IStationRepository`, `IDroneRepository` | Tính vị trí drone giả lập theo giờ cất cánh + ETA, lưu telemetry (v0.9) | [TrackingService.md](TrackingService.md) |
| `DroneSimulator.cs` | `TrackingService`, `IOrderEvents` | Dịch vụ chạy nền: vài giây một lần dời drone đang bay, gửi event `DroneLocation` (v0.9) | [DroneSimulator.md](DroneSimulator.md) |
| `PackageImageService.cs` | `IFileStorage`, `IOrderRepository` | Upload ảnh kiện (≤ 5 MB, kiểm loại ảnh bằng byte đầu), mở ảnh theo đơn có kiểm quyền | [PackageImageService.md](PackageImageService.md) |
| `NotificationService.cs` | `INotificationRepository` | Xem và đánh dấu đã đọc thông báo của người đang đăng nhập | |
| `CustomerService.cs` | `IUserRepository`, `IOrderRepository`, `ICustomerAddressRepository` | (v0.11, FR03) Nhân viên xem danh sách khách kèm số đơn, chi tiết khách (địa chỉ + 20 đơn gần nhất), Quản lý/Admin sửa họ tên, số điện thoại. Id của tài khoản nhân viên → 404 | bên dưới |
| `ProfileRules.cs` | – | (v0.11) Luật chung cho họ tên (2–100 ký tự) và số điện thoại (8–15 chữ số, có thể có `+`), dùng ở hồ sơ cá nhân, sửa khách, người nhận của đơn | |
| `AuditService.cs` | `IAuditLogRepository` | (v0.11, UC-29) Phân trang nhật ký thao tác cho Admin, lọc theo loại dữ liệu. Việc ghi nhật ký do `Infrastructure/Databases/AuditInterceptor.cs` tự làm | |
| `UserService.cs` | `IUserRepository` | Admin quản lý tài khoản: liệt kê, tạo tài khoản nội bộ, gán vai trò (chỉ nhận 5 vai trò hợp lệ, không phân biệt hoa thường), khóa/mở. Chặn Admin tự gỡ quyền hoặc tự khóa mình | |
| `ReportService.cs` | `IReportRepository` | Thống kê đơn theo khoảng ngày + xuất CSV | bên dưới |
| `EtaService.cs` | `IStationRepository`, `EtaOptions` | Ước tính ETA trước khi tạo đơn: khoảng cách Haversine giữa 2 trạm ÷ vận tốc drone + phút cộng thêm theo cân nặng (KAN-53, công thức của Tài) | bên dưới |
| `AiService.cs` | `ILlmClient`, `OrderService`, `StationService` | Hỏi LLM 1 câu; chatbot có gọi tool tra đơn/trạm/ETA | [AiService.md](AiService.md) |
| `AiInsightService.cs` | `ILlmClient`, `OrderService`, `IDeliveryRepository`, `IDroneRepository`, `ReportService`, `IReportRepository` | AI tóm tắt chuyến giao (KAN-62) và nhận xét vận hành; không có LLM thì tự ghép theo luật (v0.10) | [AiInsightService.md](AiInsightService.md) |

## StationService.cs

- `ListAsync`: tất cả trạm, sắp theo tên.
- `GetAsync(id)`: không có → `NotFoundException("Không tìm thấy trạm.")`.
- `CreateAsync` / `UpdateAsync`: cắt khoảng trắng tên, gán tọa độ, trạng thái.
- `CreateAsync` / `UpdateAsync` nhận thêm `capacity` (1–50, v0.11): số kiện trạm chứa cùng lúc, dùng khi gán drone.
- `LoadAsync` (v0.11): số đơn đang tới / kiện chờ khách / đơn sắp đi của từng trạm, cho trang Drone & trạm.
- `DeleteAsync`: xóa. Trạm đang được đơn/drone tham chiếu thì PostgreSQL chặn bằng khóa ngoại; từ v0.10 repository bắt lỗi này và trả **409** "Trạm đã có đơn hàng hoặc drone dùng tới... chuyển sang Bảo trì thay vì xóa" (trước đó là lỗi 500).

## DroneService.cs

- `CreateAsync` / `UpdateAsync`: mã drone trùng → `ConflictException` (409); `CurrentStationId` có mà trạm không tồn tại → `ValidationException` (400). Giới hạn tải, tầm bay, vận tốc kiểm tra ở `Api/Schemas/DroneSchemas.cs`.
- `DeleteAsync`: drone đang `Assigned` hoặc `InFlight` → 409.

## EtaService.cs

`EstimateAsync(origin, destination, weightKg)`:

```
phút = khoảng_cách_km / Eta:DroneSpeedKmPerHour × 60 + weightKg × Eta:WeightMinutesPerKg
```

Mặc định 40 km/h và 0.5 phút/kg (`appsettings.json`, section `Eta`). Id trạm rỗng hoặc cân nặng ngoài 0.01–50 kg → 400; không thấy trạm → 404. Khoảng cách tính bằng `GeoDistance.HaversineKm` ở `Domain/Models/EtaEstimate.cs`.

## ReportService.cs

- `SummaryAsync(from, to)`: mặc định 30 ngày gần nhất, tối đa 366 ngày. Tỷ lệ thành công = hoàn thành ÷ (hoàn thành + thất bại). Thời gian giao = `completed_at − created_at`.
- `StationsAsync(from, to)` (v0.11, FR11): mỗi trạm 1 dòng: số đơn gửi đi, nhận về, hoàn tất, thất bại, tỷ lệ thành công, thời gian giao TB. 4 số sau tính theo **trạm nhận** (nơi khách nhập OTP). Dùng lại dữ liệu của `ExportRowsAsync`, không thêm truy vấn mới.
- `ExportCsvAsync`: cột `tracking_code, status, origin_station, destination_station, weight_kg, created_at_utc, completed_at_utc, failure_reason`; ô có dấu phẩy/ngoặc kép được bọc đúng chuẩn CSV.

## CustomerAddressService.cs

- Mọi hàm nhận `customerId` của người đang đăng nhập; địa chỉ của khách khác coi như không tồn tại (404).
- Mỗi khách tối đa **một** địa chỉ mặc định: tạo/sửa với `isDefault = true` thì địa chỉ mặc định cũ tự bỏ cờ (`ClearDefaultAsync`, từ v0.8; trước đó trả 409).

## CustomerService.cs (v0.11)

- `ListAsync(search)`: mọi tài khoản vai trò Customer, lọc theo tên / email / số điện thoại, kèm `Total`, `Completed`, `Active` (đơn chưa kết thúc), ngày đặt gần nhất (một truy vấn `GROUP BY customer_id`).
- `GetAsync(id)`: hồ sơ + địa chỉ + 20 đơn gần nhất. Id không phải khách (nhân viên) → 404, để màn này không thành chỗ sửa tài khoản nội bộ (việc đó ở trang Tài khoản của Admin).
- `UpdateAsync(id, fullName, phone)`: sửa họ tên, số điện thoại qua `ProfileRules`. Email và mật khẩu không sửa ở đây.

