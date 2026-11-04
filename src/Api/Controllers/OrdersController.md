# OrdersController.cs

Endpoint quản lý đơn giao hàng. File này chỉ nối HTTP với service, không chứa luật:

- Tạo, sửa, hủy, xem, duyệt, từ chối → `Services/OrderService.cs` ([OrderService.md](../../Services/OrderService.md))
- Gán drone, cất cánh, tới trạm, OTP, giao thất bại → `Services/DeliveryService.cs` ([DeliveryService.md](../../Services/DeliveryService.md))
- Ước tính ETA → `Services/EtaService.cs`

Luật đổi trạng thái nằm ở `Domain/Models/OrderStateMachine.cs` ([OrderStateMachine.md](../../Domain/Models/OrderStateMachine.md)). Gọi sai bước (ví dụ `start` khi đơn còn `Pending`) → **409**.

## Bảng nhanh

| Method | Đường dẫn | Ai gọi | Body | Đổi trạng thái |
|---|---|---|---|---|
| GET | `/orders/eta` | Customer | query `originStationId`, `destinationStationId`, `weightKg` | – |
| POST | `/orders` | Customer | `OrderRequest` | → `Pending` |
| GET | `/orders` | Mọi người (khách chỉ thấy đơn mình) | query `page`, `pageSize`, `status` | – |
| GET | `/orders/{id}` | Chủ đơn, nhân viên | – | – |
| PUT | `/orders/{id}` | Customer | `OrderRequest` | – (chỉ khi `Pending`) |
| POST | `/orders/{id}/cancel` | Customer | – | `Pending` → `Cancelled` |
| POST | `/orders/{id}/approve` | Dispatcher, Manager, Admin | `{ "note": "..." }` (không bắt buộc) | `Pending` → `Approved` |
| POST | `/orders/{id}/reject` | Dispatcher, Manager, Admin | `{ "reason": "..." }` | `Pending` → `Rejected` |
| POST | `/orders/{id}/schedule` | Dispatcher, Manager, Admin | `{ "droneId": "uuid", "scheduledAtUtc": null }` | `Approved`/`Failed` → `Scheduled` |
| POST | `/orders/{id}/start` | Dispatcher, Manager, Admin | – | `Scheduled` → `InTransit` |
| POST | `/orders/{id}/arrive` | Dispatcher, Station Operator, Manager, Admin | – | `InTransit` → `ArrivedAtStation` |
| POST | `/orders/{id}/confirm` | Dispatcher, Station Operator, Manager, Admin | `{ "otp": "123456" }` | `ArrivedAtStation` → `Completed` |
| POST | `/orders/{id}/fail` | Dispatcher, Manager, Admin | `{ "reason": "..." }` | `InTransit`/`ArrivedAtStation` → `Failed` |
| GET | `/orders/{id}/history` | Chủ đơn, nhân viên | – | – |

Mọi endpoint đổi trạng thái trả về `OrderResponse` của đơn sau khi đổi.

## Phạm vi dữ liệu

- `User.UserId()`: dùng cho thao tác chỉ khách được làm (tạo, sửa, hủy). Service tự kiểm tra đơn có thuộc khách đó không.
- `User.CustomerScope()`: dùng cho thao tác xem. Nhân viên (Dispatcher, Manager, Admin) → `null` = xem mọi đơn; khách → id của mình. Đơn của người khác trả **404** chứ không phải 403, để không lộ là đơn đó có tồn tại.

## POST /api/v1/orders

```json
{
  "originStationId": "uuid trạm gửi",
  "destinationStationId": "uuid trạm nhận",
  "deliveryAddressId": null,
  "notes": "Giao giờ hành chính",
  "package": {
    "description": "Sách",
    "weightKg": 1.2,
    "lengthCm": 30, "widthCm": 20, "heightCm": 10,
    "imageKey": null
  }
}
```

| Kết quả | Khi nào |
|---|---|
| 201 + `OrderResponse` | Tạo được |
| 400 | Thiếu trường / sai khoảng (`weightKg` 0.01–50); trạm trùng hoặc không tồn tại; địa chỉ không phải của mình; không drone nào chở nổi |
| 401 / 403 | Chưa đăng nhập / không phải Customer |

## GET /api/v1/orders

Query: `page` (từ 1), `pageSize` (mặc định 20, tối đa 100), `status` (một giá trị của `OrderStatus`, không phân biệt hoa thường; sai → 400).

Trả `OrderPageResponse`: `items` (mã đơn, trạng thái, tên trạm gửi/nhận, ETA, ngày tạo), `page`, `pageSize`, `totalItems`. Mới nhất trước.

## PUT /api/v1/orders/{id}

Body giống POST. Chỉ khi đơn còn `Pending`, nếu không → 409. Sửa cả đơn lẫn kiện hàng, không ghi lịch sử (trạng thái không đổi).

## POST /api/v1/orders/{id}/cancel

Không có body. `Pending` → `Cancelled`, ghi một dòng lịch sử "Khách hàng hủy đơn hàng.". Trạng thái khác → 409.

## GET /api/v1/orders/{id}/history

Danh sách `fromStatus`, `toStatus`, `changedBy` (id người đổi), `note`, `changedAtUtc`, cũ nhất trước. Mobile dùng để vẽ timeline.

## GET /api/v1/orders/eta

Query: `originStationId`, `destinationStationId`, `weightKg` (0.01–50). Trả `{ "distanceKm": 7.42, "estimatedMinutes": 11.7 }`. Trạm không tồn tại → 404; khối lượng sai → 400. Công thức ở `Services/README.md` mục EtaService.

## POST /api/v1/orders/{id}/approve · /reject

- `approve`: body không bắt buộc `{ "note": "..." }`, ghi người duyệt + giờ duyệt, lịch sử ghi note (mặc định "Đơn đã được duyệt.").
- `reject`: body bắt buộc `{ "reason": "..." }` (1–1000 ký tự), thiếu → 400.
- Đơn không ở `Pending` → 409.

## POST /api/v1/orders/{id}/schedule

```json
{ "droneId": "uuid drone", "scheduledAtUtc": "2026-11-06T02:00:00Z" }
```

`scheduledAtUtc` bỏ trống = bay ngay. Lỗi:

| Mã | Khi nào |
|---|---|
| 400 | Giờ bay ở quá khứ; kiện nặng hơn tải tối đa của drone |
| 404 | Không có drone / đơn |
| 409 | Drone không `Idle`; đơn không ở `Approved` hoặc `Failed` |

## POST /api/v1/orders/{id}/start · /arrive

Không có body. `start` cất cánh (drone → `InFlight`), `arrive` hạ cánh (drone → `Idle`) và sinh OTP gửi vào thông báo của khách. Đơn chưa có chuyến bay đúng trạng thái → 409.

## POST /api/v1/orders/{id}/confirm

```json
{ "otp": "482913" }
```

Không đúng 6 chữ số → 400. Sai mã → 400 kèm "còn N lần thử". Sai quá 5 lần hoặc mã quá 24 giờ → 409. Đúng → `Completed`.

## POST /api/v1/orders/{id}/fail

```json
{ "reason": "Gió mạnh, drone quay về" }
```

Bắt buộc lý do. Chuyến đang bay → `Aborted`, drone về `Idle`, khách nhận thông báo. Giao lại: gọi `schedule` với drone khác.
