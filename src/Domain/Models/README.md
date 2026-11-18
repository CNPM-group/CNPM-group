# Domain/Models

Model nghiệp vụ và interface (tương đương `domain/models` của repo mẫu, nơi có `todo.py` và `itodo_repository.py`).

Model là class thuần, không biết mình được lưu ở đâu. Cách map xuống bảng nằm ở `Infrastructure/Models/<Tên>Configuration.cs`. Id dạng `Guid`, tạo sẵn khi `new` (trừ bảng log dùng `long` tự tăng).

## Model nghiệp vụ

| File | Bảng | Ý nghĩa | Đang dùng |
|---|---|---|---|
| `User.cs` | `users` | Người dùng ở mức nghiệp vụ (id, email, họ tên, SĐT, còn hoạt động) + record `UserWithRoles`. Bản lưu CSDL là `AppUser` ở Infrastructure | Auth, quản lý tài khoản |
| `LandingStation.cs` | `landing_stations` | Trạm hạ cánh: tên, sức chứa (mặc định 2), tọa độ, trạng thái | Stations, Orders, AI |
| `StationRoute.cs` | `station_routes` | Tuyến bay có hướng giữa 2 trạm, khoảng cách km | Seed (20 tuyến); dùng khi làm lập lịch |
| `Drone.cs` | `drones` | Mã, model, tải tối đa, tầm bay, vận tốc, % pin, trạng thái, trạm hiện tại | API drone, kiểm tra tải khi tạo đơn |
| `Package.cs` | `packages` | Kiện hàng: mô tả, cân nặng, kích thước, khóa ảnh trên MinIO | Orders |
| `CustomerAddress.cs` | `customer_addresses` | Địa chỉ giao của khách, có cờ mặc định | Customer addresses, Orders |
| `DeliveryOrder.cs` | `delivery_orders` | Đơn giao: mã tra cứu, khách, kiện, trạm gửi/nhận, trạng thái, người duyệt, lịch, ETA, lý do thất bại, người nhận (`RecipientName`, `RecipientPhone`, v0.11) | Orders, AI |
| `OrderStatusHistory.cs` | `order_status_history` | Mỗi lần đổi trạng thái một dòng: từ → đến, ai đổi, ghi chú, lúc nào | Orders (timeline) |
| `DroneFlight.cs` | `drone_flights` | Chuyến bay của drone cho một chặng của đơn | Sprint sau (gán drone, bay) |
| `FlightTelemetry.cs` | `flight_telemetry` | Điểm vị trí/pin/độ cao theo thời gian của chuyến | Sprint sau (theo dõi realtime) |
| `DeliveryConfirmation.cs` | `delivery_confirmations` | Mã OTP nhận hàng (lưu hash), hạn, số lần nhập sai | Sprint sau (xác nhận giao) |
| `Notification.cs` | `notifications` | Thông báo cho người dùng | Sprint sau |
| `StationOperator.cs` | `station_operators` | Gán nhân viên trạm vào trạm | Sprint sau |
| `AuditLog.cs` | `audit_logs` | Ai làm gì, giá trị cũ/mới | Sprint sau |
| `AiChatMessage.cs` | `ai_chat_messages` | Lịch sử chat với trợ lý AI | Sprint sau |
| `ApiKey.cs` | `api_keys` | API key cho hệ thống ngoài (lưu hash) | Sprint sau |

## Luật nghiệp vụ

| File | Ý nghĩa |
|---|---|
| `OrderStateMachine.cs` | Bảng chuyển trạng thái đơn hợp lệ (khớp mục IV.3.1 của báo cáo). `CanTransition(from, to)` trả đúng/sai; `EnsureTransition` ném `ConflictException` (409) khi chuyển sai. Mọi service đổi trạng thái đơn phải gọi hàm này. Chi tiết: [OrderStateMachine.md](OrderStateMachine.md) |

```
Pending          → Approved | Rejected | Cancelled
Approved         → Scheduled
Scheduled        → InTransit
InTransit        → ArrivedAtStation | Failed
ArrivedAtStation → Completed | Failed
Failed           → Scheduled (giao lại) | Cancelled (hoàn trả)
Rejected, Completed, Cancelled: trạng thái cuối
```

## Model kết quả (không có bảng)

