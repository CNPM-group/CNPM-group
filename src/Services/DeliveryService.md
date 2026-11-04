# DeliveryService.cs

Luồng giao hàng sau khi đơn được duyệt. Mọi bước đổi trạng thái đều đi qua `OrderStateMachine` (sai bước → 409), ghi một dòng `OrderStatusHistory`, và lưu tất cả trong **một** lần `SaveChanges`.

| Hàm | Trạng thái | Việc làm |
|---|---|---|
| `ScheduleAsync(staff, order, droneId, giờ bay?)` | `Approved`/`Failed` → `Scheduled` | Drone phải `Idle` và chở nổi kiện. Tạo `DroneFlight` (chặng = số chặng cũ + 1), drone → `Assigned`, tính `EtaUtc` = giờ bay + phút ETA của `EtaService` |
| `StartAsync(staff, order)` | `Scheduled` → `InTransit` | Chuyến bay → `Flying`, ghi giờ cất cánh thật; drone → `InFlight`; dời ETA theo giờ cất cánh thật; báo khách |
| `ArriveAsync(staff, order)` | `InTransit` → `ArrivedAtStation` | Chuyến bay → `Landed`; drone → `Idle`, đậu ở trạm nhận; sinh OTP 6 số, lưu **hash** vào `delivery_confirmations` (hạn 24 giờ); gửi OTP vào thông báo của khách |
| `ConfirmAsync(staff, order, otp)` | `ArrivedAtStation` → `Completed` | So hash OTP. Sai → +1 lần sai, báo còn mấy lần (tối đa 5). Quá 5 lần hoặc hết hạn → 409. Đúng → ghi người xác nhận, `CompletedAtUtc`, báo khách |
| `FailAsync(staff, order, reason)` | `InTransit`/`ArrivedAtStation` → `Failed` | Bắt buộc lý do (ràng buộc `ck_order_failure` của CSDL). Chuyến đang bay → `Aborted`, drone → `Idle`. Sau đó điều phối gọi lại `schedule` để giao lại |

OTP chỉ lưu dạng SHA-256 (`Domain/Models/OtpCode.cs`), nhân viên trạm không xem được mã; khách lấy mã ở `GET /api/v1/notifications`.

Chưa có (làm sau): ảnh bằng chứng giao (MinIO), vị trí drone realtime (SignalR + bộ giả lập, KAN-55).
