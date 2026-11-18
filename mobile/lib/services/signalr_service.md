# signalr_service.dart

Nhận cập nhật trạng thái đơn theo thời gian thực. Dùng chung một kết nối cho cả app: `SignalRService.instance`.

## Vòng đời

| Lúc | Gọi | Làm gì |
|---|---|---|
| Vào trang "Đơn của tôi" (sau đăng nhập) | `connect()` | Nối hub `<gốc API>/hubs/delivery`, gửi token đăng nhập qua `accessTokenFactory`. Đã nối rồi thì bỏ qua |
| Đăng xuất (`AuthStore.logout`) | `disconnect()` | Ngắt kết nối, tắt làm mới |

## Stream `drone` (v0.9)

Vị trí drone từ event `DroneLocation` của backend: `{ orderId, trackingCode, droneCode, latitude, longitude, progressPct, etaUtc }`, mỗi vài giây khi đơn đang giao. `order_detail_page.dart` nghe stream này để dời chấm drone trên `TrackingMap`. Không nối được hub thì không có event; màn chi tiết vẫn cập nhật vị trí mỗi 10 giây vì lần tải lại nào cũng gọi `GET /orders/{id}/tracking`.

## Stream `updates`

Mỗi phần tử là một `Map`:

- Event thật từ hub `OrderStatusUpdated`: `{ orderId, trackingCode, status, updatedAt }`.
- Event rỗng `{}`: phát mỗi 10 giây khi **không** nối được hub (hub chưa làm, server tắt, mất mạng). Màn nào nghe thấy cũng tự tải lại.

Màn nào cần thì `listen` trong `initState` và `cancel` trong `dispose`:

| Màn | Xử lý |
|---|---|
| `orders_page.dart` | Tải lại danh sách + số thông báo chưa đọc; event thật thì hiện SnackBar "Đơn SDD-...: Đang giao" |
| `order_detail_page.dart` | Tải lại nếu event là của đơn đang xem (hoặc event rỗng) |
| `notifications_page.dart` | Tải lại danh sách thông báo |

## Vì sao cần chế độ làm mới 10 giây

Từ v0.8 backend đã có hub (`src/Infrastructure/Realtime/`), khách chỉ nhận event của đơn mình. Chế độ làm mới 10 giây vẫn giữ để app không đứng hình khi mất kết nối hub. Khi hub chạy, `connect()` thành công thì tự tắt làm mới; mất kết nối (`onclose`) thì bật lại; nối lại được (`onreconnected`) thì tắt.

URL hub và tên event nằm ở `signalr_config.dart`, phải khớp với backend và web (`web/src/orderEvents.js`).
