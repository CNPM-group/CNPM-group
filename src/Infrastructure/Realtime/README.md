# Infrastructure/Realtime

Đẩy cập nhật trạng thái đơn tới web và mobile ngay lúc đổi, không cần client hỏi lại (KAN-55). Dùng SignalR có sẵn trong ASP.NET Core, không cài thêm gói.

| File | Là gì |
|---|---|
| `DeliveryHub.cs` | Hub ở đường dẫn `/hubs/delivery`. Bắt buộc đăng nhập (`[Authorize]`) |
| `SignalROrderEvents.cs` | Cài đặt `IOrderEvents` (khai báo ở `Domain/Models`): gửi event `OrderStatusUpdated` qua hub |

## Ai nhận event nào

Lúc kết nối (`OnConnectedAsync`), hub xếp kết nối vào nhóm:

| Người kết nối | Vào nhóm | Nhận event của |
|---|---|---|
| Dispatcher, Station Operator, Manager, Admin | `staff` | Mọi đơn |
| Mọi người | `user-{id}` (id lấy từ claim `sub` của token) | Đơn mà mình là khách |

Khi đơn đổi trạng thái, event gửi tới 2 nhóm `staff` và `user-{CustomerId của đơn}`. Khách A không bao giờ nhận được event đơn của khách B.

## Event `OrderStatusUpdated`

```json
{ "orderId": "guid", "trackingCode": "SDD-...", "status": "InTransit", "updatedAt": "2026-11-15T08:00:00Z" }
```

Event chỉ báo "đơn này vừa đổi". Client nhận xong tự gọi lại API để lấy dữ liệu đầy đủ, nên không lộ thêm thông tin nào qua hub.

## Event `DroneLocation` (v0.9)

```json
{ "orderId": "guid", "trackingCode": "SDD-...", "droneCode": "DRONE-001", "latitude": 10.851, "longitude": 106.773, "progressPct": 42.5, "etaUtc": "..." }
```

Do `Services/DroneSimulator.cs` gửi vài giây một lần cho mỗi đơn đang bay, qua `IOrderEvents.DroneMovedAsync`, cùng 2 nhóm như event trạng thái (`staff` + khách chủ đơn).

## Được gửi ở đâu

`OrderService` (tạo, hủy, duyệt, từ chối) và `DeliveryService` (gán drone, cất cánh, tới trạm, xác nhận OTP, thất bại, hủy và hoàn) gọi `events.StatusChangedAsync(order)` **ngay sau** `orders.UpdateAsync(...)`, tức là sau khi `SaveChanges` thành công.

- Gửi sau khi lưu: client tải lại sẽ thấy đúng trạng thái mới. Gửi trước mà lưu lỗi thì client thấy trạng thái không có thật.
- Gửi lỗi (ví dụ client vừa rớt mạng) chỉ ghi log cảnh báo, không làm hỏng thao tác của người bấm, vì đơn đã lưu xong.

## Đăng nhập qua WebSocket

Trình duyệt không gắn được header `Authorization` khi mở WebSocket, nên client gửi token qua query `?access_token=...`. `CreateApp.cs` (`OnMessageReceived` của JwtBearer) chỉ đọc query này khi đường dẫn bắt đầu bằng `/hubs`, các API thường vẫn bắt buộc dùng header.

## Client

- Web: `web/src/orderEvents.js`
- Mobile: `mobile/lib/services/signalr_service.dart`

Cả hai tự chuyển sang hỏi lại API mỗi 10 giây nếu không nối được hub.
