# App mobile (Flutter), cho Customer

Đăng nhập / đăng ký, tạo đơn (chọn trạm, nhập kiện, kèm ảnh), danh sách đơn, chi tiết + timeline, hủy đơn khi còn Pending.

Thư mục này chỉ chứa phần code của mình (`lib/`, `test/`, `pubspec.yaml`). Các thư mục Android do Flutter tự sinh.

## Dựng project (làm 1 lần)

Cần Flutter SDK + Android Studio (xem `flutter doctor`). Ở **gốc repo**:

```bash
flutter create --project-name smart_drone_mobile --org vn.edu.ut mobile
```

Rồi chép **đè** `lib/`, `test/`, `pubspec.yaml`, `README.md` từ bản zip vào thư mục `mobile/` vừa tạo (Replace). Sau đó:

```bash
cd mobile
flutter pub get
```

## Sửa Android 1 lần (để gọi được backend http)

Mở `android/app/src/main/AndroidManifest.xml`:
1. Thêm dòng này ngay trên thẻ `<application`:
   `<uses-permission android:name="android.permission.INTERNET"/>`
2. Trong thẻ `<application ...`, thêm thuộc tính: `android:usesCleartextTraffic="true"`

## Chạy

Backend phải đang chạy ở cổng 5080.

```bash
# Emulator Android (10.0.2.2 là máy tính của mày)
flutter run

# Điện thoại thật (cùng wifi, backend chạy với --urls http://0.0.0.0:5080, mở port 5080 trên Firewall)
flutter run --dart-define=API_URL=http://<IP-LAN-may-tinh>:5080/api/v1
```

Tài khoản demo (mật khẩu `Demo@12345`): `customer@drone.local`. Tạo đơn chỉ Customer làm được.

## Test

```bash
flutter test
```

## Các file trong `lib/`

| File | Tác dụng |
|---|---|
| `main.dart` | Điểm vào app; `Gate` kiểm tra đã đăng nhập chưa để mở trang login hoặc danh sách đơn |
| `config.dart` | `apiBaseUrl`, mặc định `http://10.0.2.2:5080/api/v1` (emulator), đổi bằng `--dart-define=API_URL=...` |
| `api.dart` | Lớp `Api` gọi backend (Dio): gắn header `Authorization: Bearer`, đổi lỗi ProblemDetails (`errors` / `detail`) thành `ApiException` có câu tiếng Việt |
| `auth_store.dart` | `AuthStore` giữ token + thông tin user, lưu lại để mở app không phải đăng nhập lại |
| `models.dart` | Model JSON: `AuthData`, `Station`, `PackageInfo`, đơn hàng, trang đơn, lịch sử |
| `format.dart` | Đổi mã trạng thái (`Pending`...) sang nhãn tiếng Việt, định dạng ngày giờ |
| `nav.dart` | `navigatorKey` toàn cục (đá về trang login khi token hết hạn) |
| `widgets.dart` | `StatusChip` hiển thị trạng thái đơn có màu |
| `pages/login_page.dart` | Đăng nhập / đăng ký |
| `pages/orders_page.dart` | Danh sách đơn của khách (`GET /orders`), bấm vào xem chi tiết, nút tạo đơn |
| `pages/create_order_page.dart` | Tạo đơn: chọn trạm gửi/nhận, nhập cân nặng + kích thước, chọn ảnh kiện |
| `pages/order_detail_page.dart` | Chi tiết đơn + timeline (`/orders/{id}/history`), nút hủy khi `Pending` |

| `services/signalr_service.dart` | Kết nối SignalR hub (`signalr_netcore`), đẩy sự kiện nhận được ra `Stream` `updates` |
| `services/signalr_config.dart` | URL hub = gốc của `apiBaseUrl` + `/hubs/delivery`, sự kiện `OrderStatusUpdated` |
| `pages/order_tracking_screen.dart` | Màn theo dõi đơn, **đang dùng dữ liệu cứng** (mã SD001, 4 bước); cần nối API/SignalR hoặc gộp vào `order_detail_page.dart` |

Backend **chưa có** SignalR hub `/hubs/delivery`, nên `SignalRService.connect` hiện sẽ lỗi kết nối cho tới khi làm xong hub.

`test/widget_test.dart`: test mẫu của Flutter.
