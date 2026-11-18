# Api/Schemas

Hình dạng JSON vào/ra của API (tương đương Marshmallow schema bên Flask). Tách riêng khỏi model Domain để đổi API không ảnh hưởng nghiệp vụ và ngược lại.

Quy ước:
- `XxxRequest`: dữ liệu client gửi lên. Validate bằng attribute (`[Required]`, `[MaxLength]`, `[Range]`, `[EmailAddress]`); sai thì `[ApiController]` tự trả 400 trước khi vào controller. Có hàm `ToOrder()` / `ToDomain()` để đổi sang model Domain.
- `XxxResponse`: dữ liệu trả về. Có hàm tĩnh `From(model)` đổi từ model Domain.
- JSON dùng camelCase (`trackingCode`), enum là chuỗi, thời gian UTC ISO-8601.

| File | Schema | Dùng ở |
|---|---|---|
| `AuthSchemas.cs` | `RegisterRequest` (email, mật khẩu ≥ 8, họ tên), `LoginRequest`, `AuthResponse` (token, `expiresAtUtc`, `userId`, email, `fullName`, roles), `ProfileRequest`, `ProfileResponse` (hồ sơ cá nhân, v0.11; thay `MeResponse` cũ), `ChangePasswordRequest` (v0.11) | `AuthController` |
| `StationSchemas.cs` | `StationRequest` (tên ≤ 200, vĩ độ −90..90, kinh độ −180..180, trạng thái, sức chứa 1–50), `StationResponse` (có `capacity`), `StationLoadResponse` (v0.11) | `StationsController` |
| `OrderSchemas.cs` | `OrderRequest` + `PackageInput` (dùng chung cho tạo và sửa đơn), `NoteRequest`, `RejectRequest`, `ScheduleRequest`, `ConfirmRequest` (OTP đúng 6 chữ số), `FailRequest`, `OrderResponse`, `OrderSummaryResponse`, `OrderPageResponse`, `OrderStatusHistoryResponse`, `EtaResponse`, `OrderDetailResponse` + `StationRef` + `PackageResponse` (chi tiết đơn đúng spec, v0.7), `TrackingResponse` + `StationPoint` (vị trí drone, v0.9); v0.11: `OrderRequest` thêm `recipientName`, `recipientPhone` (không bắt buộc), chi tiết và danh sách đơn trả người nhận | `OrdersController` |
| `PackageSchemas.cs` | `ImageUploadResponse { imageKey }` – kết quả upload ảnh kiện (v0.8) | `PackagesController` |
| `UserSchemas.cs` | `CreateUserRequest`, `SetRolesRequest`, `SetActiveRequest`, `UserResponse` | `UsersController` |
| `CustomerSchemas.cs` | (v0.11) `CustomerResponse` (hồ sơ + số đơn), `CustomerDetailResponse` (kèm địa chỉ và đơn gần đây) | `CustomersController` |
| `NotificationSchemas.cs` | `NotificationResponse` | `NotificationsController` |
| `AuditSchemas.cs` | (v0.11) `AuditLogResponse`, `AuditPageResponse` | `AuditLogsController` |
| `ReportSchemas.cs` | `ReportResponse`, `StationReportResponse` (v0.11) | `ReportsController` |
| `DroneSchemas.cs` | `DroneRequest` (mã ≤ 30, model ≤ 100, tải 0.1–50 kg, tầm 0.1–500 km, vận tốc 1–200 km/h, pin 0–100, trạng thái, trạm hiện tại), `DroneResponse` | `DronesController` |
| `CustomerAddressSchemas.cs` | `CustomerAddressRequest` (nhãn ≤ 50, địa chỉ ≤ 255, tọa độ tùy chọn, `isDefault`), `CustomerAddressResponse` | `CustomerAddressesController` |
| `AiSchemas.cs` | `PromptRequest`, `ChatRequest`, `PromptResponse` (model, prompt, answer, `latencyMs`, usage), `ChatResponse`; v0.10: `AiSummaryResponse`, `AiInsightsResponse` (có `source`, `model`, `note`) | `AiController`, `OrdersController`, `ReportsController` |

Thêm trường mới vào API: sửa schema ở đây + `docs/openapi.yaml`, báo người làm web/mobile.
