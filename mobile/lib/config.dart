/// Địa chỉ backend.
/// - Emulator Android: 10.0.2.2 chính là máy tính đang chạy emulator.
/// - Điện thoại thật: chạy với --dart-define=API_URL=http://<IP-LAN-máy-tính>:5080/api/v1
const String apiBaseUrl = String.fromEnvironment(
  'API_URL',
  defaultValue: 'http://10.0.2.2:5080/api/v1',
);
