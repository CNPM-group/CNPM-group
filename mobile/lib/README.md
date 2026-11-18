# mobile/lib

Code Flutter của app khách hàng. Mỗi file có một file `.md` cùng tên giải thích công dụng.

| File | Công dụng | Giải thích |
|---|---|---|
| `main.dart` | Điểm vào, chọn màn đăng nhập hay danh sách đơn | [main.md](main.md) |
| `config.dart` | Địa chỉ backend `apiBaseUrl` | [config.md](config.md) |
| `nav.dart` | `navigatorKey` để đá về màn gốc khi token hết hạn | [nav.md](nav.md) |
| `api.dart` | Mọi lời gọi API (Dio), đổi lỗi sang tiếng Việt | [api.md](api.md) |
| `auth_store.dart` | Lưu và đọc token đăng nhập | [auth_store.md](auth_store.md) |
| `models.dart` | Lớp đọc JSON từ API | [models.md](models.md) |
| `format.dart` | Định dạng ngày, nhãn và màu trạng thái | [format.md](format.md) |
| `widgets.dart` | `StatusChip` | [widgets.md](widgets.md) |
| `tracking_map.dart` | Bản đồ mini theo dõi drone (v0.9) | [tracking_map.md](tracking_map.md) |
| `services/signalr_config.dart` | URL hub, tên event | [signalr_config.md](services/signalr_config.md) |
| `services/signalr_service.dart` | Kết nối SignalR, stream trạng thái + vị trí drone | [signalr_service.md](services/signalr_service.md) |
| `pages/login_page.dart` | Đăng nhập / đăng ký | [login_page.md](pages/login_page.md) |
| `pages/orders_page.dart` | Đơn của tôi | [orders_page.md](pages/orders_page.md) |
| `pages/create_order_page.dart` | Tạo đơn | [create_order_page.md](pages/create_order_page.md) |
| `pages/order_detail_page.dart` | Chi tiết đơn + người nhận + theo dõi drone + AI tóm tắt chuyến giao, nút Sửa đơn khi còn chờ duyệt | [order_detail_page.md](pages/order_detail_page.md) |
| `pages/profile_page.dart` | Hồ sơ của tôi: sửa họ tên, số điện thoại, đổi mật khẩu (v0.11) | [profile_page.md](pages/profile_page.md) |
| `pages/notifications_page.dart` | Thông báo + mã OTP | [notifications_page.md](pages/notifications_page.md) |
| `pages/chat_page.dart` | Trợ lý AI | [chat_page.md](pages/chat_page.md) |
| `pages/addresses_page.dart` | Sổ địa chỉ | [addresses_page.md](pages/addresses_page.md) |
