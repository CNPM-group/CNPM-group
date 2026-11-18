# DroneSimulator.cs

Dịch vụ chạy nền (`BackgroundService`) cùng API, bắt đầu khi app khởi động (v0.9).

## Mỗi lượt (mặc định 3 giây)

1. Tạo scope DI mới (vì `TrackingService` và DbContext là Scoped, còn dịch vụ nền sống suốt đời app).
2. Gọi `TrackingService.MoveFlyingDronesAsync()`: dời mọi drone đang bay, lưu vị trí + telemetry vào DB.
3. Với mỗi vị trí mới, gọi `IOrderEvents.DroneMovedAsync` → SignalR gửi event `DroneLocation` cho nhóm `staff` và đúng khách chủ đơn.
4. Lượt nào lỗi (DB tạm mất kết nối...) thì ghi log cảnh báo rồi chờ lượt sau, không làm sập API.

## Cấu hình (`appsettings.json`, mục `Simulator`)

| Khóa | Mặc định | Ý nghĩa |
|---|---|---|
| `Enabled` | `true` | Tắt (`false`) nếu sau này nối drone/GPS thật |
| `IntervalSeconds` | `3` | Bao nhiêu giây một lượt (tối thiểu 1) |
| `TimeScale` | `1` | Hệ số tua nhanh khi demo, xem `TrackingService.md` |

Trong Docker đổi bằng biến môi trường, ví dụ `Simulator__TimeScale=10`.

## Event `DroneLocation`

```json
{ "orderId": "guid", "trackingCode": "SDD-...", "droneCode": "DRONE-001",
  "latitude": 10.851, "longitude": 106.773, "progressPct": 42.5, "etaUtc": "2026-11-15T14:04:00Z" }
```

Web (`Station.jsx`) dùng để chạy thanh tiến độ, app (`order_detail_page.dart`) dùng để dời chấm drone trên bản đồ mini.
