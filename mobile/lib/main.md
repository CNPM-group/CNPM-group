# main.dart

Điểm vào của app.

- `main()` chạy `App`.
- `App`: `MaterialApp` với màu chủ đạo `#1F3864`, Material 3, gắn `navigatorKey` (xem `nav.dart`).
- `Gate`: đọc token đã lưu (`AuthStore.load()`); trong lúc đọc hiện vòng xoay, có token còn hạn thì vào `OrdersPage`, không thì `LoginPage`. Dùng `ListenableBuilder` nên đăng nhập / đăng xuất là tự đổi màn, không cần `Navigator.push`.
