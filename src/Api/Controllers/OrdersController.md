# OrdersController.cs

Endpoint quản lý đơn giao hàng. Mọi logic nằm ở `Services/OrderService.cs`, file này chỉ nối HTTP với service.

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
