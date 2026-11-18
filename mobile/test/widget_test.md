# widget_test.dart

Test đơn vị của app mobile, chạy bằng `flutter test` (CI job `mobile` chạy tự động mỗi lần push).

| Test | Kiểm tra gì |
|---|---|
| `statusLabel ...` | Đổi mã trạng thái sang tiếng Việt (`Pending` → "Chờ duyệt"), mã lạ giữ nguyên |
| `formatDate xử lý null` | Ngày rỗng in ra `-`, không văng lỗi |
| `UserProfile ...` (v0.11) | Đọc JSON của `GET /auth/me`, số điện thoại trống không lỗi |
| `AiSummary ...` (v0.10) | `byLlm` đúng cho `source` = `llm` và `rule` |
| `TrackingInfo ...` (v0.9) | Đọc JSON của `GET /orders/{id}/tracking`; `moved()` áp vị trí mới từ event SignalR `DroneLocation`, giữ nguyên trạm đi/đến và mã drone nếu event không gửi |

Không có test giao diện (widget test thật) vì cần giả lập API và token, để sau.
