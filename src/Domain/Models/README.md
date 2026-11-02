# Domain/Models

Model nghiệp vụ và interface (tương đương `domain/models` của repo mẫu, nơi có `todo.py` và `itodo_repository.py`).

Model là class thuần, không biết mình được lưu ở đâu. Cách map xuống bảng nằm ở `Infrastructure/Models/<Tên>Configuration.cs`. Id dạng `Guid`, tạo sẵn khi `new` (trừ bảng log dùng `long` tự tăng).

## Model nghiệp vụ

| File | Bảng | Ý nghĩa | Đang dùng |
|---|---|---|---|
| `User.cs` | `users` | Người dùng ở mức nghiệp vụ (id, email, họ tên, SĐT, còn hoạt động). Bản lưu CSDL là `AppUser` ở Infrastructure | Auth |
| `LandingStation.cs` | `landing_stations` | Trạm hạ cánh: tên, sức chứa (mặc định 2), tọa độ, trạng thái | Stations, Orders, AI |
| `StationRoute.cs` | `station_routes` | Tuyến bay có hướng giữa 2 trạm, khoảng cách km | Seed (20 tuyến); dùng khi làm lập lịch |
| `Drone.cs` | `drones` | Mã, model, tải tối đa, tầm bay, vận tốc, % pin, trạng thái, trạm hiện tại | Kiểm tra tải khi tạo đơn |
| `Package.cs` | `packages` | Kiện hàng: mô tả, cân nặng, kích thước, khóa ảnh trên MinIO | Orders |
| `CustomerAddress.cs` | `customer_addresses` | Địa chỉ giao của khách, có cờ mặc định | Customer addresses, Orders |
| `DeliveryOrder.cs` | `delivery_orders` | Đơn giao: mã tra cứu, khách, kiện, trạm gửi/nhận, trạng thái, người duyệt, lịch, ETA, lý do thất bại | Orders, AI |
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
| `OrderStateMachine.cs` | Bảng chuyển trạng thái đơn hợp lệ (khớp mục IV.3.1 của báo cáo). `CanTransition(from, to)` trả đúng/sai; `EnsureTransition` ném `ConflictException` (409) khi chuyển sai. Mọi service đổi trạng thái đơn phải gọi hàm này. |

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
| `OrderSummary.cs` | Dòng tóm tắt đơn cho danh sách (kèm tên trạm) + `PagedResult<T>` cho phân trang |
| `AiAnswer.cs` | Câu trả lời của LLM: model, nội dung, độ trễ, token usage |

## Interface (Infrastructure cài đặt)

| Interface | Cài đặt | Hàm |
|---|---|---|
| `IUserRepository` | `Infrastructure/Repositories/UserRepository.cs` | tìm theo email, kiểm tra mật khẩu, tạo user + gán role, lấy roles |
| `IStationRepository` | `StationRepository.cs` | list, get, exists, add, update, delete |
| `IOrderRepository` | `OrderRepository.cs` | get (có lọc theo khách), get kiện, phân trang, lịch sử, add (đơn + kiện + lịch sử), update (+ lịch sử) |
| `ICustomerAddressRepository` | `CustomerAddressRepository.cs` | list theo khách, get, kiểm tra sở hữu, kiểm tra đã có mặc định, add, update, delete |
| `IDroneRepository` | `DroneRepository.cs` | có drone nào chở được cân nặng X không |
| `ITokenService` | `Infrastructure/Services/JwtTokenService.cs` | tạo JWT |
| `ILlmClient` | `Infrastructure/Services/LlmClient.cs` | gọi chat completion của LLM |

Service chỉ biết các interface này, nên test service có thể thay bằng bản giả (fake) mà không cần PostgreSQL.
