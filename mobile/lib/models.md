# models.dart

Các lớp đọc JSON từ API (`fromJson`). Tên trường khớp với response của backend (camelCase).

| Lớp | Dùng cho |
|---|---|
| `AuthData` | Kết quả đăng nhập: token, hạn, họ tên, roles |
| `Station` | Trạm hạ cánh (chọn khi tạo đơn) |
| `PackageInfo` | Kiện hàng: cân nặng, kích thước, `imageKey` |
| `Order`, `OrderSummary`, `OrderPage` | Chi tiết đơn (có tên trạm, kiện), dòng danh sách, trang phân trang |
| `HistoryItem` | Một mốc trong lịch sử trạng thái |
| `AppNotification` | Thông báo; getter `otp` tách mã 6 số trong nội dung (v0.7) |
| `CustomerAddress` | Địa chỉ trong sổ địa chỉ (v0.8) |
| `StationPoint`, `TrackingInfo` | Vị trí drone + 2 trạm cho bản đồ mini; `moved(event)` áp vị trí mới từ SignalR (v0.9) |
| `Order.recipientName`, `recipientPhone` | Người nhận của đơn (v0.11) |
| `Order.originStationId`, `destinationStationId`, `deliveryAddressId` | Id để điền sẵn khi sửa đơn (v0.11) |
| `UserProfile` | Hồ sơ: id, email, họ tên, số điện thoại (v0.11) |
| `AiSummary` | Tóm tắt chuyến giao: `text`, `source`, `model`, `note`; getter `byLlm` = AI viết hay server tự ghép (v0.10) |
