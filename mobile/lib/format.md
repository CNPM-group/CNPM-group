# format.dart

Hàm hiển thị dùng chung:

| Hàm | Làm gì |
|---|---|
| `formatDate(DateTime?)` | Đổi giờ UTC từ server sang giờ máy, dạng `dd/MM/yyyy HH:mm`; `null` → `-` |
| `statusLabel(status)` | Mã trạng thái → tiếng Việt (InTransit → "Đang giao"...) |
| `statusColor(status)` | Màu theo trạng thái: cam = chờ duyệt, xanh dương = đã duyệt/tới trạm, tím = lên lịch/đang giao, xanh lá = hoàn tất, đỏ = từ chối/thất bại, xám = đã hủy |