| File | Ý nghĩa |
|---|---|
| `AuthSession.cs` | Kết quả đăng nhập/đăng ký: token, hạn, user, roles |
| `OrderSummary.cs` | Dòng tóm tắt đơn cho danh sách (kèm tên trạm, người nhận từ v0.11) + `PagedResult<T>` cho phân trang |
| `CustomerModels.cs` | (v0.11) `CustomerOrderStats` (tổng / hoàn tất / đang xử lý / ngày đặt gần nhất), `CustomerSummary`, `CustomerDetail` cho màn Khách hàng |
| `IAuditLogRepository.cs` | (v0.11) Đọc nhật ký thao tác (`AuditEntry` = dòng `AuditLog` + email người làm). Ghi nhật ký không qua interface này mà do interceptor ở Infrastructure |
| `StationLoad.cs` | (v0.11) `StationLoad` (đơn đang tới, kiện chờ nhận, đơn sắp đi của 1 trạm) và `StationReport` (hiệu quả 1 trạm trong khoảng ngày) |
| `IOrderEvents.cs` | Interface báo "đơn vừa đổi trạng thái" (`StatusChangedAsync`) và "drone vừa dời vị trí" (`DroneMovedAsync`, v0.9); Infrastructure cài bằng SignalR (`Realtime/SignalROrderEvents.cs`) |
| `TrackingSnapshot.cs` | Ảnh chụp vị trí 1 đơn đang giao: tọa độ drone, % đã bay, tọa độ 2 trạm, giờ cất cánh, ETA (v0.9). `TrackingService` tạo ra, API `/orders/{id}/tracking` và event `DroneLocation` dùng |
| `IFileStorage.cs` | Interface lưu/đọc file + record `StoredFile`; Infrastructure cài bằng MinIO (`Services/MinioFileStorage.cs`) |
| `OrderDetail.cs` | Gói đơn + kiện hàng + trạm gửi + trạm nhận, dùng cho `GET /orders/{id}` |
| `EtaEstimate.cs` | Kết quả ước tính ETA (km, phút) + `GeoDistance.HaversineKm` tính khoảng cách 2 tọa độ trên mặt cầu |
| `OtpCode.cs` | Sinh mã OTP 6 số ngẫu nhiên (RandomNumberGenerator), băm SHA-256 kèm id đơn, hạn 24 giờ, tối đa 5 lần sai |
| `ReportModels.cs` | Kết quả thống kê (`OrderReport`, `StatusCount`, `DailyCount`), dòng xuất CSV, và interface `IReportRepository` |
| `AiAnswer.cs` | Câu trả lời của LLM: model, nội dung, độ trễ, token usage |
| `AiInsight.cs` | (v0.10) `AiSummary` (tóm tắt 1 chuyến giao), `AiInsights` (nhận xét vận hành), hằng `AiSource.Llm` / `AiSource.Rule` cho biết ai viết nội dung |

## Interface (Infrastructure cài đặt)

| Interface | Cài đặt | Hàm |
|---|---|---|
| `IUserRepository.cs` | `Infrastructure/Repositories/UserRepository.cs` | tìm theo email/id, liệt kê, kiểm tra mật khẩu, tạo user + gán role, lấy/đặt lại roles, khóa/mở; v0.11: `ListInRoleAsync` (khách hàng), `UpdateProfileAsync` (họ tên, số điện thoại) |
| `IStationRepository.cs` | `StationRepository.cs` | list, get, exists, add, update, delete; `LoadAsync` (v0.11) |
| `IOrderRepository.cs` | `OrderRepository.cs` | get (có lọc theo khách), get kiện, phân trang, lịch sử, add (đơn + kiện + lịch sử), update (+ lịch sử); v0.11: phân trang có `search` theo mã đơn, `StatsByCustomerAsync`, `CountIncomingAsync` (kiểm sức chứa trạm) |
| `ICustomerAddressRepository.cs` | `CustomerAddressRepository.cs` | list theo khách, get, kiểm tra sở hữu, kiểm tra đã có mặc định, bỏ mặc định ở các địa chỉ khác (`ClearDefaultAsync`, v0.8), add, update, delete |
| `IDroneRepository.cs` | `DroneRepository.cs` | list, get, kiểm tra trùng mã, có drone nào chở được cân nặng X không, add, update, delete |
| `IDeliveryRepository.cs` | `DeliveryRepository.cs` | đếm chặng bay, lấy chặng mới nhất, lấy OTP của đơn, thêm chuyến bay, thêm OTP (lưu cùng lúc khi `IOrderRepository.UpdateAsync`), lưu vị trí drone + 1 dòng telemetry (`SavePositionAsync`, v0.9) |
| `INotificationRepository.cs` | `NotificationRepository.cs` | list thông báo của user, get, thêm, lưu |
| `IReportRepository.cs` | `ReportRepository.cs` | đơn trong khoảng ngày, dòng xuất CSV |
| `ITokenService.cs` | `Infrastructure/Services/JwtTokenService.cs` | tạo JWT |
| `ILlmClient.cs` | `Infrastructure/Services/LlmClient.cs` | gọi chat completion của LLM |

Service chỉ biết các interface này, nên test service có thể thay bằng bản giả (fake) mà không cần PostgreSQL.
