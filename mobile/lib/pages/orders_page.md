# orders_page.dart

Màn **Đơn của tôi** – màn chính sau khi đăng nhập.

- Danh sách `GET /orders` (khách chỉ thấy đơn của mình), mỗi dòng: mã đơn, tuyến, nhãn trạng thái. Bấm vào mở chi tiết.
- Kéo xuống để tải lại; nút nổi **Tạo đơn** mở `create_order_page.dart`.
- Mở SignalR (`SignalRService.connect`) khi vào màn. Có event đổi trạng thái thì tải lại và hiện SnackBar "Đơn SDD-...: Đang giao" (v0.7).
- AppBar: chuông có số thông báo chưa đọc → `notifications_page.dart`; menu ⋮ → Trợ lý AI, Sổ địa chỉ (v0.8); nút đăng xuất.

## Thêm ở v0.11

- Ô **Tìm theo mã đơn** trên đầu danh sách: gõ xong 400 ms gọi lại `getOrders(search: ...)`. Không có kết quả thì báo "Không có đơn nào khớp ...".
- Menu ⋮ thêm **Hồ sơ của tôi** → `ProfilePage`.

