# order_detail_page.dart

Màn **Chi tiết đơn**.

| Phần | Nội dung | Từ bản |
|---|---|---|
| Đầu trang | Mã đơn + nhãn trạng thái | v0.5 |
| Theo dõi drone | Bản đồ mini `TrackingMap` + thanh tiến độ, chỉ hiện khi đơn Đang giao hoặc Đã tới trạm. Lấy bằng `GET /orders/{id}/tracking`, sau đó chấm drone dời theo event SignalR `DroneLocation` | v0.9 |
| Thông tin đơn | Trạm đi, trạm đến, người nhận (v0.11), ETA, ngày tạo, ghi chú, lý do thất bại | v0.5 |
| Kiện hàng | Mô tả, cân nặng, kích thước, ảnh kiện (`GET /orders/{id}/package-image` kèm token) | v0.5, ảnh v0.8 |
| Tóm tắt chuyến giao | Chỉ hiện khi đơn đã kết thúc (Hoàn tất, Thất bại, Đã hủy, Bị từ chối). Bấm "Nhờ AI tóm tắt" → `POST /orders/{id}/ai-summary`, hiện 3–4 câu + dòng nhỏ "AI viết · model" hoặc "Tự ghép từ dữ liệu (chưa dùng AI)". Có nút Viết lại. Không tự gọi khi mở màn, vì mỗi lần là 1 lần gọi LLM | v0.10 |
| Lịch sử | Timeline các mốc trạng thái (`/orders/{id}/history`) | v0.5 |
| Nút Sửa đơn | Chỉ hiện khi đơn còn Chờ duyệt: mở `CreateOrderPage(editing: order)` điền sẵn dữ liệu cũ, lưu bằng `PUT /orders/{id}` (FR05 "cập nhật đơn") | v0.11 |
| Nút Hủy đơn | Chỉ hiện khi đơn còn Chờ duyệt, hỏi lại trước khi hủy | v0.5 |

Tự tải lại khi có event đổi trạng thái của đúng đơn này (hoặc mỗi 10 giây khi không nối được hub).
