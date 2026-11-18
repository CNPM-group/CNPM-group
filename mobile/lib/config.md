# config.dart

`apiBaseUrl` – địa chỉ backend, mặc định `http://10.0.2.2:5080/api/v1`.

- Emulator Android: `10.0.2.2` là máy tính đang chạy emulator (gọi `localhost` sẽ trỏ vào chính emulator).
- Điện thoại thật: `flutter run --dart-define=API_URL=http://<IP-LAN-máy-tính>:5080/api/v1`.

`signalr_config.dart` và ảnh kiện (`/orders/{id}/package-image`) đều dựng URL từ biến này.
