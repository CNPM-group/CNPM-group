# Api/Schemas

Hình dạng JSON vào/ra của API (tương đương Marshmallow schema bên Flask). Tách riêng khỏi model Domain để đổi API không ảnh hưởng nghiệp vụ và ngược lại.

Quy ước:
- `XxxRequest`: dữ liệu client gửi lên. Validate bằng attribute (`[Required]`, `[MaxLength]`, `[Range]`, `[EmailAddress]`); sai thì `[ApiController]` tự trả 400 trước khi vào controller. Có hàm `ToOrder()` / `ToDomain()` để đổi sang model Domain.
- `XxxResponse`: dữ liệu trả về. Có hàm tĩnh `From(model)` đổi từ model Domain.
- JSON dùng camelCase (`trackingCode`), enum là chuỗi, thời gian UTC ISO-8601.

| File | Schema | Dùng ở |
|---|---|---|
| `AuthSchemas.cs` | `RegisterRequest` (email, mật khẩu ≥ 8, họ tên), `LoginRequest`, `AuthResponse` (token, `expiresAtUtc`, `userId`, email, `fullName`, roles), `MeResponse` | `AuthController` |
| `StationSchemas.cs` | `StationRequest` (tên ≤ 200, vĩ độ −90..90, kinh độ −180..180, trạng thái), `StationResponse` | `StationsController` |
| `OrderSchemas.cs` | `OrderRequest` + `PackageInput` (dùng chung cho tạo và sửa đơn), `NoteRequest`, `RejectRequest`, `ScheduleRequest`, `ConfirmRequest` (OTP đúng 6 chữ số), `FailRequest`, `OrderResponse`, `OrderSummaryResponse`, `OrderPageResponse`, `OrderStatusHistoryResponse`, `EtaResponse` | `OrdersController` |
| `UserSchemas.cs` | `CreateUserRequest`, `SetRolesRequest`, `SetActiveRequest`, `UserResponse` | `UsersController` |
| `NotificationSchemas.cs` | `NotificationResponse` | `NotificationsController` |
| `ReportSchemas.cs` | `ReportResponse` | `ReportsController` |
| `DroneSchemas.cs` | `DroneRequest` (mã ≤ 30, model ≤ 100, tải 0.1–50 kg, tầm 0.1–500 km, vận tốc 1–200 km/h, pin 0–100, trạng thái, trạm hiện tại), `DroneResponse` | `DronesController` |
| `CustomerAddressSchemas.cs` | `CustomerAddressRequest` (nhãn ≤ 50, địa chỉ ≤ 255, tọa độ tùy chọn, `isDefault`), `CustomerAddressResponse` | `CustomerAddressesController` |
| `AiSchemas.cs` | `PromptRequest`, `ChatRequest`, `PromptResponse` (model, prompt, answer, `latencyMs`, usage), `ChatResponse` | `AiController` |

Thêm trường mới vào API: sửa schema ở đây + `docs/openapi.yaml`, báo người làm web/mobile.
